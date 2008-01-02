/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ClassMetadata : PersistentBase, IIndexableTypeHandler, IFirstClassHandler
		, IStoredClass
	{
		public Db4objects.Db4o.Internal.ClassMetadata i_ancestor;

		private Config4Class i_config;

		public int _metaClassID;

		public FieldMetadata[] i_fields;

		private readonly IClassIndexStrategy _index;

		protected string i_name;

		private readonly ObjectContainerBase _container;

		internal byte[] i_nameBytes;

		private BufferImpl i_reader;

		private bool _classIndexed;

		private IReflectClass _reflector;

		private bool _isEnum;

		private EventDispatcher _eventDispatcher;

		private bool _internal;

		private bool _unversioned;

		private int i_lastID;

		private TernaryBool _canUpdateFast = TernaryBool.UNSPECIFIED;

		public ObjectContainerBase Stream()
		{
			return _container;
		}

		public bool CanUpdateFast()
		{
			return _canUpdateFast.BooleanValue(CheckCanUpdateFast());
		}

		private bool CheckCanUpdateFast()
		{
			if (i_ancestor != null && !i_ancestor.CanUpdateFast())
			{
				return false;
			}
			if (i_config != null && i_config.CascadeOnDelete() == TernaryBool.YES)
			{
				return false;
			}
			for (int i = 0; i < i_fields.Length; ++i)
			{
				if (i_fields[i].HasIndex())
				{
					return false;
				}
			}
			return true;
		}

		internal virtual bool IsInternal()
		{
			return _internal;
		}

		private IClassIndexStrategy CreateIndexStrategy()
		{
			return new BTreeClassIndexStrategy(this);
		}

		public ClassMetadata(ObjectContainerBase container, IReflectClass reflector)
		{
			_container = container;
			_reflector = reflector;
			_index = CreateIndexStrategy();
			_classIndexed = true;
		}

		internal virtual void ActivateFields(Transaction trans, object obj, IActivationDepth
			 depth)
		{
			if (ObjectCanActivate(trans, obj))
			{
				ActivateFieldsLoop(trans, obj, depth);
			}
		}

		private void ActivateFieldsLoop(Transaction trans, object obj, IActivationDepth depth
			)
		{
			for (int i = 0; i < i_fields.Length; i++)
			{
				i_fields[i].CascadeActivation(trans, obj, depth);
			}
			if (i_ancestor != null)
			{
				i_ancestor.ActivateFieldsLoop(trans, obj, depth);
			}
		}

		public void AddFieldIndices(StatefulBuffer a_writer, Slot oldSlot)
		{
			if (HasClassIndex() || HasVirtualAttributes())
			{
				ObjectHeader oh = new ObjectHeader(_container, this, a_writer);
				oh._marshallerFamily._object.AddFieldIndices(this, oh._headerAttributes, a_writer
					, oldSlot);
			}
		}

		internal virtual void AddMembers(ObjectContainerBase container)
		{
			BitTrue(Const4.CHECKED_CHANGES);
			if (InstallTranslator(container))
			{
				return;
			}
			if (container.DetectSchemaChanges())
			{
				bool dirty = IsDirty();
				Collection4 members = new Collection4();
				if (null != i_fields)
				{
					members.AddAll(i_fields);
					if (i_fields.Length == 1 && i_fields[0] is TranslatedFieldMetadata)
					{
						SetStateOK();
						return;
					}
				}
				if (GenerateVersionNumbers())
				{
					if (!HasVersionField())
					{
						members.Add(container.VersionIndex());
						dirty = true;
					}
				}
				if (GenerateUUIDs())
				{
					if (!HasUUIDField())
					{
						members.Add(container.UUIDIndex());
						dirty = true;
					}
				}
				dirty = CollectReflectFields(container, members) | dirty;
				if (dirty)
				{
					_container.SetDirtyInSystemTransaction(this);
					i_fields = new FieldMetadata[members.Size()];
					members.ToArray(i_fields);
					for (int i = 0; i < i_fields.Length; i++)
					{
						i_fields[i].SetArrayPosition(i);
					}
				}
				else
				{
					if (members.Size() == 0)
					{
						i_fields = new FieldMetadata[0];
					}
				}
				DiagnosticProcessor dp = _container._handlers._diagnosticProcessor;
				if (dp.Enabled())
				{
					dp.CheckClassHasFields(this);
				}
			}
			else
			{
				if (i_fields == null)
				{
					i_fields = new FieldMetadata[0];
				}
			}
			_container.Callbacks().ClassOnRegistered(this);
			SetStateOK();
		}

		private bool CollectReflectFields(ObjectContainerBase container, Collection4 collectedFields
			)
		{
			bool dirty = false;
			IReflectField[] fields = ReflectFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (StoreField(fields[i]))
				{
					Db4objects.Db4o.Internal.ClassMetadata fieldClassMetadata = container._handlers.ClassMetadataForClass
						(container, fields[i].GetFieldType());
					if (fieldClassMetadata == null)
					{
						continue;
					}
					ITypeHandler4 handler = fieldClassMetadata.TypeHandler();
					FieldMetadata field = new FieldMetadata(this, fields[i], handler, fieldClassMetadata
						.GetID());
					bool found = false;
					IEnumerator m = collectedFields.GetEnumerator();
					while (m.MoveNext())
					{
						if (((FieldMetadata)m.Current).Equals(field))
						{
							found = true;
							break;
						}
					}
					if (found)
					{
						continue;
					}
					dirty = true;
					collectedFields.Add(field);
				}
			}
			return dirty;
		}

		private bool InstallTranslator(ObjectContainerBase ocb)
		{
			IObjectTranslator ot = GetTranslator();
			if (ot == null)
			{
				return false;
			}
			if (IsNewTranslator(ot))
			{
				_container.SetDirtyInSystemTransaction(this);
			}
			InstallCustomFieldMetadata(ocb, new TranslatedFieldMetadata(this, ot));
			return true;
		}

		private void InstallCustomFieldMetadata(ObjectContainerBase ocb, FieldMetadata customFieldMetadata
			)
		{
			int fieldCount = 1;
			bool versions = GenerateVersionNumbers() && !AncestorHasVersionField();
			bool uuids = GenerateUUIDs() && !AncestorHasUUIDField();
			if (versions)
			{
				fieldCount = 2;
			}
			if (uuids)
			{
				fieldCount = 3;
			}
			i_fields = new FieldMetadata[fieldCount];
			i_fields[0] = customFieldMetadata;
			if (versions || uuids)
			{
				i_fields[1] = ocb.VersionIndex();
			}
			if (uuids)
			{
				i_fields[2] = ocb.UUIDIndex();
			}
			SetStateOK();
		}

		private IObjectTranslator GetTranslator()
		{
			return i_config == null ? null : i_config.GetTranslator();
		}

		private bool IsNewTranslator(IObjectTranslator ot)
		{
			return !HasFields() || !ot.GetType().FullName.Equals(i_fields[0].GetName());
		}

		private bool HasFields()
		{
			return i_fields != null && i_fields.Length > 0;
		}

		internal virtual void AddToIndex(LocalObjectContainer a_stream, Transaction a_trans
			, int a_id)
		{
			if (a_stream.MaintainsIndices())
			{
				AddToIndex1(a_stream, a_trans, a_id);
			}
		}

		internal virtual void AddToIndex1(LocalObjectContainer a_stream, Transaction a_trans
			, int a_id)
		{
			if (i_ancestor != null)
			{
				i_ancestor.AddToIndex1(a_stream, a_trans, a_id);
			}
			if (HasClassIndex())
			{
				_index.Add(a_trans, a_id);
			}
		}

		internal virtual bool AllowsQueries()
		{
			return HasClassIndex();
		}

		public virtual void CascadeActivation(Transaction trans, object onObject, IActivationDepth
			 depth)
		{
			if (DescendOnCascadingActivation())
			{
				depth = depth.Descend(this);
			}
			if (!depth.RequiresActivation())
			{
				return;
			}
			ObjectContainerBase stream = trans.Container();
			if (depth.Mode().IsDeactivate())
			{
				stream.StillToDeactivate(trans, onObject, depth, false);
			}
			else
			{
				if (IsValueType())
				{
					ActivateFields(trans, onObject, depth);
				}
				else
				{
					stream.StillToActivate(trans, onObject, depth);
				}
			}
		}

		protected virtual bool DescendOnCascadingActivation()
		{
			return true;
		}

		internal virtual void CheckChanges()
		{
			if (StateOK())
			{
				if (!BitIsTrue(Const4.CHECKED_CHANGES))
				{
					BitTrue(Const4.CHECKED_CHANGES);
					if (i_ancestor != null)
					{
						i_ancestor.CheckChanges();
					}
					if (_reflector != null)
					{
						AddMembers(_container);
						if (!_container.IsClient())
						{
							Write(_container.SystemTransaction());
						}
					}
				}
			}
		}

		public virtual void CheckType()
		{
			IReflectClass claxx = ClassReflector();
			if (claxx == null)
			{
				return;
			}
			if (_container._handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx))
			{
				_internal = true;
			}
			if (_container._handlers.ICLASS_UNVERSIONED.IsAssignableFrom(claxx))
			{
				_unversioned = true;
			}
			if (IsDb4oTypeImpl())
			{
				IDb4oTypeImpl db4oTypeImpl = (IDb4oTypeImpl)claxx.NewInstance();
				_classIndexed = (db4oTypeImpl == null || db4oTypeImpl.HasClassIndex());
			}
			else
			{
				if (i_config != null)
				{
					_classIndexed = i_config.Indexed();
				}
			}
		}

		public virtual bool IsDb4oTypeImpl()
		{
			return _container._handlers.ICLASS_DB4OTYPEIMPL.IsAssignableFrom(ClassReflector()
				);
		}

		public int AdjustUpdateDepth(Transaction trans, int depth)
		{
			Config4Class config = ConfigOrAncestorConfig();
			if (depth == Const4.UNSPECIFIED)
			{
				depth = CheckUpdateDepthUnspecified(trans.Container().ConfigImpl());
				if (ClassReflector().IsCollection())
				{
					depth = AdjustDepthToBorders(depth);
				}
			}
			if (config == null)
			{
				return depth - 1;
			}
			bool cascadeOnDelete = config.CascadeOnDelete() == TernaryBool.YES;
			bool cascadeOnUpdate = config.CascadeOnUpdate() == TernaryBool.YES;
			if (cascadeOnDelete || cascadeOnUpdate)
			{
				depth = AdjustDepthToBorders(depth);
			}
			return depth - 1;
		}

		private int AdjustDepthToBorders(int depth)
		{
			int depthBorder = Reflector().CollectionUpdateDepth(ClassReflector());
			if (depth > int.MinValue && depth < depthBorder)
			{
				depth = depthBorder;
			}
			return depth;
		}

		private int CheckUpdateDepthUnspecified(Config4Impl config)
		{
			int depth = config.UpdateDepth() + 1;
			if (i_config != null && i_config.UpdateDepth() != 0)
			{
				depth = i_config.UpdateDepth() + 1;
			}
			if (i_ancestor != null)
			{
				int ancestordepth = i_ancestor.CheckUpdateDepthUnspecified(config);
				if (ancestordepth > depth)
				{
					return ancestordepth;
				}
			}
			return depth;
		}

		public virtual void CollectConstraints(Transaction a_trans, QConObject a_parent, 
			object a_object, IVisitor4 a_visitor)
		{
			if (i_fields != null)
			{
				for (int i = 0; i < i_fields.Length; i++)
				{
					i_fields[i].CollectConstraints(a_trans, a_parent, a_object, a_visitor);
				}
			}
			if (i_ancestor != null)
			{
				i_ancestor.CollectConstraints(a_trans, a_parent, a_object, a_visitor);
			}
		}

		public TreeInt CollectFieldIDs(MarshallerFamily mf, ObjectHeaderAttributes attributes
			, TreeInt tree, StatefulBuffer a_bytes, string name)
		{
			return mf._object.CollectFieldIDs(tree, this, attributes, a_bytes, name);
		}

		public virtual bool CustomizedNewInstance()
		{
			return ConfigInstantiates();
		}

		public virtual Config4Class Config()
		{
			return i_config;
		}

		public virtual Config4Class ConfigOrAncestorConfig()
		{
			if (i_config != null)
			{
				return i_config;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.ConfigOrAncestorConfig();
			}
			return null;
		}

		private bool CreateConstructor(ObjectContainerBase container, string className)
		{
			IReflectClass claxx = container.Reflector().ForName(className);
			return CreateConstructor(container, claxx, className, true);
		}

		public virtual bool CreateConstructor(ObjectContainerBase a_stream, IReflectClass
			 a_class, string a_name, bool errMessages)
		{
			_reflector = a_class;
			_eventDispatcher = EventDispatcher.ForClass(a_stream, a_class);
			if (CustomizedNewInstance())
			{
				return true;
			}
			if (a_class != null)
			{
				if (a_stream._handlers.ICLASS_TRANSIENTCLASS.IsAssignableFrom(a_class) || Platform4
					.IsTransient(a_class))
				{
					a_class = null;
				}
			}
			if (a_class == null)
			{
				if (a_name == null || !Platform4.IsDb4oClass(a_name))
				{
					if (errMessages)
					{
						a_stream.LogMsg(23, a_name);
					}
				}
				SetStateDead();
				return false;
			}
			if (a_stream._handlers.CreateConstructor(a_class, !CallConstructor()))
			{
				return true;
			}
			SetStateDead();
			if (errMessages)
			{
				a_stream.LogMsg(7, a_name);
			}
			if (a_stream.ConfigImpl().ExceptionsOnNotStorable())
			{
				throw new ObjectNotStorableException(a_class);
			}
			return false;
		}

		public virtual void Deactivate(Transaction trans, object obj, IActivationDepth depth
			)
		{
			if (ObjectCanDeactivate(trans, obj))
			{
				DeactivateFields(trans, obj, depth);
				ObjectOnDeactivate(trans, obj);
			}
		}

		private void ObjectOnDeactivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnDeactivate(transaction, obj);
			DispatchEvent(container, obj, EventDispatcher.DEACTIVATE);
		}

		private bool ObjectCanDeactivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			return container.Callbacks().ObjectCanDeactivate(transaction, obj) && DispatchEvent
				(container, obj, EventDispatcher.CAN_DEACTIVATE);
		}

		internal virtual void DeactivateFields(Transaction a_trans, object a_object, IActivationDepth
			 a_depth)
		{
			for (int i = 0; i < i_fields.Length; i++)
			{
				i_fields[i].Deactivate(a_trans, a_object, a_depth);
			}
			if (i_ancestor != null)
			{
				i_ancestor.DeactivateFields(a_trans, a_object, a_depth);
			}
		}

		internal void Delete(StatefulBuffer a_bytes, object a_object)
		{
			ObjectHeader oh = new ObjectHeader(_container, this, a_bytes);
			Delete1(oh._marshallerFamily, oh._headerAttributes, a_bytes, a_object);
		}

		private void Delete1(MarshallerFamily mf, ObjectHeaderAttributes attributes, StatefulBuffer
			 a_bytes, object a_object)
		{
			RemoveFromIndex(a_bytes.GetTransaction(), a_bytes.GetID());
			DeleteMembers(mf, attributes, a_bytes, a_bytes.GetTransaction().Container()._handlers
				.ArrayType(a_object), false);
		}

		/// <exception cref="Db4oIOException"></exception>
		public virtual void Delete(IDeleteContext context)
		{
			((ObjectContainerBase)context.ObjectContainer()).DeleteByID(context.Transaction()
				, context.ReadInt(), context.CascadeDeleteDepth());
		}

		internal virtual void DeleteMembers(MarshallerFamily mf, ObjectHeaderAttributes attributes
			, StatefulBuffer a_bytes, int a_type, bool isUpdate)
		{
			try
			{
				if (CascadeOnDelete())
				{
					int preserveCascade = a_bytes.CascadeDeletes();
					if (ClassReflector().IsCollection())
					{
						int newCascade = preserveCascade + Reflector().CollectionUpdateDepth(ClassReflector
							()) - 3;
						if (newCascade < 1)
						{
							newCascade = 1;
						}
						a_bytes.SetCascadeDeletes(newCascade);
					}
					else
					{
						a_bytes.SetCascadeDeletes(1);
					}
					mf._object.DeleteMembers(this, attributes, a_bytes, a_type, isUpdate);
					a_bytes.SetCascadeDeletes(preserveCascade);
				}
				else
				{
					mf._object.DeleteMembers(this, attributes, a_bytes, a_type, isUpdate);
				}
			}
			catch (Exception e)
			{
				DiagnosticProcessor dp = Container()._handlers._diagnosticProcessor;
				if (dp.Enabled())
				{
					dp.DeletionFailed();
				}
			}
		}

		public virtual TernaryBool CascadeOnDeleteTernary()
		{
			Config4Class config = Config();
			TernaryBool cascadeOnDelete = TernaryBool.UNSPECIFIED;
			if (config != null && (cascadeOnDelete = config.CascadeOnDelete()) != TernaryBool
				.UNSPECIFIED)
			{
				return cascadeOnDelete;
			}
			if (i_ancestor == null)
			{
				return cascadeOnDelete;
			}
			return i_ancestor.CascadeOnDeleteTernary();
		}

		public virtual bool CascadeOnDelete()
		{
			return CascadeOnDeleteTernary() == TernaryBool.YES;
		}

		public bool DispatchEvent(ObjectContainerBase stream, object obj, int message)
		{
			if (!DispatchingEvents(stream))
			{
				return true;
			}
			return _eventDispatcher.Dispatch(stream, obj, message);
		}

		private bool DispatchingEvents(ObjectContainerBase stream)
		{
			return _eventDispatcher != null && stream.DispatchsEvents();
		}

		public virtual bool HasEventRegistered(ObjectContainerBase stream, int eventID)
		{
			if (!DispatchingEvents(stream))
			{
				return true;
			}
			return _eventDispatcher.HasEventRegistered(eventID);
		}

		public int FieldCount()
		{
			int count = i_fields.Length;
			if (i_ancestor != null)
			{
				count += i_ancestor.FieldCount();
			}
			return count;
		}

		private class FieldMetadataIterator : IEnumerator
		{
			private readonly ClassMetadata _initialClazz;

			private ClassMetadata _curClazz;

			private int _curIdx;

			public FieldMetadataIterator(ClassMetadata clazz)
			{
				_initialClazz = clazz;
				Reset();
			}

			public virtual object Current
			{
				get
				{
					return _curClazz.i_fields[_curIdx];
				}
			}

			public virtual bool MoveNext()
			{
				if (_curClazz == null)
				{
					_curClazz = _initialClazz;
					_curIdx = 0;
				}
				else
				{
					_curIdx++;
				}
				while (_curClazz != null && !IndexInRange())
				{
					_curClazz = _curClazz.i_ancestor;
					_curIdx = 0;
				}
				return _curClazz != null && IndexInRange();
			}

			public virtual void Reset()
			{
				_curClazz = null;
				_curIdx = -1;
			}

			private bool IndexInRange()
			{
				return _curIdx < _curClazz.i_fields.Length;
			}
		}

		public virtual IEnumerator Fields()
		{
			return new ClassMetadata.FieldMetadataIterator(this);
		}

		public HandlerVersion FindOffset(BufferImpl buffer, FieldMetadata field)
		{
			if (buffer == null)
			{
				return HandlerVersion.INVALID;
			}
			buffer._offset = 0;
			ObjectHeader oh = new ObjectHeader(_container, this, buffer);
			bool res = oh.ObjectMarshaller().FindOffset(this, oh._headerAttributes, buffer, field
				);
			if (!res)
			{
				return HandlerVersion.INVALID;
			}
			return new HandlerVersion(oh.HandlerVersion());
		}

		internal virtual void ForEachFieldMetadata(IVisitor4 visitor)
		{
			if (i_fields != null)
			{
				for (int i = 0; i < i_fields.Length; i++)
				{
					visitor.Visit(i_fields[i]);
				}
			}
			if (i_ancestor != null)
			{
				i_ancestor.ForEachFieldMetadata(visitor);
			}
		}

		public static ClassMetadata ForObject(Transaction trans, object obj, bool allowCreation
			)
		{
			IReflectClass reflectClass = trans.Reflector().ForObject(obj);
			if (reflectClass != null && reflectClass.GetSuperclass() == null && obj != null)
			{
				throw new ObjectNotStorableException(obj.ToString());
			}
			if (allowCreation)
			{
				return trans.Container().ProduceClassMetadata(reflectClass);
			}
			return trans.Container().ClassMetadataForReflectClass(reflectClass);
		}

		public virtual bool GenerateUUIDs()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateUUIDs();
			return Generate1(_container.Config().GenerateUUIDs(), configValue);
		}

		private bool GenerateVersionNumbers()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateVersionNumbers();
			return Generate1(_container.Config().GenerateVersionNumbers(), configValue);
		}

		private bool GenerateVirtual()
		{
			if (_unversioned)
			{
				return false;
			}
			if (_internal)
			{
				return false;
			}
			return true;
		}

		private bool Generate1(ConfigScope globalConfig, bool individualConfig)
		{
			return globalConfig.ApplyConfig(individualConfig);
		}

		internal virtual ClassMetadata GetAncestor()
		{
			return i_ancestor;
		}

		public virtual object GetComparableObject(object forObject)
		{
			if (i_config != null)
			{
				if (i_config.QueryAttributeProvider() != null)
				{
					return i_config.QueryAttributeProvider().Attribute(forObject);
				}
			}
			return forObject;
		}

		public virtual ClassMetadata GetHigherHierarchy(ClassMetadata a_yapClass)
		{
			ClassMetadata yc = GetHigherHierarchy1(a_yapClass);
			if (yc != null)
			{
				return yc;
			}
			return a_yapClass.GetHigherHierarchy1(this);
		}

		private ClassMetadata GetHigherHierarchy1(ClassMetadata a_yapClass)
		{
			if (a_yapClass == this)
			{
				return this;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.GetHigherHierarchy1(a_yapClass);
			}
			return null;
		}

		public virtual ClassMetadata GetHigherOrCommonHierarchy(ClassMetadata a_yapClass)
		{
			ClassMetadata yc = GetHigherHierarchy1(a_yapClass);
			if (yc != null)
			{
				return yc;
			}
			if (i_ancestor != null)
			{
				yc = i_ancestor.GetHigherOrCommonHierarchy(a_yapClass);
				if (yc != null)
				{
					return yc;
				}
			}
			return a_yapClass.GetHigherHierarchy1(this);
		}

		public override byte GetIdentifier()
		{
			return Const4.YAPCLASS;
		}

		public virtual long[] GetIDs()
		{
			lock (Lock())
			{
				if (!StateOK())
				{
					return new long[0];
				}
				return GetIDs(_container.Transaction());
			}
		}

		public virtual long[] GetIDs(Transaction trans)
		{
			lock (Lock())
			{
				if (!StateOK())
				{
					return new long[0];
				}
				if (!HasClassIndex())
				{
					return new long[0];
				}
				return trans.Container().GetIDsForClass(trans, this);
			}
		}

		public virtual bool HasClassIndex()
		{
			return _classIndexed;
		}

		private bool AncestorHasUUIDField()
		{
			if (i_ancestor == null)
			{
				return false;
			}
			return i_ancestor.HasUUIDField();
		}

		private bool HasUUIDField()
		{
			if (AncestorHasUUIDField())
			{
				return true;
			}
			return Arrays4.ContainsInstanceOf(i_fields, typeof(UUIDFieldMetadata));
		}

		private bool AncestorHasVersionField()
		{
			if (i_ancestor == null)
			{
				return false;
			}
			return i_ancestor.HasVersionField();
		}

		private bool HasVersionField()
		{
			if (AncestorHasVersionField())
			{
				return true;
			}
			return Arrays4.ContainsInstanceOf(i_fields, typeof(VersionFieldMetadata));
		}

		public virtual IClassIndexStrategy Index()
		{
			return _index;
		}

		public virtual int IndexEntryCount(Transaction ta)
		{
			if (!StateOK())
			{
				return 0;
			}
			return _index.EntryCount(ta);
		}

		public virtual object IndexEntryToObject(Transaction trans, object indexEntry)
		{
			if (indexEntry == null)
			{
				return null;
			}
			int id = ((int)indexEntry);
			return Container().GetByID2(trans, id);
		}

		public virtual IReflectClass ClassReflector()
		{
			return _reflector;
		}

		public virtual string GetName()
		{
			if (i_name == null)
			{
				if (_reflector != null)
				{
					i_name = _reflector.GetName();
				}
			}
			return i_name;
		}

		public virtual IStoredClass GetParentStoredClass()
		{
			return GetAncestor();
		}

		public virtual IStoredField[] GetStoredFields()
		{
			lock (Lock())
			{
				if (i_fields == null)
				{
					return new IStoredField[0];
				}
				IStoredField[] fields = new IStoredField[i_fields.Length];
				System.Array.Copy(i_fields, 0, fields, 0, i_fields.Length);
				return fields;
			}
		}

		internal ObjectContainerBase Container()
		{
			return _container;
		}

		public virtual FieldMetadata FieldMetadataForName(string name)
		{
			FieldMetadata[] yf = new FieldMetadata[1];
			ForEachFieldMetadata(new _IVisitor4_905(this, name, yf));
			return yf[0];
		}

		private sealed class _IVisitor4_905 : IVisitor4
		{
			public _IVisitor4_905(ClassMetadata _enclosing, string name, FieldMetadata[] yf)
			{
				this._enclosing = _enclosing;
				this.name = name;
				this.yf = yf;
			}

			public void Visit(object obj)
			{
				if (name.Equals(((FieldMetadata)obj).GetName()))
				{
					yf[0] = (FieldMetadata)obj;
				}
			}

			private readonly ClassMetadata _enclosing;

			private readonly string name;

			private readonly FieldMetadata[] yf;
		}

		/// <param name="container"></param>
		public virtual bool HasField(ObjectContainerBase container, string fieldName)
		{
			if (ClassReflector().IsCollection())
			{
				return true;
			}
			return FieldMetadataForName(fieldName) != null;
		}

		internal virtual bool HasVirtualAttributes()
		{
			if (_internal)
			{
				return false;
			}
			return HasVersionField() || HasUUIDField();
		}

		public virtual bool HoldsAnyClass()
		{
			return ClassReflector().IsCollection();
		}

		internal virtual void IncrementFieldsOffset1(BufferImpl a_bytes)
		{
			int length = ReadFieldCount(a_bytes);
			for (int i = 0; i < length; i++)
			{
				i_fields[i].IncrementOffset(a_bytes);
			}
		}

		internal bool Init(ObjectContainerBase a_stream, ClassMetadata a_ancestor, IReflectClass
			 claxx)
		{
			if (DTrace.enabled)
			{
				DTrace.YAPCLASS_INIT.Log(GetID());
			}
			SetAncestor(a_ancestor);
			Config4Impl config = a_stream.ConfigImpl();
			string className = claxx.GetName();
			SetConfig(config.ConfigClass(className));
			if (!CreateConstructor(a_stream, claxx, className, false))
			{
				return false;
			}
			CheckType();
			if (AllowsQueries())
			{
				_index.Initialize(a_stream);
			}
			i_name = className;
			i_ancestor = a_ancestor;
			BitTrue(Const4.CHECKED_CHANGES);
			return true;
		}

		internal void InitConfigOnUp(Transaction systemTrans)
		{
			Config4Class extendedConfig = Platform4.ExtendConfiguration(_reflector, _container
				.Configure(), i_config);
			if (extendedConfig != null)
			{
				i_config = extendedConfig;
			}
			if (i_config == null)
			{
				return;
			}
			if (!StateOK())
			{
				return;
			}
			if (i_fields == null)
			{
				return;
			}
			for (int i = 0; i < i_fields.Length; i++)
			{
				FieldMetadata curField = i_fields[i];
				string fieldName = curField.GetName();
				if (!curField.HasConfig() && extendedConfig != null && extendedConfig.ConfigField
					(fieldName) != null)
				{
					curField.InitIndex(this, fieldName);
				}
				curField.InitConfigOnUp(systemTrans);
			}
		}

		internal virtual void InitOnUp(Transaction systemTrans)
		{
			if (!StateOK())
			{
				return;
			}
			InitConfigOnUp(systemTrans);
			StoreStaticFieldValues(systemTrans, false);
		}

		public virtual object Instantiate(UnmarshallingContext context)
		{
			object obj = context.PersistentObject();
			bool instantiating = (obj == null);
			if (instantiating)
			{
				obj = InstantiateObject(context);
				if (obj == null)
				{
					return null;
				}
				ShareTransaction(obj, context.Transaction());
				ShareObjectReference(obj, context.Reference());
				context.SetObjectWeak(obj);
				context.Transaction().ReferenceSystem().AddExistingReferenceToObjectTree(context.
					Reference());
				ObjectOnInstantiate(context.Transaction(), obj);
			}
			context.AddToIDTree();
			if (instantiating)
			{
				if (!context.ActivationDepth().RequiresActivation())
				{
					context.Reference().SetStateDeactivated();
				}
				else
				{
					Activate(context);
				}
			}
			else
			{
				if (ActivatingActiveObject(context.ActivationDepth().Mode(), context.Reference())
					)
				{
					IActivationDepth child = context.ActivationDepth().Descend(this);
					if (child.RequiresActivation())
					{
						ActivateFields(context.Transaction(), obj, child);
					}
				}
				else
				{
					Activate(context);
				}
			}
			return obj;
		}

		public virtual object InstantiateTransient(UnmarshallingContext context)
		{
			object obj = InstantiateObject(context);
			if (obj == null)
			{
				return null;
			}
			context.Container().Peeked(context.ObjectID(), obj);
			if (context.ActivationDepth().RequiresActivation())
			{
				InstantiateFields(context);
			}
			return obj;
		}

		private bool ActivatingActiveObject(ActivationMode mode, ObjectReference @ref)
		{
			return !mode.IsRefresh() && @ref.IsActive();
		}

		private void Activate(UnmarshallingContext context)
		{
			if (!ObjectCanActivate(context.Transaction(), context.PersistentObject()))
			{
				context.Reference().SetStateDeactivated();
				return;
			}
			context.Reference().SetStateClean();
			if (context.ActivationDepth().RequiresActivation())
			{
				InstantiateFields(context);
			}
			ObjectOnActivate(context.Transaction(), context.PersistentObject());
		}

		private bool ConfigInstantiates()
		{
			return Config() != null && Config().Instantiates();
		}

		private object InstantiateObject(UnmarshallingContext context)
		{
			object obj = ConfigInstantiates() ? InstantiateFromConfig(context) : InstantiateFromReflector
				(context.Container());
			context.PersistentObject(obj);
			return obj;
		}

		private void ObjectOnInstantiate(Transaction transaction, object instance)
		{
			transaction.Container().Callbacks().ObjectOnInstantiate(transaction, instance);
		}

		internal virtual object InstantiateFromReflector(ObjectContainerBase stream)
		{
			if (_reflector == null)
			{
				return null;
			}
			stream.Instantiating(true);
			try
			{
				return _reflector.NewInstance();
			}
			catch (MissingMethodException)
			{
				stream.LogMsg(7, ClassReflector().GetName());
				return null;
			}
			catch (Exception)
			{
				return null;
			}
			finally
			{
				stream.Instantiating(false);
			}
		}

		private object InstantiateFromConfig(UnmarshallingContext context)
		{
			int offset = context.Offset();
			context.Seek(offset + Const4.INT_LENGTH);
			try
			{
				return i_config.Instantiate(context.Container(), i_fields[0].Read(context));
			}
			finally
			{
				context.Seek(offset);
			}
		}

		private void ShareObjectReference(object obj, ObjectReference @ref)
		{
			if (obj is IDb4oTypeImpl)
			{
				((IDb4oTypeImpl)obj).SetObjectReference(@ref);
			}
		}

		private void ShareTransaction(object obj, Transaction transaction)
		{
			if (obj is ITransactionAware)
			{
				((ITransactionAware)obj).SetTrans(transaction);
			}
		}

		private void ObjectOnActivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			container.Callbacks().ObjectOnActivate(transaction, obj);
			DispatchEvent(container, obj, EventDispatcher.ACTIVATE);
		}

		private bool ObjectCanActivate(Transaction transaction, object obj)
		{
			ObjectContainerBase container = transaction.Container();
			return container.Callbacks().ObjectCanActivate(transaction, obj) && DispatchEvent
				(container, obj, EventDispatcher.CAN_ACTIVATE);
		}

		internal virtual void InstantiateFields(UnmarshallingContext context)
		{
			MarshallerFamily.Version(context.HandlerVersion())._object.InstantiateFields(context
				);
		}

		public virtual bool IsArray()
		{
			return ClassReflector().IsCollection();
		}

		internal virtual bool IsCollection(object obj)
		{
			return Reflector().ForObject(obj).IsCollection();
		}

		public override bool IsDirty()
		{
			if (!StateOK())
			{
				return false;
			}
			return base.IsDirty();
		}

		internal virtual bool IsEnum()
		{
			return _isEnum;
		}

		public virtual bool IsPrimitive()
		{
			return false;
		}

		/// <summary>no any, primitive, array or other tricks.</summary>
		/// <remarks>
		/// no any, primitive, array or other tricks. overriden in YapClassAny and
		/// YapClassPrimitive
		/// </remarks>
		public virtual bool IsStrongTyped()
		{
			return true;
		}

		public virtual bool IsValueType()
		{
			return Platform4.IsValueType(ClassReflector());
		}

		private object Lock()
		{
			return _container.Lock();
		}

		public virtual string NameToWrite()
		{
			if (i_config != null && i_config.WriteAs() != null)
			{
				return i_config.WriteAs();
			}
			if (i_name == null)
			{
				return string.Empty;
			}
			return _container.ConfigImpl().ResolveAliasRuntimeName(i_name);
		}

		public bool CallConstructor()
		{
			TernaryBool specialized = CallConstructorSpecialized();
			if (!specialized.Unspecified())
			{
				return specialized.DefiniteYes();
			}
			return _container.ConfigImpl().CallConstructors().DefiniteYes();
		}

		private TernaryBool CallConstructorSpecialized()
		{
			if (i_config != null)
			{
				TernaryBool res = i_config.CallConstructor();
				if (!res.Unspecified())
				{
					return res;
				}
			}
			if (_isEnum)
			{
				return TernaryBool.NO;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.CallConstructorSpecialized();
			}
			return TernaryBool.UNSPECIFIED;
		}

		public override int OwnLength()
		{
			return MarshallerFamily.Current()._class.MarshalledLength(_container, this);
		}

		public virtual int PrefetchActivationDepth()
		{
			return ConfigOrAncestorConfig() == null ? 1 : 0;
		}

		internal virtual void Purge()
		{
			_index.Purge();
		}

		public virtual object ReadValueType(Transaction trans, int id, IActivationDepth depth
			)
		{
			ObjectReference @ref = trans.ReferenceForId(id);
			if (@ref != null)
			{
				object obj = @ref.GetObject();
				if (obj == null)
				{
					trans.RemoveReference(@ref);
				}
				else
				{
					@ref.Activate(trans, obj, depth);
					return @ref.GetObject();
				}
			}
			return new ObjectReference(id).Read(trans, depth, Const4.ADD_TO_ID_TREE, false);
		}

		public virtual ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, BufferImpl[] a_bytes)
		{
			if (IsArray())
			{
				return this;
			}
			return null;
		}

		public virtual ITypeHandler4 ReadArrayHandler1(BufferImpl[] a_bytes)
		{
			if (DTrace.enabled)
			{
				if (a_bytes[0] is StatefulBuffer)
				{
					DTrace.READ_ARRAY_WRAPPER.Log(((StatefulBuffer)a_bytes[0]).GetID());
				}
			}
			if (IsArray())
			{
				if (Platform4.IsCollectionTranslator(this.i_config))
				{
					a_bytes[0].IncrementOffset(Const4.INT_LENGTH);
					return new ArrayHandler(_container, null, false);
				}
				IncrementFieldsOffset1(a_bytes[0]);
				if (i_ancestor != null)
				{
					return i_ancestor.ReadArrayHandler1(a_bytes);
				}
			}
			return null;
		}

		public virtual ObjectID ReadObjectID(IInternalReadContext context)
		{
			int id = context.ReadInt();
			return id == 0 ? ObjectID.IS_NULL : new ObjectID(id);
		}

		public virtual void ReadCandidates(int handlerVersion, BufferImpl buffer, QCandidates
			 candidates)
		{
			int id = 0;
			int offset = buffer._offset;
			try
			{
				id = buffer.ReadInt();
			}
			catch (Exception)
			{
			}
			buffer._offset = offset;
			if (id != 0)
			{
				Transaction trans = candidates.i_trans;
				object obj = trans.Container().GetByID(trans, id);
				if (obj != null)
				{
					candidates.i_trans.Container().Activate(trans, obj, ActivationDepthProvider().ActivationDepth
						(2, ActivationMode.ACTIVATE));
					Platform4.ForEachCollectionElement(obj, new _IVisitor4_1328(this, candidates, trans
						));
				}
			}
		}

		private sealed class _IVisitor4_1328 : IVisitor4
		{
			public _IVisitor4_1328(ClassMetadata _enclosing, QCandidates candidates, Transaction
				 trans)
			{
				this._enclosing = _enclosing;
				this.candidates = candidates;
				this.trans = trans;
			}

			public void Visit(object elem)
			{
				candidates.AddByIdentity(new QCandidate(candidates, elem, trans.Container().GetID
					(trans, elem), true));
			}

			private readonly ClassMetadata _enclosing;

			private readonly QCandidates candidates;

			private readonly Transaction trans;
		}

		private IActivationDepthProvider ActivationDepthProvider()
		{
			return Stream().ActivationDepthProvider();
		}

		public int ReadFieldCount(BufferImpl buffer)
		{
			int count = buffer.ReadInt();
			if (count > i_fields.Length)
			{
				return i_fields.Length;
			}
			return count;
		}

		public virtual object ReadIndexEntry(BufferImpl a_reader)
		{
			return a_reader.ReadInt();
		}

		/// <exception cref="CorruptionException"></exception>
		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return ReadIndexEntry(a_writer);
		}

		internal virtual byte[] ReadName(Transaction a_trans)
		{
			i_reader = a_trans.Container().ReadReaderByID(a_trans, GetID());
			return ReadName1(a_trans, i_reader);
		}

		public byte[] ReadName1(Transaction trans, BufferImpl reader)
		{
			if (reader == null)
			{
				return null;
			}
			i_reader = reader;
			bool ok = false;
			try
			{
				ClassMarshaller marshaller = MarshallerFamily.Current()._class;
				i_nameBytes = marshaller.ReadName(trans, reader);
				_metaClassID = marshaller.ReadMetaClassID(reader);
				SetStateUnread();
				BitFalse(Const4.CHECKED_CHANGES);
				BitFalse(Const4.STATIC_FIELDS_STORED);
				ok = true;
				return i_nameBytes;
			}
			finally
			{
				if (!ok)
				{
					SetStateDead();
				}
			}
		}

		internal virtual void ReadVirtualAttributes(Transaction a_trans, ObjectReference 
			a_yapObject)
		{
			int id = a_yapObject.GetID();
			ObjectContainerBase stream = a_trans.Container();
			BufferImpl reader = stream.ReadReaderByID(a_trans, id);
			ObjectHeader oh = new ObjectHeader(stream, this, reader);
			oh.ObjectMarshaller().ReadVirtualAttributes(a_trans, this, a_yapObject, oh._headerAttributes
				, reader);
		}

		public virtual GenericReflector Reflector()
		{
			return _container.Reflector();
		}

		public virtual void Rename(string newName)
		{
			if (!_container.IsClient())
			{
				int tempState = _state;
				SetStateOK();
				i_name = newName;
				i_nameBytes = AsBytes(i_name);
				SetStateDirty();
				Write(_container.SystemTransaction());
				IReflectClass oldReflector = _reflector;
				_reflector = Container().Reflector().ForName(newName);
				Container().ClassCollection().RefreshClassCache(this, oldReflector);
				Refresh();
				_state = tempState;
			}
			else
			{
				Exceptions4.ThrowRuntimeException(58);
			}
		}

		private byte[] AsBytes(string str)
		{
			return Container().StringIO().Write(str);
		}

		internal void CreateConfigAndConstructor(Hashtable4 a_byteHashTable, IReflectClass
			 claxx, string name)
		{
			i_name = name;
			SetConfig(_container.ConfigImpl().ConfigClass(i_name));
			if (claxx == null)
			{
				CreateConstructor(_container, i_name);
			}
			else
			{
				CreateConstructor(_container, claxx, i_name, true);
			}
			if (i_nameBytes != null)
			{
				a_byteHashTable.Remove(i_nameBytes);
				i_nameBytes = null;
			}
		}

		internal virtual string ResolveName(IReflectClass claxx)
		{
			if (claxx != null)
			{
				return claxx.GetName();
			}
			if (i_nameBytes != null)
			{
				string name = _container.StringIO().Read(i_nameBytes);
				return _container.ConfigImpl().ResolveAliasStoredName(name);
			}
			throw new InvalidOperationException();
		}

		internal virtual bool ReadThis()
		{
			if (StateUnread())
			{
				SetStateOK();
				SetStateClean();
				ForceRead();
				return true;
			}
			return false;
		}

		internal void ForceRead()
		{
			if (i_reader == null || BitIsTrue(Const4.READING))
			{
				return;
			}
			BitTrue(Const4.READING);
			MarshallerFamily.ForConverterVersion(_container.ConverterVersion())._class.Read(_container
				, this, i_reader);
			i_nameBytes = null;
			i_reader = null;
			BitFalse(Const4.READING);
		}

		public override void ReadThis(Transaction a_trans, BufferImpl a_reader)
		{
			throw Exceptions4.VirtualException();
		}

		public virtual void Refresh()
		{
			if (!StateUnread())
			{
				CreateConstructor(_container, i_name);
				BitFalse(Const4.CHECKED_CHANGES);
				CheckChanges();
				if (i_fields != null)
				{
					for (int i = 0; i < i_fields.Length; i++)
					{
						i_fields[i].Refresh();
					}
				}
			}
		}

		internal virtual void RemoveFromIndex(Transaction ta, int id)
		{
			if (HasClassIndex())
			{
				_index.Remove(ta, id);
			}
			if (i_ancestor != null)
			{
				i_ancestor.RemoveFromIndex(ta, id);
			}
		}

		internal virtual bool RenameField(string a_from, string a_to)
		{
			bool renamed = false;
			for (int i = 0; i < i_fields.Length; i++)
			{
				if (i_fields[i].GetName().Equals(a_to))
				{
					_container.LogMsg(9, "class:" + GetName() + " field:" + a_to);
					return false;
				}
			}
			for (int i = 0; i < i_fields.Length; i++)
			{
				if (i_fields[i].GetName().Equals(a_from))
				{
					i_fields[i].SetName(a_to);
					renamed = true;
				}
			}
			return renamed;
		}

		internal virtual void SetConfig(Config4Class config)
		{
			if (config == null)
			{
				return;
			}
			if (i_config == null)
			{
				i_config = config;
			}
		}

		internal virtual void SetName(string a_name)
		{
			i_name = a_name;
		}

		internal void SetStateDead()
		{
			BitTrue(Const4.DEAD);
			BitFalse(Const4.CONTINUE);
		}

		private void SetStateUnread()
		{
			BitFalse(Const4.DEAD);
			BitTrue(Const4.CONTINUE);
		}

		private void SetStateOK()
		{
			BitFalse(Const4.DEAD);
			BitFalse(Const4.CONTINUE);
		}

		internal virtual bool StateDead()
		{
			return BitIsTrue(Const4.DEAD);
		}

		private bool StateOK()
		{
			return BitIsFalse(Const4.CONTINUE) && BitIsFalse(Const4.DEAD) && BitIsFalse(Const4
				.READING);
		}

		internal bool StateOKAndAncestors()
		{
			if (!StateOK() || i_fields == null)
			{
				return false;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.StateOKAndAncestors();
			}
			return true;
		}

		internal virtual bool StateUnread()
		{
			return BitIsTrue(Const4.CONTINUE) && BitIsFalse(Const4.DEAD) && BitIsFalse(Const4
				.READING);
		}

		internal virtual bool StoreField(IReflectField a_field)
		{
			if (a_field.IsStatic())
			{
				return false;
			}
			if (a_field.IsTransient())
			{
				Config4Class config = ConfigOrAncestorConfig();
				if (config == null)
				{
					return false;
				}
				if (!config.StoreTransientFields())
				{
					return false;
				}
			}
			return Platform4.CanSetAccessible() || a_field.IsPublic();
		}

		public virtual IStoredField StoredField(string name, object clazz)
		{
			lock (Lock())
			{
				ClassMetadata classMetadata = _container.ClassMetadataForReflectClass(ReflectorUtils
					.ReflectClassFor(Reflector(), clazz));
				if (i_fields != null)
				{
					for (int i = 0; i < i_fields.Length; i++)
					{
						if (i_fields[i].GetName().Equals(name))
						{
							if (classMetadata == null || classMetadata == i_fields[i].HandlerClassMetadata(_container
								))
							{
								return (i_fields[i]);
							}
						}
					}
				}
				return null;
			}
		}

		internal virtual void StoreStaticFieldValues(Transaction trans, bool force)
		{
			if (BitIsTrue(Const4.STATIC_FIELDS_STORED) && !force)
			{
				return;
			}
			BitTrue(Const4.STATIC_FIELDS_STORED);
			if (!ShouldStoreStaticFields(trans))
			{
				return;
			}
			ObjectContainerBase stream = trans.Container();
			stream.ShowInternalClasses(true);
			try
			{
				StaticClass sc = QueryStaticClass(trans);
				if (sc == null)
				{
					CreateStaticClass(trans);
				}
				else
				{
					UpdateStaticClass(trans, sc);
				}
			}
			finally
			{
				stream.ShowInternalClasses(false);
			}
		}

		private bool ShouldStoreStaticFields(Transaction trans)
		{
			return !trans.Container().Config().IsReadOnly() && (StaticFieldValuesArePersisted
				() || Platform4.StoreStaticFieldValues(trans.Reflector(), ClassReflector()));
		}

		private void UpdateStaticClass(Transaction trans, StaticClass sc)
		{
			ObjectContainerBase stream = trans.Container();
			stream.Activate(trans, sc, new FixedActivationDepth(4));
			StaticField[] existingFields = sc.fields;
			IEnumerator staticFields = Iterators.Map(StaticReflectFields(), new _IFunction4_1666
				(this, existingFields, trans));
			sc.fields = ToStaticFieldArray(staticFields);
			if (!stream.IsClient())
			{
				SetStaticClass(trans, sc);
			}
		}

		private sealed class _IFunction4_1666 : IFunction4
		{
			public _IFunction4_1666(ClassMetadata _enclosing, StaticField[] existingFields, Transaction
				 trans)
			{
				this._enclosing = _enclosing;
				this.existingFields = existingFields;
				this.trans = trans;
			}

			public object Apply(object arg)
			{
				IReflectField reflectField = (IReflectField)arg;
				StaticField existingField = this._enclosing.FieldByName(existingFields, reflectField
					.GetName());
				if (existingField != null)
				{
					this._enclosing.UpdateExistingStaticField(trans, existingField, reflectField);
					return existingField;
				}
				return this._enclosing.ToStaticField(reflectField);
			}

			private readonly ClassMetadata _enclosing;

			private readonly StaticField[] existingFields;

			private readonly Transaction trans;
		}

		private void CreateStaticClass(Transaction trans)
		{
			if (trans.Container().IsClient())
			{
				return;
			}
			StaticClass sc = new StaticClass(i_name, ToStaticFieldArray(StaticReflectFieldsToStaticFields
				()));
			SetStaticClass(trans, sc);
		}

		private IEnumerator StaticReflectFieldsToStaticFields()
		{
			return Iterators.Map(StaticReflectFields(), new _IFunction4_1694(this));
		}

		private sealed class _IFunction4_1694 : IFunction4
		{
			public _IFunction4_1694(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object arg)
			{
				return this._enclosing.ToStaticField((IReflectField)arg);
			}

			private readonly ClassMetadata _enclosing;
		}

		protected virtual StaticField ToStaticField(IReflectField reflectField)
		{
			return new StaticField(reflectField.GetName(), StaticReflectFieldValue(reflectField
				));
		}

		private object StaticReflectFieldValue(IReflectField reflectField)
		{
			reflectField.SetAccessible();
			return reflectField.Get(null);
		}

		private void SetStaticClass(Transaction trans, StaticClass sc)
		{
			trans.Container().SetInternal(trans, sc, true);
		}

		private StaticField[] ToStaticFieldArray(IEnumerator iterator4)
		{
			return ToStaticFieldArray(new Collection4(iterator4));
		}

		private StaticField[] ToStaticFieldArray(Collection4 fields)
		{
			return (StaticField[])fields.ToArray(new StaticField[fields.Size()]);
		}

		private IEnumerator StaticReflectFields()
		{
			return Iterators.Filter(ReflectFields(), new _IPredicate4_1724(this));
		}

		private sealed class _IPredicate4_1724 : IPredicate4
		{
			public _IPredicate4_1724(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public bool Match(object candidate)
			{
				return ((IReflectField)candidate).IsStatic();
			}

			private readonly ClassMetadata _enclosing;
		}

		private IReflectField[] ReflectFields()
		{
			return ClassReflector().GetDeclaredFields();
		}

		protected virtual void UpdateExistingStaticField(Transaction trans, StaticField existingField
			, IReflectField reflectField)
		{
			ObjectContainerBase stream = trans.Container();
			object newValue = StaticReflectFieldValue(reflectField);
			if (existingField.value != null && newValue != null && existingField.value.GetType
				() == newValue.GetType())
			{
				int id = stream.GetID(trans, existingField.value);
				if (id > 0)
				{
					if (existingField.value != newValue)
					{
						stream.Bind(trans, newValue, id);
						stream.Refresh(trans, newValue, int.MaxValue);
						existingField.value = newValue;
					}
					return;
				}
			}
			if (newValue == null)
			{
				try
				{
					reflectField.Set(null, existingField.value);
				}
				catch (Exception)
				{
				}
				return;
			}
			existingField.value = newValue;
		}

		private bool StaticFieldValuesArePersisted()
		{
			return (i_config != null && i_config.StaticFieldValuesArePersisted());
		}

		protected virtual StaticField FieldByName(StaticField[] fields, string fieldName)
		{
			for (int i = 0; i < fields.Length; i++)
			{
				StaticField field = fields[i];
				if (fieldName.Equals(field.name))
				{
					return field;
				}
			}
			return null;
		}

		private StaticClass QueryStaticClass(Transaction trans)
		{
			IQuery q = trans.Container().Query(trans);
			q.Constrain(Const4.CLASS_STATICCLASS);
			q.Descend("name").Constrain(i_name);
			IObjectSet os = q.Execute();
			return os.Size() > 0 ? (StaticClass)os.Next() : null;
		}

		public override string ToString()
		{
			if (i_name != null)
			{
				return i_name;
			}
			if (i_nameBytes == null)
			{
				return "*CLASS NAME UNKNOWN*";
			}
			LatinStringIO stringIO = _container == null ? Const4.stringIO : _container.StringIO
				();
			return stringIO.Read(i_nameBytes);
		}

		public override bool WriteObjectBegin()
		{
			if (!StateOK())
			{
				return false;
			}
			return base.WriteObjectBegin();
		}

		public virtual void WriteIndexEntry(BufferImpl a_writer, object a_object)
		{
			if (a_object == null)
			{
				a_writer.WriteInt(0);
				return;
			}
			a_writer.WriteInt(((int)a_object));
		}

		public sealed override void WriteThis(Transaction trans, BufferImpl writer)
		{
			MarshallerFamily.Current()._class.Write(trans, this, writer);
		}

		private IReflectClass i_compareTo;

		public virtual IComparable4 PrepareComparison(object obj)
		{
			if (obj == null)
			{
				i_lastID = 0;
				i_compareTo = null;
				return this;
			}
			if (obj is int)
			{
				i_lastID = ((int)obj);
			}
			else
			{
				if (obj is TransactionContext)
				{
					TransactionContext tc = (TransactionContext)obj;
					obj = tc._object;
					i_lastID = _container.GetID(tc._transaction, obj);
				}
				else
				{
					throw new IllegalComparisonException();
				}
			}
			i_compareTo = Reflector().ForObject(obj);
			return this;
		}

		public virtual int CompareTo(object obj)
		{
			if (obj is TransactionContext)
			{
				obj = ((TransactionContext)obj)._object;
			}
			if (obj is int)
			{
				return ((int)obj) - i_lastID;
			}
			if (obj == null)
			{
				if (i_compareTo == null)
				{
					return 0;
				}
				return -1;
			}
			if (i_compareTo != null)
			{
				if (i_compareTo.IsAssignableFrom(Reflector().ForObject(obj)))
				{
					return 0;
				}
			}
			throw new IllegalComparisonException();
		}

		public virtual IPreparedComparison NewPrepareCompare(object source)
		{
			if (source == null)
			{
				return new _IPreparedComparison_1884(this);
			}
			int id = 0;
			IReflectClass claxx = null;
			if (source is int)
			{
				id = ((int)source);
			}
			else
			{
				if (source is TransactionContext)
				{
					TransactionContext tc = (TransactionContext)source;
					object obj = tc._object;
					id = _container.GetID(tc._transaction, obj);
					claxx = Reflector().ForObject(obj);
				}
				else
				{
					throw new IllegalComparisonException();
				}
			}
			return new ClassMetadata.PreparedClassMetadataComparison(this, id, claxx);
		}

		private sealed class _IPreparedComparison_1884 : IPreparedComparison
		{
			public _IPreparedComparison_1884(ClassMetadata _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public int CompareTo(object obj)
			{
				if (obj == null)
				{
					return 0;
				}
				return -1;
			}

			private readonly ClassMetadata _enclosing;
		}

		public sealed class PreparedClassMetadataComparison : IPreparedComparison
		{
			private readonly int _id;

			private readonly IReflectClass _claxx;

			public PreparedClassMetadataComparison(ClassMetadata _enclosing, int id, IReflectClass
				 claxx)
			{
				this._enclosing = _enclosing;
				this._id = id;
				this._claxx = claxx;
			}

			public int CompareTo(object obj)
			{
				if (obj is TransactionContext)
				{
					obj = ((TransactionContext)obj)._object;
				}
				if (obj is int)
				{
					return this._id - ((int)obj);
				}
				if (this._claxx != null)
				{
					if (this._claxx.IsAssignableFrom(this._enclosing.Reflector().ForObject(obj)))
					{
						return 0;
					}
				}
				throw new IllegalComparisonException();
			}

			private readonly ClassMetadata _enclosing;
		}

		public static void DefragObject(DefragmentContextImpl context)
		{
			ObjectHeader header = ObjectHeader.Defrag(context);
			header._marshallerFamily._object.DefragFields(header.ClassMetadata(), header, context
				);
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			if (HasClassIndex())
			{
				context.CopyID();
			}
			else
			{
				context.CopyUnindexedID();
			}
			int restLength = (LinkLength() - Const4.INT_LENGTH);
			context.IncrementOffset(restLength);
		}

		/// <exception cref="CorruptionException"></exception>
		/// <exception cref="IOException"></exception>
		public virtual void DefragClass(DefragmentContextImpl context, int classIndexID)
		{
			MarshallerFamily mf = MarshallerFamily.Current();
			mf._class.Defrag(this, _container.StringIO(), context, classIndexID);
		}

		public static ClassMetadata ReadClass(ObjectContainerBase stream, BufferImpl reader
			)
		{
			ObjectHeader oh = new ObjectHeader(stream, reader);
			return oh.ClassMetadata();
		}

		public virtual bool IsAssignableFrom(ClassMetadata other)
		{
			return ClassReflector().IsAssignableFrom(other.ClassReflector());
		}

		public void DefragIndexEntry(DefragmentContextImpl context)
		{
			context.CopyID();
		}

		public virtual void SetAncestor(ClassMetadata ancestor)
		{
			if (ancestor == this)
			{
				throw new InvalidOperationException();
			}
			i_ancestor = ancestor;
		}

		public virtual object WrapWithTransactionContext(Transaction transaction, object 
			value)
		{
			if (value is int)
			{
				return value;
			}
			return new TransactionContext(transaction, value);
		}

		public virtual object Read(IReadContext context)
		{
			if (IsValueType())
			{
				IActivationDepth activationDepth = ((UnmarshallingContext)context).ActivationDepth
					();
				return ReadValueType(context.Transaction(), context.ReadInt(), activationDepth.Descend
					(this));
			}
			return context.ReadObject();
		}

		public virtual void Write(IWriteContext context, object obj)
		{
			context.WriteObject(obj);
		}

		public virtual ITypeHandler4 TypeHandler()
		{
			return this;
		}
	}
}
