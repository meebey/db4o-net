/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Handlers.Array;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class FieldMetadata : ClassAspect, IStoredField
	{
		private ClassMetadata _containingClass;

		private string _name;

		private bool _isArray;

		private bool _isNArray;

		private bool _isPrimitive;

		private IReflectField _reflectField;

		internal ITypeHandler4 _handler;

		protected int _handlerID;

		private FieldMetadataState _state = FieldMetadataState.NotLoaded;

		private Config4Field _config;

		private IDb4oTypeImpl _db4oType;

		private int _linkLength;

		private BTree _index;

		internal static readonly Db4objects.Db4o.Internal.FieldMetadata[] EmptyArray = new 
			Db4objects.Db4o.Internal.FieldMetadata[0];

		public FieldMetadata(ClassMetadata classMetadata)
		{
			_containingClass = classMetadata;
		}

		internal FieldMetadata(ClassMetadata containingClass, IObjectTranslator translator
			) : this(containingClass)
		{
			// for TranslatedFieldMetadata only
			Init(containingClass, translator.GetType().FullName);
			_state = FieldMetadataState.Available;
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
			_state = FieldMetadataState.Available;
		}

		protected FieldMetadata(int handlerID, ITypeHandler4 handler)
		{
			_handlerID = handlerID;
			_handler = handler;
		}

		/// <exception cref="FieldIndexException"></exception>
		public virtual void AddFieldIndex(ObjectIdContextImpl context, Slot oldSlot)
		{
			if (!HasIndex())
			{
				IncrementOffset(context);
				return;
			}
			try
			{
				AddIndexEntry(context.Transaction(), context.Id(), ReadIndexEntry(context));
			}
			catch (CorruptionException exc)
			{
				throw new FieldIndexException(exc, this);
			}
		}

		protected void AddIndexEntry(StatefulBuffer a_bytes, object indexEntry)
		{
			AddIndexEntry(a_bytes.Transaction(), a_bytes.GetID(), indexEntry);
		}

		public void AddIndexEntry(Transaction trans, int parentID, object indexEntry)
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
		public object ReadIndexEntry(IObjectIdContext context)
		{
			IIndexableTypeHandler indexableTypeHandler = (IIndexableTypeHandler)Handlers4.CorrectHandlerVersion
				(context, _handler);
			return indexableTypeHandler.ReadIndexEntry(context);
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
			if (_state == FieldMetadataState.Available)
			{
				return true;
			}
			if (_state == FieldMetadataState.NotLoaded)
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
				// TODO: This part is not quite correct.
				// We are using the old array information read from file to wrap.
				// If a schema evolution changes an array to a different variable,
				// we are in trouble here.
				_handler = WrapHandlerToArrays(_handler);
				if (_handler == null || _reflectField == null)
				{
					_state = FieldMetadataState.Unavailable;
					_reflectField = null;
				}
				else
				{
					if (!Updating())
					{
						_state = FieldMetadataState.Available;
						CheckDb4oType();
					}
				}
			}
			return _state == FieldMetadataState.Available;
		}

		public virtual bool Updating()
		{
			return _state == FieldMetadataState.Updating;
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
			return Handlers4.HandlerCanHold(_handler, Reflector(), claxx);
		}

		private GenericReflector Reflector()
		{
			ObjectContainerBase container = Container();
			if (container == null)
			{
				return null;
			}
			return container.Reflector();
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
				return ((PrimitiveHandler)_handler).Coerce(Reflector(), claxx, obj);
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

		public sealed override void CascadeActivation(Transaction trans, object onObject, 
			IActivationDepth depth)
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
			EnsureObjectIsActive(trans, cascadeTo, depth);
			IFirstClassHandler cascadingHandler = (IFirstClassHandler)_handler;
			ActivationContext4 context = new ActivationContext4(trans, cascadeTo, depth);
			cascadingHandler.CascadeActivation(context);
		}

		private void EnsureObjectIsActive(Transaction trans, object cascadeTo, IActivationDepth
			 depth)
		{
			if (!depth.Mode().IsActivate())
			{
				return;
			}
			if (_handler is IEmbeddedTypeHandler)
			{
				return;
			}
			ObjectContainerBase container = trans.Container();
			ClassMetadata classMetadata = container.ClassMetadataForObject(cascadeTo);
			if (classMetadata == null || classMetadata.IsPrimitive())
			{
				return;
			}
			if (container.IsActive(cascadeTo))
			{
				return;
			}
			container.StillToActivate(trans, cascadeTo, depth.Descend(classMetadata));
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

		internal virtual void CollectConstraints(Transaction trans, QConObject a_parent, 
			object a_template, IVisitor4 a_visitor)
		{
			object obj = GetOn(trans, a_template);
			if (obj != null)
			{
				Collection4 objs = Platform4.FlattenCollection(trans.Container(), obj);
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
								object nullValue = _reflectField.GetFieldType().NullValue();
								if (obj.Equals(nullValue))
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
							QConObject constraint = new QConObject(trans, a_parent, QField(trans), obj);
							constraint.ByExample();
							a_visitor.Visit(constraint);
						}
					}
				}
			}
		}

		/// <exception cref="FieldIndexException"></exception>
		public sealed override void CollectIDs(CollectIdContext context)
		{
			if (!Alive())
			{
				return;
			}
			ITypeHandler4 handler = Handlers4.CorrectHandlerVersion(context, _handler);
			if (!(handler is IFirstClassHandler))
			{
				return;
			}
			if (handler is ClassMetadata)
			{
				context.AddId();
				return;
			}
			LocalObjectContainer container = (LocalObjectContainer)context.Container();
			SlotFormat slotFormat = context.SlotFormat();
			if (slotFormat.HandleAsObject(handler))
			{
				// TODO: Code is similar to QCandidate.readArrayCandidates. Try to refactor to one place.
				int collectionID = context.ReadInt();
				ByteArrayBuffer collectionBuffer = container.ReadReaderByID(context.Transaction()
					, collectionID);
				ObjectHeader objectHeader = new ObjectHeader(container, collectionBuffer);
				QueryingReadContext subContext = new QueryingReadContext(context.Transaction(), context
					.HandlerVersion(), collectionBuffer, collectionID, context.Collector());
				objectHeader.ClassMetadata().CollectIDs(subContext);
				return;
			}
			QueryingReadContext queryingReadContext = new QueryingReadContext(context.Transaction
				(), context.HandlerVersion(), context.Buffer(), 0, context.Collector());
			slotFormat.DoWithSlotIndirection(queryingReadContext, handler, new _IClosure4_399
				(handler, queryingReadContext));
		}

		private sealed class _IClosure4_399 : IClosure4
		{
			public _IClosure4_399(ITypeHandler4 handler, QueryingReadContext queryingReadContext
				)
			{
				this.handler = handler;
				this.queryingReadContext = queryingReadContext;
			}

			public object Run()
			{
				((IFirstClassHandler)handler).CollectIDs(queryingReadContext);
				return null;
			}

			private readonly ITypeHandler4 handler;

			private readonly QueryingReadContext queryingReadContext;
		}

		internal virtual void Configure(IReflectClass clazz, bool isPrimitive)
		{
			_isArray = clazz.IsArray();
			if (_isArray)
			{
				IReflectArray reflectArray = Reflector().Array();
				_isNArray = reflectArray.IsNDimensional(clazz);
				_isPrimitive = reflectArray.GetComponentType(clazz).IsPrimitive();
				_handler = WrapHandlerToArrays(_handler);
			}
			else
			{
				_isPrimitive = isPrimitive | clazz.IsPrimitive();
			}
		}

		private ITypeHandler4 WrapHandlerToArrays(ITypeHandler4 handler)
		{
			if (handler == null)
			{
				return null;
			}
			if (_isNArray)
			{
				return new MultidimensionalArrayHandler(handler, ArraysUsePrimitiveClassReflector
					());
			}
			if (_isArray)
			{
				return new ArrayHandler(handler, ArraysUsePrimitiveClassReflector());
			}
			return handler;
		}

		private bool ArraysUsePrimitiveClassReflector()
		{
			return _isPrimitive;
		}

		public override void Deactivate(Transaction a_trans, object a_onObject, IActivationDepth
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
					object nullValue = _reflectField.GetFieldType().NullValue();
					_reflectField.Set(a_onObject, nullValue);
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
		public override void Delete(DeleteContextImpl context, bool isUpdate)
		{
			if (!CheckAlive(context))
			{
				return;
			}
			try
			{
				RemoveIndexEntry(context);
				StatefulBuffer buffer = (StatefulBuffer)context.Buffer();
				DeleteContextImpl childContext = new DeleteContextImpl(context, GetStoredType(), 
					_config);
				context.SlotFormat().DoWithSlotIndirection(buffer, _handler, new _IClosure4_468(this
					, childContext));
			}
			catch (CorruptionException exc)
			{
				throw new FieldIndexException(exc, this);
			}
		}

		private sealed class _IClosure4_468 : IClosure4
		{
			public _IClosure4_468(FieldMetadata _enclosing, DeleteContextImpl childContext)
			{
				this._enclosing = _enclosing;
				this.childContext = childContext;
			}

			public object Run()
			{
				childContext.Delete(this._enclosing._handler);
				return null;
			}

			private readonly FieldMetadata _enclosing;

			private readonly DeleteContextImpl childContext;
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="Db4oIOException"></exception>
		private void RemoveIndexEntry(DeleteContextImpl context)
		{
			if (!HasIndex())
			{
				return;
			}
			int offset = context.Offset();
			object obj = ReadIndexEntry(context);
			RemoveIndexEntry(context.Transaction(), context.Id(), obj);
			context.Seek(offset);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Db4objects.Db4o.Internal.FieldMetadata))
			{
				return false;
			}
			Db4objects.Db4o.Internal.FieldMetadata other = (Db4objects.Db4o.Internal.FieldMetadata
				)obj;
			other.Alive();
			Alive();
			return other._isPrimitive == _isPrimitive && ((_handler == null && other._handler
				 == null) || other._handler.Equals(_handler)) && other._name.Equals(_name);
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

		public override string GetName()
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
			if (handler is ClassMetadata)
			{
				return (ClassMetadata)handler;
			}
			return container.ClassMetadataForReflectClass(_reflectField.GetFieldType());
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

		public override void Instantiate(UnmarshallingContext context)
		{
			if (!CheckAlive(context))
			{
				return;
			}
			object toSet = Read(context);
			InformAboutTransaction(toSet, context.Transaction());
			Set(context.PersistentObject(), toSet);
		}

		public virtual void AttemptUpdate(UnmarshallingContext context)
		{
			if (!Updating())
			{
				IncrementOffset(context);
				return;
			}
			int savedOffset = context.Offset();
			try
			{
				object toSet = context.Read(_handler);
				if (toSet != null)
				{
					Set(context.PersistentObject(), toSet);
				}
			}
			catch (Exception)
			{
				// FIXME: COR-547 Diagnostics here please.
				context.Buffer().Seek(savedOffset);
				IncrementOffset(context);
			}
		}

		private bool CheckAlive(IAspectVersionContext context)
		{
			if (!CheckEnabled(context))
			{
				return false;
			}
			bool alive = Alive();
			if (!alive)
			{
				IncrementOffset((IReadBuffer)context);
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

		public override int LinkLength()
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
			// TODO: Clean up here by creating a common interface
			//       for the Typehandlers that have a "linkLength"
			//       concept.
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
			if (_handler is IVariableLengthTypeHandler)
			{
				if (_handler is IEmbeddedTypeHandler)
				{
					return Const4.IndirectionLength;
				}
				return Const4.IdLength;
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
			if (handler == null)
			{
				_reflectField = null;
				_state = FieldMetadataState.Unavailable;
				return;
			}
			if (!handler.Equals(_handler))
			{
				// FIXME: COR-547 Diagnostics here please.
				_state = FieldMetadataState.Updating;
			}
		}

		private int AdjustUpdateDepth(object obj, int updateDepth)
		{
			int minimumUpdateDepth = 1;
			if (_containingClass.IsCollection(obj))
			{
				GenericReflector reflector = Reflector();
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

		public override void Marshall(MarshallingContext context, object obj)
		{
			// alive needs to be checked by all callers: Done
			int updateDepth = context.UpdateDepth();
			if (obj != null && CascadeOnUpdate(context.ClassConfiguration()))
			{
				context.UpdateDepth(AdjustUpdateDepth(obj, updateDepth));
			}
			if (UseDedicatedSlot(context, _handler))
			{
				context.WriteObject(_handler, obj);
			}
			else
			{
				context.CreateIndirectionWithinSlot(_handler);
				_handler.Write(context, obj);
			}
			context.UpdateDepth(updateDepth);
			if (HasIndex())
			{
				context.AddIndexEntry(this, obj);
			}
		}

		public static bool UseDedicatedSlot(IContext context, ITypeHandler4 handler)
		{
			if (handler is IEmbeddedTypeHandler)
			{
				return false;
			}
			if (handler is UntypedFieldHandler)
			{
				return false;
			}
			if (handler is ClassMetadata)
			{
				return UseDedicatedSlot(context, ((ClassMetadata)handler).DelegateTypeHandler());
			}
			return true;
		}

		public virtual bool NeedsArrayAndPrimitiveInfo()
		{
			return true;
		}

		public virtual bool NeedsHandlerId()
		{
			return true;
		}

		public virtual IPreparedComparison PrepareComparison(IContext context, object obj
			)
		{
			if (!Alive())
			{
				return null;
			}
			return _handler.PrepareComparison(context, obj);
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
				yapClassID, _handle);
		}

		public virtual object Read(IInternalReadContext context)
		{
			if (!CheckAlive((IAspectVersionContext)context))
			{
				return null;
			}
			return context.Read(_handler);
		}

		/// <summary>never called but keep for Rickie</summary>
		public virtual void RefreshActivated()
		{
			_state = FieldMetadataState.Available;
			Refresh();
		}

		internal virtual void Refresh()
		{
			ITypeHandler4 handler = DetectHandlerForField();
			if (handler != null)
			{
				handler = WrapHandlerToArrays(handler);
				if (handler.Equals(_handler))
				{
					return;
				}
			}
			_reflectField = null;
			_state = FieldMetadataState.Unavailable;
		}

		// FIXME: needs test case
		public virtual void Rename(string newName)
		{
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
				IContext context = transaction.Context();
				_index.TraverseKeys(transaction, new _IVisitor4_939(this, userVisitor, context));
			}
		}

		private sealed class _IVisitor4_939 : IVisitor4
		{
			public _IVisitor4_939(FieldMetadata _enclosing, IVisitor4 userVisitor, IContext context
				)
			{
				this._enclosing = _enclosing;
				this.userVisitor = userVisitor;
				this.context = context;
			}

			public void Visit(object obj)
			{
				FieldIndexKey key = (FieldIndexKey)obj;
				userVisitor.Visit(((IIndexableTypeHandler)this._enclosing._handler).IndexEntryToObject
					(context, key.Value()));
			}

			private readonly FieldMetadata _enclosing;

			private readonly IVisitor4 userVisitor;

			private readonly IContext context;
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
			return new BTree(systemTrans, id, new FieldIndexKeyHandler(indexHandler));
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
			ClassMetadata classMetadata = oh.ClassMetadata();
			if (classMetadata == null)
			{
				return null;
			}
			ObjectIdContextImpl context = new ObjectIdContextImpl(writer.Transaction(), writer
				, oh, writer.GetID());
			if (!classMetadata.SeekToField(context, this))
			{
				return null;
			}
			try
			{
				return ReadIndexEntry(context);
			}
			catch (CorruptionException exc)
			{
				throw new FieldIndexException(exc, this);
			}
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

		public override void DefragAspect(IDefragmentContext context)
		{
			ITypeHandler4 typeHandler = Handlers4.CorrectHandlerVersion(context, _handler);
			context.SlotFormat().DoWithSlotIndirection(context, typeHandler, new _IClosure4_1100
				(context, typeHandler));
		}

		private sealed class _IClosure4_1100 : IClosure4
		{
			public _IClosure4_1100(IDefragmentContext context, ITypeHandler4 typeHandler)
			{
				this.context = context;
				this.typeHandler = typeHandler;
			}

			public object Run()
			{
				context.Defragment(typeHandler);
				return null;
			}

			private readonly IDefragmentContext context;

			private readonly ITypeHandler4 typeHandler;
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

		public override Db4objects.Db4o.Internal.Marshall.AspectType AspectType()
		{
			return Db4objects.Db4o.Internal.Marshall.AspectType.Field;
		}

		// overriden in VirtualFieldMetadata
		public override bool CanBeDisabled()
		{
			return true;
		}
	}
}
