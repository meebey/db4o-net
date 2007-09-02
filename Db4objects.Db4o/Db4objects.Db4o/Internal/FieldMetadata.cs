/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Text;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
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
		private ClassMetadata _clazz;

		private int i_arrayPosition;

		private string i_name;

		private bool i_isArray;

		private bool i_isNArray;

		private bool i_isPrimitive;

		private IReflectField i_javaField;

		internal ITypeHandler4 i_handler;

		private int i_handlerID;

		private int i_state;

		private const int NOT_LOADED = 0;

		private const int UNAVAILABLE = -1;

		private const int AVAILABLE = 1;

		private Config4Field i_config;

		private IDb4oTypeImpl i_db4oType;

		private BTree _index;

		internal static readonly Db4objects.Db4o.Internal.FieldMetadata[] EMPTY_ARRAY = new 
			Db4objects.Db4o.Internal.FieldMetadata[0];

		public FieldMetadata(ClassMetadata a_yapClass)
		{
			_clazz = a_yapClass;
		}

		internal FieldMetadata(ClassMetadata a_yapClass, IObjectTranslator a_translator) : 
			this(a_yapClass)
		{
			Init(a_yapClass, a_translator.GetType().FullName);
			i_state = AVAILABLE;
			ObjectContainerBase stream = Container();
			i_handler = stream._handlers.HandlerForClass(stream, stream.Reflector().ForClass(
				TranslatorStoredClass(a_translator)));
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

		internal FieldMetadata(ClassMetadata containingClass, IObjectMarshaller marshaller
			) : this(containingClass)
		{
			Init(containingClass, marshaller.GetType().FullName);
			i_state = AVAILABLE;
			i_handler = Container()._handlers.UntypedHandler();
		}

		internal FieldMetadata(ClassMetadata a_yapClass, IReflectField a_field, ITypeHandler4
			 a_handler) : this(a_yapClass)
		{
			Init(a_yapClass, a_field.GetName());
			i_javaField = a_field;
			i_javaField.SetAccessible();
			i_handler = a_handler;
			bool isPrimitive = false;
			if (a_field is GenericField)
			{
				isPrimitive = ((GenericField)a_field).IsPrimitive();
			}
			Configure(a_field.GetFieldType(), isPrimitive);
			CheckDb4oType();
			i_state = AVAILABLE;
		}

		/// <param name="classMetadata"></param>
		/// <param name="oldSlot"></param>
		public virtual void AddFieldIndex(MarshallerFamily mf, ClassMetadata classMetadata
			, StatefulBuffer writer, Slot oldSlot)
		{
			if (!HasIndex())
			{
				writer.IncrementOffset(LinkLength());
				return;
			}
			try
			{
				AddIndexEntry(writer, ReadIndexEntry(mf, writer));
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
			return i_javaField.IndexEntry(indexEntry);
		}

		public virtual bool CanUseNullBitmap()
		{
			return true;
		}

		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer writer)
		{
			return ((IIndexableTypeHandler)i_handler).ReadIndexEntry(mf, writer);
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
			if (i_state == AVAILABLE)
			{
				return true;
			}
			if (i_state == NOT_LOADED)
			{
				if (i_handler == null)
				{
					i_handler = LoadJavaField1();
					if (i_handler != null)
					{
						if (i_handlerID == 0)
						{
							i_handlerID = i_handler.GetID();
						}
						else
						{
							if (i_handler.GetID() != i_handlerID)
							{
								i_handler = null;
							}
						}
					}
				}
				LoadJavaField();
				if (i_handler != null)
				{
					i_handler = WrapHandlerToArrays(Container(), i_handler);
				}
				if (i_handler == null || i_javaField == null)
				{
					i_state = UNAVAILABLE;
					i_javaField = null;
				}
				else
				{
					i_state = AVAILABLE;
					CheckDb4oType();
				}
			}
			return i_state == AVAILABLE;
		}

		internal virtual bool CanAddToQuery(string fieldName)
		{
			if (!Alive())
			{
				return false;
			}
			return fieldName.Equals(GetName()) && GetParentYapClass() != null && !GetParentYapClass
				().IsInternal();
		}

		public virtual bool CanHold(IReflectClass claxx)
		{
			if (claxx == null)
			{
				return !i_isPrimitive;
			}
			return Handlers4.HandlerCanHold(i_handler, claxx);
		}

		public virtual object Coerce(IReflectClass claxx, object obj)
		{
			if (claxx == null || obj == null)
			{
				return i_isPrimitive ? No4.INSTANCE : obj;
			}
			if (i_handler is PrimitiveHandler)
			{
				return ((PrimitiveHandler)i_handler).Coerce(claxx, obj);
			}
			if (!CanHold(claxx))
			{
				return No4.INSTANCE;
			}
			return obj;
		}

		public bool CanLoadByIndex()
		{
			if (i_handler is ClassMetadata)
			{
				ClassMetadata yc = (ClassMetadata)i_handler;
				if (yc.IsArray())
				{
					return false;
				}
			}
			return true;
		}

		internal virtual void CascadeActivation(Transaction a_trans, object a_object, int
			 a_depth, bool a_activate)
		{
			if (Alive())
			{
				try
				{
					object cascadeTo = GetOrCreate(a_trans, a_object);
					if (cascadeTo != null && i_handler != null)
					{
						i_handler.CascadeActivation(a_trans, cascadeTo, a_depth, a_activate);
					}
				}
				catch (Exception)
				{
				}
			}
		}

		private void CheckDb4oType()
		{
			if (i_javaField != null)
			{
				if (Container()._handlers.ICLASS_DB4OTYPE.IsAssignableFrom(i_javaField.GetFieldType
					()))
				{
					i_db4oType = HandlerRegistry.GetDb4oType(i_javaField.GetFieldType());
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
						if (i_isPrimitive)
						{
							if (i_handler is PrimitiveHandler)
							{
								if (obj.Equals(((PrimitiveHandler)i_handler).PrimitiveNull()))
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
							a_visitor.Visit(new QConObject(a_trans, a_parent, QField(a_trans), obj));
						}
					}
				}
			}
		}

		public TreeInt CollectIDs(MarshallerFamily mf, TreeInt tree, StatefulBuffer a_bytes
			)
		{
			if (Alive())
			{
				if (i_handler is ClassMetadata)
				{
					tree = (TreeInt)Tree.Add(tree, new TreeInt(a_bytes.ReadInt()));
				}
				else
				{
					if (i_handler is ArrayHandler)
					{
						tree = ((ArrayHandler)i_handler).CollectIDs(mf, tree, a_bytes);
					}
				}
			}
			return tree;
		}

		internal virtual void Configure(IReflectClass clazz, bool isPrimitive)
		{
			i_isArray = clazz.IsArray();
			if (i_isArray)
			{
				IReflectArray reflectArray = Container().Reflector().Array();
				i_isNArray = reflectArray.IsNDimensional(clazz);
				i_isPrimitive = reflectArray.GetComponentType(clazz).IsPrimitive();
				if (i_isNArray)
				{
					i_handler = new MultidimensionalArrayHandler(Container(), i_handler, i_isPrimitive
						);
				}
				else
				{
					i_handler = new ArrayHandler(Container(), i_handler, i_isPrimitive);
				}
			}
			else
			{
				i_isPrimitive = isPrimitive | clazz.IsPrimitive();
			}
		}

		internal virtual void Deactivate(Transaction a_trans, object a_onObject, int a_depth
			)
		{
			if (!Alive())
			{
				return;
			}
			bool isEnumClass = _clazz.IsEnum();
			if (i_isPrimitive && !i_isArray)
			{
				if (!isEnumClass)
				{
					i_javaField.Set(a_onObject, ((PrimitiveHandler)i_handler).PrimitiveNull());
				}
				return;
			}
			if (a_depth > 0)
			{
				CascadeActivation(a_trans, a_onObject, a_depth, false);
			}
			if (!isEnumClass)
			{
				i_javaField.Set(a_onObject, null);
			}
		}

		/// <param name="isUpdate"></param>
		public virtual void Delete(MarshallerFamily mf, StatefulBuffer a_bytes, bool isUpdate
			)
		{
			if (!Alive())
			{
				IncrementOffset(a_bytes);
				return;
			}
			try
			{
				RemoveIndexEntry(mf, a_bytes);
				bool dotnetValueType = false;
				dotnetValueType = Platform4.IsValueType(i_handler.ClassReflector());
				if ((i_config != null && i_config.CascadeOnDelete().DefiniteYes()) || dotnetValueType
					)
				{
					int preserveCascade = a_bytes.CascadeDeletes();
					a_bytes.SetCascadeDeletes(1);
					i_handler.DeleteEmbedded(mf, a_bytes);
					a_bytes.SetCascadeDeletes(preserveCascade);
				}
				else
				{
					if (i_config != null && i_config.CascadeOnDelete().DefiniteNo())
					{
						int preserveCascade = a_bytes.CascadeDeletes();
						a_bytes.SetCascadeDeletes(0);
						i_handler.DeleteEmbedded(mf, a_bytes);
						a_bytes.SetCascadeDeletes(preserveCascade);
					}
					else
					{
						i_handler.DeleteEmbedded(mf, a_bytes);
					}
				}
			}
			catch (CorruptionException exc)
			{
				throw new FieldIndexException(exc, this);
			}
		}

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
				return yapField.i_isPrimitive == i_isPrimitive && yapField.i_handler.Equals(i_handler
					) && yapField.i_name.Equals(i_name);
			}
			return false;
		}

		public override int GetHashCode()
		{
			return i_name.GetHashCode();
		}

		public object Get(object onObject)
		{
			return Get(null, onObject);
		}

		public object Get(Transaction trans, object onObject)
		{
			if (_clazz == null)
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
				if (trans == null)
				{
					trans = container.Transaction();
				}
				container.CheckClosed();
				ObjectReference yo = trans.ReferenceForObject(onObject);
				if (yo == null)
				{
					return null;
				}
				int id = yo.GetID();
				if (id <= 0)
				{
					return null;
				}
				StatefulBuffer writer = container.ReadWriterByID(container.Transaction(), id);
				if (writer == null)
				{
					return null;
				}
				writer._offset = 0;
				ObjectHeader oh = new ObjectHeader(container, writer);
				bool findOffset = oh.ObjectMarshaller().FindOffset(oh.ClassMetadata(), oh._headerAttributes
					, writer, this);
				if (!findOffset)
				{
					return null;
				}
				try
				{
					return Read(oh._marshallerFamily, writer);
				}
				catch (CorruptionException e)
				{
				}
			}
			return null;
		}

		public virtual string GetName()
		{
			return i_name;
		}

		public ClassMetadata GetFieldYapClass(ObjectContainerBase container)
		{
			ITypeHandler4 handler = BaseTypeHandler();
			if (Handlers4.HandlesSimple(handler))
			{
				return container._handlers.PrimitiveClassById(handler.GetID());
			}
			return (ClassMetadata)handler;
		}

		private ITypeHandler4 BaseTypeHandler()
		{
			return Handlers4.BaseTypeHandler(i_handler);
		}

		public virtual ITypeHandler4 GetHandler()
		{
			return i_handler;
		}

		public virtual int GetHandlerID()
		{
			return i_handlerID;
		}

		/// <param name="trans"></param>
		public virtual object GetOn(Transaction trans, object onObject)
		{
			if (Alive())
			{
				return i_javaField.Get(onObject);
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
			object obj = i_javaField.Get(onObject);
			if (i_db4oType != null && obj == null)
			{
				obj = i_db4oType.CreateDefault(trans);
				i_javaField.Set(onObject, obj);
			}
			return obj;
		}

		public virtual ClassMetadata GetParentYapClass()
		{
			return _clazz;
		}

		public virtual IReflectClass GetStoredType()
		{
			if (i_handler == null)
			{
				return null;
			}
			return i_handler.ClassReflector();
		}

		public virtual ObjectContainerBase Container()
		{
			if (_clazz == null)
			{
				return null;
			}
			return _clazz.Container();
		}

		public virtual bool HasConfig()
		{
			return i_config != null;
		}

		public virtual bool HasIndex()
		{
			return _index != null;
		}

		public void IncrementOffset(Db4objects.Db4o.Internal.Buffer buffer)
		{
			buffer.IncrementOffset(LinkLength());
		}

		public void Init(ClassMetadata a_yapClass, string a_name)
		{
			_clazz = a_yapClass;
			i_name = a_name;
			InitIndex(a_yapClass, a_name);
		}

		internal void InitIndex(ClassMetadata a_yapClass, string a_name)
		{
			if (a_yapClass.Config() != null)
			{
				i_config = a_yapClass.Config().ConfigField(a_name);
			}
		}

		public virtual void Init(int handlerID, bool isPrimitive, bool isArray, bool isNArray
			)
		{
			i_handlerID = handlerID;
			i_isPrimitive = isPrimitive;
			i_isArray = isArray;
			i_isNArray = isNArray;
		}

		private bool _initialized = false;

		internal void InitConfigOnUp(Transaction trans)
		{
			if (i_config != null && !_initialized)
			{
				_initialized = true;
				i_config.InitOnUp(trans, this);
			}
		}

		/// <param name="@ref"></param>
		public virtual void Instantiate(MarshallerFamily mf, ObjectReference @ref, object
			 onObject, StatefulBuffer buffer)
		{
			if (!Alive())
			{
				IncrementOffset(buffer);
				return;
			}
			object toSet = Read(mf, buffer);
			if (i_db4oType != null)
			{
				if (toSet != null)
				{
					((IDb4oTypeImpl)toSet).SetTrans(buffer.GetTransaction());
				}
			}
			Set(onObject, toSet);
		}

		public virtual bool IsArray()
		{
			return i_isArray;
		}

		public virtual int LinkLength()
		{
			Alive();
			if (i_handler == null)
			{
				return Const4.ID_LENGTH;
			}
			return i_handler.LinkLength();
		}

		public virtual void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, object obj)
		{
			Alive();
			if (i_handler == null)
			{
				header.AddBaseLength(Const4.ID_LENGTH);
				return;
			}
			i_handler.CalculateLengths(trans, header, true, obj, true);
		}

		public virtual void LoadHandler(ObjectContainerBase a_stream)
		{
			i_handler = a_stream.HandlerByID(i_handlerID);
		}

		private void LoadJavaField()
		{
			ITypeHandler4 handler = LoadJavaField1();
			if (handler == null || (!handler.Equals(i_handler)))
			{
				i_javaField = null;
				i_state = UNAVAILABLE;
			}
		}

		private ITypeHandler4 LoadJavaField1()
		{
			IReflectClass claxx = _clazz.ClassReflector();
			if (claxx == null)
			{
				return null;
			}
			i_javaField = claxx.GetDeclaredField(i_name);
			if (i_javaField == null)
			{
				return null;
			}
			i_javaField.SetAccessible();
			ObjectContainerBase container = Container();
			container.ShowInternalClasses(true);
			ITypeHandler4 handlerForClass = container._handlers.HandlerForClass(container, i_javaField
				.GetFieldType());
			container.ShowInternalClasses(false);
			return handlerForClass;
		}

		/// <param name="@ref"></param>
		/// <param name="isNew"></param>
		public virtual void Marshall(ObjectReference @ref, object obj, MarshallerFamily mf
			, StatefulBuffer writer, Config4Class config, bool isNew)
		{
			object indexEntry = null;
			if (obj != null && CascadeOnUpdate(config))
			{
				int updateDepth = writer.GetUpdateDepth();
				writer.SetUpdateDepth(AdjustUpdateDepth(obj, updateDepth));
				indexEntry = i_handler.Write(mf, obj, true, writer, true, true);
				writer.SetUpdateDepth(updateDepth);
			}
			else
			{
				indexEntry = i_handler.Write(mf, obj, true, writer, true, true);
			}
			AddIndexEntry(writer, indexEntry);
		}

		private int AdjustUpdateDepth(object obj, int updateDepth)
		{
			int minimumUpdateDepth = 1;
			if (_clazz.IsCollection(obj))
			{
				GenericReflector reflector = _clazz.Reflector();
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
				().DefiniteYes())) || (i_config != null && (i_config.CascadeOnUpdate().DefiniteYes
				())));
		}

		public virtual void Marshall(MarshallingContext context, object obj)
		{
			int updateDepth = context.UpdateDepth();
			if (obj != null && CascadeOnUpdate(context.ClassConfiguration()))
			{
				context.UpdateDepth(AdjustUpdateDepth(obj, updateDepth));
			}
			i_handler.Write(context, obj);
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

		public virtual IComparable4 PrepareComparison(object obj)
		{
			if (Alive())
			{
				i_handler.PrepareComparison(obj);
				return i_handler;
			}
			return null;
		}

		public virtual Db4objects.Db4o.Internal.Query.Processor.QField QField(Transaction
			 a_trans)
		{
			int yapClassID = 0;
			if (_clazz != null)
			{
				yapClassID = _clazz.GetID();
			}
			return new Db4objects.Db4o.Internal.Query.Processor.QField(a_trans, i_name, this, 
				yapClassID, i_arrayPosition);
		}

		internal virtual object Read(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
			if (!Alive())
			{
				IncrementOffset(a_bytes);
				return null;
			}
			return i_handler.Read(mf, a_bytes, true);
		}

		public virtual object ReadQuery(Transaction a_trans, MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 a_reader)
		{
			return i_handler.ReadQuery(a_trans, mf, true, a_reader, false);
		}

		/// <param name="trans"></param>
		/// <param name="@ref"></param>
		public virtual void ReadVirtualAttribute(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 buffer, ObjectReference @ref)
		{
			buffer.IncrementOffset(i_handler.LinkLength());
		}

		internal virtual void Refresh()
		{
			ITypeHandler4 handler = LoadJavaField1();
			if (handler != null)
			{
				handler = WrapHandlerToArrays(Container(), handler);
				if (handler.Equals(i_handler))
				{
					return;
				}
			}
			i_javaField = null;
			i_state = UNAVAILABLE;
		}

		public virtual void Rename(string newName)
		{
			ObjectContainerBase container = Container();
			if (!container.IsClient())
			{
				i_name = newName;
				_clazz.SetStateDirty();
				_clazz.Write(container.SystemTransaction());
			}
			else
			{
				Exceptions4.ThrowRuntimeException(58);
			}
		}

		public virtual void SetArrayPosition(int a_index)
		{
			i_arrayPosition = a_index;
		}

		public virtual void Set(object onObject, object obj)
		{
			if (null == i_javaField)
			{
				return;
			}
			i_javaField.Set(onObject, obj);
		}

		internal virtual void SetName(string a_name)
		{
			i_name = a_name;
		}

		internal virtual bool SupportsIndex()
		{
			return Alive() && (i_handler is IIndexable4) && (!(i_handler is UntypedFieldHandler
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
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.CLIENT_SERVER_UNSUPPORTED
					);
			}
			lock (stream.Lock())
			{
				_index.TraverseKeys(transaction, new _IVisitor4_850(this, userVisitor, transaction
					));
			}
		}

		private sealed class _IVisitor4_850 : IVisitor4
		{
			public _IVisitor4_850(FieldMetadata _enclosing, IVisitor4 userVisitor, Transaction
				 transaction)
			{
				this._enclosing = _enclosing;
				this.userVisitor = userVisitor;
				this.transaction = transaction;
			}

			public void Visit(object obj)
			{
				FieldIndexKey key = (FieldIndexKey)obj;
				userVisitor.Visit(((IIndexableTypeHandler)this._enclosing.i_handler).IndexEntryToObject
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
				Exceptions4.ThrowRuntimeException(Db4objects.Db4o.Internal.Messages.ONLY_FOR_INDEXED_FIELDS
					);
			}
		}

		private ITypeHandler4 WrapHandlerToArrays(ObjectContainerBase a_stream, ITypeHandler4
			 a_handler)
		{
			if (i_isNArray)
			{
				a_handler = new MultidimensionalArrayHandler(a_stream, a_handler, i_isPrimitive);
			}
			else
			{
				if (i_isArray)
				{
					a_handler = new ArrayHandler(a_stream, a_handler, i_isPrimitive);
				}
			}
			return a_handler;
		}

		public override string ToString()
		{
			StringBuilder sb = new StringBuilder();
			if (_clazz != null)
			{
				sb.Append(_clazz.GetName());
				sb.Append(".");
				sb.Append(GetName());
			}
			return sb.ToString();
		}

		public string ToString(MarshallerFamily mf, StatefulBuffer writer)
		{
			string str = "\n Field " + i_name;
			if (!Alive())
			{
				IncrementOffset(writer);
			}
			else
			{
				object obj = null;
				try
				{
					obj = Read(mf, writer);
				}
				catch (Exception)
				{
				}
				if (obj == null)
				{
					str += "\n [null]";
				}
				else
				{
					str += "\n  " + obj.ToString();
				}
			}
			return str;
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
			IReflectClass indexType = null;
			if (i_javaField != null)
			{
				indexType = i_javaField.IndexType();
			}
			ITypeHandler4 classHandler = stream._handlers.HandlerForClass(stream, indexType);
			if (!(classHandler is IIndexable4))
			{
				return null;
			}
			IIndexable4 indexHandler = (IIndexable4)classHandler;
			return indexHandler;
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
			return i_isPrimitive;
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
			if (i_handler is ClassMetadata)
			{
				value = ((ClassMetadata)i_handler).WrapWithTransactionContext(transaction, value);
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
				, keyPart), SearchTarget.LOWEST);
		}

		public virtual bool RebuildIndexForClass(LocalObjectContainer stream, ClassMetadata
			 yapClass)
		{
			long[] ids = yapClass.GetIDs();
			for (int i = 0; i < ids.Length; i++)
			{
				RebuildIndexForObject(stream, yapClass, (int)ids[i]);
			}
			return ids.Length > 0;
		}

		/// <param name="classMetadata"></param>
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
			if (stream.ConfigImpl().MessageLevel() > Const4.NONE)
			{
				stream.Message("dropping index " + ToString());
			}
			_index.Free(systemTrans);
			stream.SetDirtyInSystemTransaction(GetParentYapClass());
			_index = null;
		}

		public virtual void DefragField(MarshallerFamily mf, BufferPair readers)
		{
			GetHandler().Defrag(mf, readers, true);
		}

		public virtual void CreateIndex()
		{
			if (HasIndex())
			{
				return;
			}
			LocalObjectContainer container = (LocalObjectContainer)Container();
			if (container.ConfigImpl().MessageLevel() > Const4.NONE)
			{
				container.Message("creating index " + ToString());
			}
			InitIndex(container.SystemTransaction());
			container.SetDirtyInSystemTransaction(GetParentYapClass());
			Reindex(container);
		}

		private void Reindex(LocalObjectContainer container)
		{
			ClassMetadata clazz = GetParentYapClass();
			if (RebuildIndexForClass(container, clazz))
			{
				container.SystemTransaction().Commit();
			}
		}
	}
}
