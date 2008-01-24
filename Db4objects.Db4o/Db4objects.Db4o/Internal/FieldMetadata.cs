/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class FieldMetadata : IStoredField
	{
		private ClassMetadata _containingClass;

		private int _arrayPosition;

		private string _name;

		private bool _isArray;

		private bool _isNArray;

		private bool _isPrimitive;

		private IReflectField _reflectField;

		internal ITypeHandler4 _handler;

		protected int _handlerID;

		private int _state;

		private const int NotLoaded = 0;

		private const int Unavailable = -1;

		private const int Available = 1;

		private Config4Field _config;

		private IDb4oTypeImpl _db4oType;

		private int _linkLength;

		private BTree _index;

		internal static readonly Db4objects.Db4o.Internal.FieldMetadata[] EmptyArray = new 
			Db4objects.Db4o.Internal.FieldMetadata[0];

		public FieldMetadata(ClassMetadata classMetadata)
		{
			//  position in ClassMetadata i_fields
			_containingClass = classMetadata;
		}

		internal FieldMetadata(ClassMetadata containingClass, IObjectTranslator translator
			) : this(containingClass)
		{
			// for TranslatedFieldMetadata only
			Init(containingClass, translator.GetType().FullName);
			_state = Available;
			ObjectContainerBase stream = Container();
			IReflectClass claxx = stream.Reflector().ForClass(TranslatorStoredClass(translator
				));
			_handler = FieldHandlerForClass(stream, claxx);
		}

		protected Type TranslatorStoredClass(IObjectTranslator translator)
		{
			try
			{
				return translator.StoredClass();
			}
			catch (Exception e)
			{
				throw new ReflectException(e);
			}
		}

		internal FieldMetadata(ClassMetadata containingClass, IReflectField field, ITypeHandler4
			 handler, int handlerID) : this(containingClass)
		{
			Init(containingClass, field.GetName());
			_reflectField = field;
			_reflectField.SetAccessible();
			_handler = handler;
			_handlerID = handlerID;
			// TODO: beautify !!!  possibly pull up isPrimitive to ReflectField
			bool isPrimitive = false;
			if (field is GenericField)
			{
				isPrimitive = ((GenericField)field).IsPrimitive();
			}
			Configure(field.GetFieldType(), isPrimitive);
			CheckDb4oType();
			_state = Available;
		}

		protected FieldMetadata(int handlerID, ITypeHandler4 handler)
		{
			_handlerID = handlerID;
			_handler = handler;
		}

		/// <param name="classMetadata"></param>
		/// <param name="oldSlot"></param>
		/// <exception cref="FieldIndexException"></exception>
		public virtual void AddFieldIndex(MarshallerFamily mf, ClassMetadata classMetadata
			, StatefulBuffer buffer, Slot oldSlot)
		{
			if (!HasIndex())
			{
				IncrementOffset(buffer);
				return;
			}
			try
			{
				AddIndexEntry(buffer, ReadIndexEntry(mf, buffer));
			}
			catch (CorruptionException exc)
			{
				throw new FieldIndexException(exc, this);
			}
		}

		protected virtual void AddIndexEntry(StatefulBuffer a_bytes, object indexEntry)
		{
			AddIndexEntry(a_bytes.GetTransaction(), a_bytes.GetID(), indexEntry);
		}

		public virtual void AddIndexEntry(Transaction trans, int parentID, object indexEntry
			)
		{
			if (!HasIndex())
			{
				return;
			}
			BTree index = GetIndex(trans);
			// Although we checked hasIndex() already, we have to check
			// again here since index creation in YapFieldUUID can be
			// unsuccessful if it's called too early for PBootRecord.
			if (index == null)
			{
				return;
			}
			index.Add(trans, CreateFieldIndexKey(parentID, indexEntry));
		}

		private FieldIndexKey CreateFieldIndexKey(int parentID, object indexEntry)
		{
			object convertedIndexEntry = IndexEntryFor(indexEntry);
			return new FieldIndexKey(parentID, convertedIndexEntry);
		}

		protected virtual object IndexEntryFor(object indexEntry)
		{
			return _reflectField.IndexEntry(indexEntry);
		}

		public virtual bool CanUseNullBitmap()
		{
			return true;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer writer)
		{
			// alive() checked
			return ((IIndexableTypeHandler)_handler).ReadIndexEntry(mf, writer);
		}

		public virtual void RemoveIndexEntry(Transaction trans, int parentID, object indexEntry
			)
		{
			if (!HasIndex())
			{
				return;
			}
			if (_index == null)
			{
				return;
			}
			_index.Remove(trans, CreateFieldIndexKey(parentID, indexEntry));
		}

		public virtual bool Alive()
		{
			if (_state == Available)
			{
				return true;
			}
			if (_state == NotLoaded)
			{
				if (_handler == null)
				{
					// this may happen if the local ClassMetadataRepository
					// has not been updated from the server and presumably 
					// in some refactoring cases. 
					// We try to heal the problem by re-reading the class.
					// This could be dangerous, if the class type of a field
					// has been modified.
					// TODO: add class refactoring features
					_handler = DetectHandlerForField();
					CheckHandlerID();
				}
				CheckCorrectHandlerForField();
				if (_handler != null)
				{
					// TODO: This part is not quite correct.
					// We are using the old array information read from file to wrap.
					// If a schema evolution changes an array to a different variable,
					// we are in trouble here.
					_handler = WrapHandlerToArrays(Container(), _handler);
				}
				if (_handler == null || _reflectField == null)
				{
					_state = Unavailable;
					_reflectField = null;
				}
				else
				{
					_state = Available;
					CheckDb4oType();
				}
			}
			return _state == Available;
		}

		private void CheckHandlerID()
		{
			if (!(_handler is ClassMetadata))
			{
				return;
			}
			ClassMetadata classMetadata = (ClassMetadata)_handler;
			int id = classMetadata.GetID();
			if (_handlerID == 0)
			{
				_handlerID = id;
				return;
			}
			if (id > 0 && id != _handlerID)
			{
				// wrong type, refactoring, field should be turned off
				// TODO: it would be cool to log something here
				_handler = null;
			}
		}

		internal virtual bool CanAddToQuery(string fieldName)
		{
			if (!Alive())
			{
				return false;
			}
			return fieldName.Equals(GetName()) && ContainingClass() != null && !ContainingClass
				().IsInternal();
		}

		public virtual bool CanHold(IReflectClass claxx)
		{
			// alive() is checked in QField caller
			if (claxx == null)
			{
				return !_isPrimitive;
			}
			return Handlers4.HandlerCanHold(_handler, claxx);
		}

		public virtual object Coerce(IReflectClass claxx, object obj)
		{
			// alive() is checked in QField caller
			if (claxx == null || obj == null)
			{
				return _isPrimitive ? No4.Instance : obj;
			}
			if (_handler is PrimitiveHandler)
			{
				return ((PrimitiveHandler)_handler).Coerce(claxx, obj);
			}
			if (!CanHold(claxx))
			{
				return No4.Instance;
			}
			return obj;
		}

		public bool CanLoadByIndex()
		{
			if (_handler is ClassMetadata)
			{
				ClassMetadata yc = (ClassMetadata)_handler;
				if (yc.IsArray())
				{
					return false;
				}
			}
			return true;
		}

		internal void CascadeActivation(Transaction trans, object onObject, IActivationDepth
			 depth)
		{
			if (!Alive())
			{
				return;
			}
			if (!(_handler is IFirstClassHandler))
			{
				return;
			}
			object cascadeTo = CascadingTarget(trans, depth, onObject);
			if (cascadeTo == null)
			{
				return;
			}
			IFirstClassHandler firstClassHandler = (IFirstClassHandler)_handler;
			firstClassHandler.CascadeActivation(trans, cascadeTo, depth);
		}

		protected virtual object CascadingTarget(Transaction trans, IActivationDepth depth
			, object onObject)
		{
			if (depth.Mode().IsDeactivate())
			{
				if (null == _reflectField)
				{
					return null;
				}
				return _reflectField.Get(onObject);
			}
			return GetOrCreate(trans, onObject);
		}

		private void CheckDb4oType()
		{
			if (_reflectField != null)
			{
				if (Container()._handlers.IclassDb4otype.IsAssignableFrom(_reflectField.GetFieldType
					()))
				{
					_db4oType = HandlerRegistry.GetDb4oType(_reflectField.GetFieldType());
				}
			}
		}

		internal virtual void CollectConstraints(Transaction a_trans, QConObject a_parent
			, object a_template, IVisitor4 a_visitor)
		{
			object obj = GetOn(a_trans, a_template);
			if (obj != null)
			{
				Collection4 objs = Platform4.FlattenCollection(a_trans.Container(), obj);
				IEnumerator j = objs.GetEnumerator();
				while (j.MoveNext())
				{
					obj = j.Current;
					if (obj != null)
					{
						if (_isPrimitive)
						{
							if (_handler is PrimitiveHandler)
							{
								if (obj.Equals(((PrimitiveHandler)_handler).PrimitiveNull()))
								{
									return;
								}
							}
						}
						if (Platform4.IgnoreAsConstraint(obj))
						{
							return;
						}
						if (!a_parent.HasObjectInParentPath(obj))
						{
							QConObject constraint = new QConObject(a_trans, a_parent, QField(a_trans), obj);
							constraint.ByExample();
							a_visitor.Visit(constraint);
						}
					}
				}
			}
		}

		/// <exception cref="FieldIndexException"></exception>
		public TreeInt CollectIDs(MarshallerFamily mf, TreeInt tree, StatefulBuffer a_bytes
			)
		{
			if (!Alive())
			{
				return tree;
			}
			if (_handler is ClassMetadata)
			{
				return (TreeInt)Tree.Add(tree, new TreeInt(a_bytes.ReadInt()));
			}
			else
			{
				if (_handler is ArrayHandler)
				{
					return ((ArrayHandler)_handler).CollectIDs(mf, tree, a_bytes);
				}
			}
			return tree;
		}

		internal virtual void Configure(IReflectClass clazz, bool isPrimitive)
		{
			_isArray = clazz.IsArray();
			if (_isArray)
			{
				IReflectArray reflectArray = Container().Reflector().Array();
				_isNArray = reflectArray.IsNDimensional(clazz);
				_isPrimitive = reflectArray.GetComponentType(clazz).IsPrimitive();
				_handler = WrapHandlerToArrays(Container(), _handler);
			}
			else
			{
				_isPrimitive = isPrimitive | clazz.IsPrimitive();
			}
		}

		private ITypeHandler4 WrapHandlerToArrays(ObjectContainerBase container, ITypeHandler4
			 handler)
		{
			if (_isNArray)
			{
				return new MultidimensionalArrayHandler(container, handler, ArraysUsePrimitiveClassReflector
					());
			}
			if (_isArray)
			{
				return new ArrayHandler(container, handler, ArraysUsePrimitiveClassReflector());
			}
			return handler;
		}

		private bool ArraysUsePrimitiveClassReflector()
		{
			return Deploy.csharp ? false : _isPrimitive;
		}

		internal virtual void Deactivate(Transaction a_trans, object a_onObject, IActivationDepth
			 a_depth)
		{
			if (!Alive())
			{
				return;
			}
			bool isEnumClass = _containingClass.IsEnum();
			if (_isPrimitive && !_isArray)
			{
				if (!isEnumClass)
				{
					_reflectField.Set(a_onObject, ((PrimitiveHandler)_handler).PrimitiveNull());
				}
				return;
			}
			if (a_depth.RequiresActivation())
			{
				CascadeActivation(a_trans, a_onObject, a_depth);
			}
			if (!isEnumClass)
			{
				_reflectField.Set(a_onObject, null);
			}
		}

		/// <param name="isUpdate"></param>
		/// <exception cref="FieldIndexException"></exception>
		public virtual void Delete(MarshallerFamily mf, StatefulBuffer buffer, bool isUpdate
			)
		{
			if (!CheckAlive(buffer))
			{
				return;
			}
			DeleteContextImpl context = new DeleteContextImpl(buffer, mf.HandlerVersion());
			try
			{
				RemoveIndexEntry(mf, buffer);
				bool dotnetValueType = false;
				dotnetValueType = Platform4.IsValueType(GetStoredType());
				if ((_config != null && _config.CascadeOnDelete().DefiniteYes()) || dotnetValueType
					)
				{
					DeleteWithCascadeDepth(context, 1);
				}
				else
				{
					if (_config != null && _config.CascadeOnDelete().DefiniteNo())
					{
						DeleteWithCascadeDepth(context, 0);
					}
					else
					{
						context.CorrectHandlerVersion(_handler).Delete(context);
					}
				}
			}
			catch (CorruptionException exc)
			{
				throw new FieldIndexException(exc, this);
			}
		}

		private void DeleteWithCascadeDepth(DeleteContextImpl context, int depth)
		{
			int preservedCascadeDepth = context.CascadeDeleteDepth();
			context.CascadeDeleteDepth(depth);
			context.CorrectHandlerVersion(_handler).Delete(context);
			context.CascadeDeleteDepth(preservedCascadeDepth);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		private void RemoveIndexEntry(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
			if (!HasIndex())
			{
				return;
			}
			int offset = a_bytes._offset;
			object obj = ReadIndexEntry(mf, a_bytes);
			RemoveIndexEntry(a_bytes.GetTransaction(), a_bytes.GetID(), obj);
			a_bytes._offset = offset;
		}

		public override bool Equals(object obj)
		{
			if (obj is Db4objects.Db4o.Internal.FieldMetadata)
			{
				Db4objects.Db4o.Internal.FieldMetadata yapField = (Db4objects.Db4o.Internal.FieldMetadata
					)obj;
				yapField.Alive();
				Alive();
				return yapField._isPrimitive == _isPrimitive && yapField._handler.Equals(_handler
					) && yapField._name.Equals(_name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return _name.GetHashCode();
		}

		public object Get(object onObject)
		{
			return Get(null, onObject);
		}

		public object Get(Transaction trans, object onObject)
		{
			if (_containingClass == null)
			{
				return null;
			}
			ObjectContainerBase container = Container();
			if (container == null)
			{
				return null;
			}
			lock (container._lock)
			{
				// FIXME: The following is not really transactional.
				//        This will work OK for normal C/S and for
				//        single local mode but the transaction will
				//        be wrong for MTOC.
				if (trans == null)
				{
					trans = container.Transaction();
				}
				container.CheckClosed();
				ObjectReference @ref = trans.ReferenceForObject(onObject);
				if (@ref == null)
				{
					return null;
				}
				int id = @ref.GetID();
				if (id <= 0)
				{
					return null;
				}
				UnmarshallingContext context = new UnmarshallingContext(trans, @ref, Const4.AddToIdTree
					, false);
				context.ActivationDepth(new LegacyActivationDepth(1));
				return context.ReadFieldValue(this);
			}
		}

		public virtual string GetName()
		{
			return _name;
		}

		public ClassMetadata HandlerClassMetadata(ObjectContainerBase container)
		{
			// alive needs to be checked by all callers: Done
			ITypeHandler4 handler = BaseTypeHandler();
			if (Handlers4.HandlesSimple(handler))
			{
				return container._handlers.ClassMetadataForId(HandlerID());
			}
			return (ClassMetadata)handler;
		}

		private ITypeHandler4 BaseTypeHandler()
		{
			return Handlers4.BaseTypeHandler(_handler);
		}

		public virtual ITypeHandler4 GetHandler()
		{
			// alive needs to be checked by all callers: Done
			return _handler;
		}

		public virtual int HandlerID()
		{
			// alive needs to be checked by all callers: Done
			return _handlerID;
		}

		/// <param name="trans"></param>
		public virtual object GetOn(Transaction trans, object onObject)
		{
			if (Alive())
			{
				return _reflectField.Get(onObject);
			}
			return null;
		}

		/// <summary>
		/// dirty hack for com.db4o.types some of them need to be set automatically
		/// TODO: Derive from YapField for Db4oTypes
		/// </summary>
		public virtual object GetOrCreate(Transaction trans, object onObject)
		{
			if (!Alive())
			{
				return null;
			}
			object obj = _reflectField.Get(onObject);
			if (_db4oType != null && obj == null)
			{
				obj = _db4oType.CreateDefault(trans);
				_reflectField.Set(onObject, obj);
			}
			return obj;
		}

		public ClassMetadata ContainingClass()
		{
			// alive needs to be checked by all callers: Done
			return _containingClass;
		}

		public virtual IReflectClass GetStoredType()
		{
			if (_reflectField == null)
			{
				return null;
			}
			return Handlers4.BaseType(_reflectField.GetFieldType());
		}

		public virtual ObjectContainerBase Container()
		{
			if (_containingClass == null)
			{
				return null;
			}
			return _containingClass.Container();
		}

		public virtual bool HasConfig()
		{
			return _config != null;
		}

		public virtual bool HasIndex()
		{
			// alive needs to be checked by all callers: Done
			return _index != null;
		}

		public void IncrementOffset(IBuffer buffer)
		{
			buffer.Seek(buffer.Offset() + LinkLength());
		}

		public void Init(ClassMetadata containingClass, string name)
		{
			_containingClass = containingClass;
			_name = name;
			InitIndex(containingClass, name);
		}

		internal void InitIndex(ClassMetadata containingClass, string name)
		{
			if (containingClass.Config() == null)
			{
				return;
			}
			_config = containingClass.Config().ConfigField(name);
			if (Debug.configureAllFields && _config == null)
			{
				_config = (Config4Field)containingClass.Config().ObjectField(_name);
			}
		}

		public virtual void Init(int handlerID, bool isPrimitive, bool isArray, bool isNArray
			)
		{
			_handlerID = handlerID;
			_isPrimitive = isPrimitive;
			_isArray = isArray;
			_isNArray = isNArray;
		}

		private bool _initialized = false;

		internal void InitConfigOnUp(Transaction trans)
		{
			if (_config != null && !_initialized)
			{
				_initialized = true;
				_config.InitOnUp(trans, this);
			}
		}

		public virtual void Instantiate(UnmarshallingContext context)
		{
			if (!CheckAlive(context.Buffer()))
			{
				return;
			}
			object toSet = Read(context);
			InformAboutTransaction(toSet, context.Transaction());
			Set(context.PersistentObject(), toSet);
		}

		private bool CheckAlive(IBuffer buffer)
		{
			bool alive = Alive();
			if (!alive)
			{
				IncrementOffset(buffer);
			}
			return alive;
		}

		private void InformAboutTransaction(object obj, Transaction trans)
		{
			if (_db4oType != null && obj != null)
			{
				((IDb4oTypeImpl)obj).SetTrans(trans);
			}
		}

		public virtual bool IsArray()
		{
			return _isArray;
		}

		protected virtual int LinkLength()
		{
			Alive();
			if (_linkLength == 0)
			{
				_linkLength = CalculateLinkLength();
			}
			return _linkLength;
		}

		private int CalculateLinkLength()
		{
			if (_handler == null)
			{
				// must be ClassMetadata
				return Const4.IdLength;
			}
			if (_handler is PersistentBase)
			{
				return ((PersistentBase)_handler).LinkLength();
			}
			if (_handler is PrimitiveHandler)
			{
				return ((PrimitiveHandler)_handler).LinkLength();
			}
			if (_handler is VariableLengthTypeHandler)
			{
				return ((VariableLengthTypeHandler)_handler).LinkLength();
			}
			// TODO: For custom handlers there will have to be a way 
			//       to calculate the length in the slot.
			//        Options:
			//        (1) Remember when the first object is marshalled.
			//        (2) Add a #defaultValue() method to TypeHandler4,
			//            marshall the default value and check.
			//        (3) Add a way to test the custom handler when it
			//            is installed and remember the length there. 
			throw new NotImplementedException();
		}

		public virtual void LoadHandlerById(ObjectContainerBase container)
		{
			_handler = (ITypeHandler4)container.FieldHandlerForId(_handlerID);
		}

		private ITypeHandler4 DetectHandlerForField()
		{
			IReflectClass claxx = _containingClass.ClassReflector();
			if (claxx == null)
			{
				return null;
			}
			_reflectField = claxx.GetDeclaredField(_name);
			if (_reflectField == null)
			{
				return null;
			}
			_reflectField.SetAccessible();
			return FieldHandlerForClass(Container(), _reflectField.GetFieldType());
		}

		private ITypeHandler4 FieldHandlerForClass(ObjectContainerBase container, IReflectClass
			 fieldType)
		{
			container.ShowInternalClasses(true);
			ITypeHandler4 handlerForClass = (ITypeHandler4)container.FieldHandlerForClass(Handlers4
				.BaseType(fieldType));
			container.ShowInternalClasses(false);
			return handlerForClass;
		}

		private void CheckCorrectHandlerForField()
		{
			ITypeHandler4 handler = DetectHandlerForField();
			if (handler == null || (!handler.Equals(_handler)))
			{
				_reflectField = null;
				_state = Unavailable;
			}
		}

		private int AdjustUpdateDepth(object obj, int updateDepth)
		{
			int minimumUpdateDepth = 1;
			if (_containingClass.IsCollection(obj))
			{
				GenericReflector reflector = _containingClass.Reflector();
				minimumUpdateDepth = reflector.CollectionUpdateDepth(reflector.ForObject(obj));
			}
			if (updateDepth < minimumUpdateDepth)
			{
				return minimumUpdateDepth;
			}
			return updateDepth;
		}

		private bool CascadeOnUpdate(Config4Class parentClassConfiguration)
		{
			return ((parentClassConfiguration != null && (parentClassConfiguration.CascadeOnUpdate
				().DefiniteYes())) || (_config != null && (_config.CascadeOnUpdate().DefiniteYes
				())));
		}

		public virtual void Marshall(MarshallingContext context, object obj)
		{
			// alive needs to be checked by all callers: Done
			int updateDepth = context.UpdateDepth();
			if (obj != null && CascadeOnUpdate(context.ClassConfiguration()))
			{
				context.UpdateDepth(AdjustUpdateDepth(obj, updateDepth));
			}
			context.CreateIndirection(_handler);
			_handler.Write(context, obj);
			context.UpdateDepth(updateDepth);
			if (HasIndex())
			{
				context.AddIndexEntry(this, obj);
			}
		}

		public virtual bool NeedsArrayAndPrimitiveInfo()
		{
			return true;
		}

		public virtual bool NeedsHandlerId()
		{
			return true;
		}

		public virtual IPreparedComparison PrepareComparison(object obj)
		{
			if (!Alive())
			{
				return null;
			}
			return _handler.PrepareComparison(obj);
		}

		public virtual Db4objects.Db4o.Internal.Query.Processor.QField QField(Transaction
			 a_trans)
		{
			int yapClassID = 0;
			if (_containingClass != null)
			{
				yapClassID = _containingClass.GetID();
			}
			return new Db4objects.Db4o.Internal.Query.Processor.QField(a_trans, _name, this, 
				yapClassID, _arrayPosition);
		}

		public virtual object Read(IInternalReadContext context)
		{
			if (!CheckAlive(context.Buffer()))
			{
				return null;
			}
			return context.Read(_handler);
		}

		/// <param name="trans"></param>
		/// <param name="@ref"></param>
		public virtual void ReadVirtualAttribute(Transaction trans, BufferImpl buffer, ObjectReference
			 @ref)
		{
			IncrementOffset(buffer);
		}

		public virtual void RefreshActivated()
		{
			_state = Available;
			Refresh();
		}

		internal virtual void Refresh()
		{
			ITypeHandler4 handler = DetectHandlerForField();
			if (handler != null)
			{
				handler = WrapHandlerToArrays(Container(), handler);
				if (handler.Equals(_handler))
				{
					return;
				}
			}
			_reflectField = null;
			_state = Unavailable;
		}

		public virtual void Rename(string newName)
		{
			// FIXME: needs test case
			ObjectContainerBase container = Container();
			if (!container.IsClient())
			{
				_name = newName;
				_containingClass.SetStateDirty();
				_containingClass.Write(container.SystemTransaction());
			}
			else
			{
				Exceptions4.ThrowRuntimeException(58);
			}
		}

		public virtual void SetArrayPosition(int a_index)
		{
			_arrayPosition = a_index;
		}

		public virtual void Set(object onObject, object obj)
		{
			// TODO: remove the following if and check callers
			if (null == _reflectField)
			{
				return;
			}
			_reflectField.Set(onObject, obj);
		}

		internal virtual void SetName(string a_name)
		{
			_name = a_name;
		}

		internal virtual bool SupportsIndex()
		{
			return Alive() && (_handler is IIndexable4) && (!(_handler is UntypedFieldHandler
				));
		}

		public void TraverseValues(IVisitor4 userVisitor)
		{
			if (!Alive())
			{
				return;
			}
			TraverseValues(Container().Transaction(), userVisitor);
		}

		public void TraverseValues(Transaction transaction, IVisitor4 userVisitor)
		{
			if (!Alive())
			{
				return;
			}
			AssertHasIndex();
			ObjectContainerBase stream = transaction.Container();
			if (stream.IsClient())
			{
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.ClientServerUnsupported
					);
			}
			lock (stream.Lock())
			{
				_index.TraverseKeys(transaction, new _IVisitor4_854(this, userVisitor, transaction
					));
			}
		}

		private sealed class _IVisitor4_854 : IVisitor4
		{
			public _IVisitor4_854(FieldMetadata _enclosing, IVisitor4 userVisitor, Transaction
				 transaction)
			{
				this._enclosing = _enclosing;
				this.userVisitor = userVisitor;
				this.transaction = transaction;
			}

			public void Visit(object obj)
			{
				FieldIndexKey key = (FieldIndexKey)obj;
				userVisitor.Visit(((IIndexableTypeHandler)this._enclosing._handler).IndexEntryToObject
					(transaction, key.Value()));
			}

			private readonly FieldMetadata _enclosing;

			private readonly IVisitor4 userVisitor;

			private readonly Transaction transaction;
		}

		private void AssertHasIndex()
		{
			if (!HasIndex())
			{
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.OnlyForIndexedFields
					);
			}
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (_containingClass != null)
			{
				sb.Append(_containingClass.GetName());
				sb.Append(".");
				sb.Append(GetName());
			}
			return sb.ToString();
		}

		private void InitIndex(Transaction systemTrans)
		{
			InitIndex(systemTrans, 0);
		}

		public virtual void InitIndex(Transaction systemTrans, int id)
		{
			if (_index != null)
			{
				throw new InvalidOperationException();
			}
			if (systemTrans.Container().IsClient())
			{
				return;
			}
			_index = NewBTree(systemTrans, id);
		}

		protected BTree NewBTree(Transaction systemTrans, int id)
		{
			ObjectContainerBase stream = systemTrans.Container();
			IIndexable4 indexHandler = IndexHandler(stream);
			if (indexHandler == null)
			{
				return null;
			}
			return new BTree(systemTrans, id, new FieldIndexKeyHandler(stream, indexHandler));
		}

		protected virtual IIndexable4 IndexHandler(ObjectContainerBase stream)
		{
			if (_reflectField == null)
			{
				return null;
			}
			IReflectClass indexType = _reflectField.IndexType();
			ITypeHandler4 classHandler = FieldHandlerForClass(stream, indexType);
			if (!(classHandler is IIndexable4))
			{
				return null;
			}
			return (IIndexable4)classHandler;
		}

		/// <param name="trans"></param>
		public virtual BTree GetIndex(Transaction trans)
		{
			return _index;
		}

		public virtual bool IsVirtual()
		{
			return false;
		}

		public virtual bool IsPrimitive()
		{
			return _isPrimitive;
		}

		public virtual IBTreeRange Search(Transaction transaction, object value)
		{
			AssertHasIndex();
			object transActionalValue = WrapWithTransactionContext(transaction, value);
			BTreeNodeSearchResult lowerBound = SearchLowerBound(transaction, transActionalValue
				);
			BTreeNodeSearchResult upperBound = SearchUpperBound(transaction, transActionalValue
				);
			return lowerBound.CreateIncludingRange(upperBound);
		}

		private object WrapWithTransactionContext(Transaction transaction, object value)
		{
			if (_handler is ClassMetadata)
			{
				value = ((ClassMetadata)_handler).WrapWithTransactionContext(transaction, value);
			}
			return value;
		}

		private BTreeNodeSearchResult SearchUpperBound(Transaction transaction, object value
			)
		{
			return SearchBound(transaction, int.MaxValue, value);
		}

		private BTreeNodeSearchResult SearchLowerBound(Transaction transaction, object value
			)
		{
			return SearchBound(transaction, 0, value);
		}

		private BTreeNodeSearchResult SearchBound(Transaction transaction, int parentID, 
			object keyPart)
		{
			return GetIndex(transaction).SearchLeaf(transaction, CreateFieldIndexKey(parentID
				, keyPart), SearchTarget.Lowest);
		}

		public virtual bool RebuildIndexForClass(LocalObjectContainer stream, ClassMetadata
			 yapClass)
		{
			// FIXME: BTree traversal over index here.
			long[] ids = yapClass.GetIDs();
			for (int i = 0; i < ids.Length; i++)
			{
				RebuildIndexForObject(stream, yapClass, (int)ids[i]);
			}
			return ids.Length > 0;
		}

		/// <param name="classMetadata"></param>
		/// <exception cref="FieldIndexException"></exception>
		protected virtual void RebuildIndexForObject(LocalObjectContainer stream, ClassMetadata
			 classMetadata, int objectId)
		{
			StatefulBuffer writer = stream.ReadWriterByID(stream.SystemTransaction(), objectId
				);
			if (writer != null)
			{
				RebuildIndexForWriter(stream, writer, objectId);
			}
		}

		protected virtual void RebuildIndexForWriter(LocalObjectContainer stream, StatefulBuffer
			 writer, int objectId)
		{
			ObjectHeader oh = new ObjectHeader(stream, writer);
			object obj = ReadIndexEntryForRebuild(writer, oh);
			AddIndexEntry(stream.SystemTransaction(), objectId, obj);
		}

		private object ReadIndexEntryForRebuild(StatefulBuffer writer, ObjectHeader oh)
		{
			return oh.ObjectMarshaller().ReadIndexEntry(oh.ClassMetadata(), oh._headerAttributes
				, this, writer);
		}

		public virtual void DropIndex(Transaction systemTrans)
		{
			if (_index == null)
			{
				return;
			}
			ObjectContainerBase stream = systemTrans.Container();
			if (stream.ConfigImpl().MessageLevel() > Const4.None)
			{
				stream.Message("dropping index " + ToString());
			}
			_index.Free(systemTrans);
			stream.SetDirtyInSystemTransaction(ContainingClass());
			_index = null;
		}

		public virtual void DefragField(MarshallerFamily mf, DefragmentContextImpl context
			)
		{
			context.HandlerVersion(mf.HandlerVersion());
			context.CorrectHandlerVersion(GetHandler()).Defragment(context);
		}

		public virtual void CreateIndex()
		{
			if (HasIndex())
			{
				return;
			}
			LocalObjectContainer container = (LocalObjectContainer)Container();
			if (container.ConfigImpl().MessageLevel() > Const4.None)
			{
				container.Message("creating index " + ToString());
			}
			InitIndex(container.SystemTransaction());
			container.SetDirtyInSystemTransaction(ContainingClass());
			Reindex(container);
		}

		private void Reindex(LocalObjectContainer container)
		{
			ClassMetadata clazz = ContainingClass();
			if (RebuildIndexForClass(container, clazz))
			{
				container.SystemTransaction().Commit();
			}
		}
	}
}
