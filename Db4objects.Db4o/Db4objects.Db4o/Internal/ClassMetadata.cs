/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Classindex;
using Db4objects.Db4o.Internal.Diagnostic;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Query.Processor;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ClassMetadata : PersistentBase, ITypeHandler4, IStoredClass
	{
		private ClassHandler _classHandler;

		public Db4objects.Db4o.Internal.ClassMetadata i_ancestor;

		private Config4Class i_config;

		public int _metaClassID;

		public FieldMetadata[] i_fields;

		private readonly IClassIndexStrategy _index;

		protected string i_name;

		private readonly ObjectContainerBase _stream;

		internal byte[] i_nameBytes;

		private Db4objects.Db4o.Internal.Buffer i_reader;

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
			return _stream;
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

		internal ClassMetadata(ObjectContainerBase container, IReflectClass reflector)
		{
			_stream = container;
			_reflector = reflector;
			_index = CreateIndexStrategy();
			_classIndexed = true;
			_classHandler = new ClassHandler(this);
		}

		internal virtual void ActivateFields(Transaction a_trans, object a_object, int a_depth
			)
		{
			if (ObjectCanActivate(a_trans.Stream(), a_object))
			{
				ActivateFields1(a_trans, a_object, a_depth);
			}
		}

		internal virtual void ActivateFields1(Transaction a_trans, object a_object, int a_depth
			)
		{
			for (int i = 0; i < i_fields.Length; i++)
			{
				i_fields[i].CascadeActivation(a_trans, a_object, a_depth, true);
			}
			if (i_ancestor != null)
			{
				i_ancestor.ActivateFields1(a_trans, a_object, a_depth);
			}
		}

		public void AddFieldIndices(StatefulBuffer a_writer, Slot oldSlot)
		{
			if (HasIndex() || HasVirtualAttributes())
			{
				ObjectHeader oh = new ObjectHeader(_stream, this, a_writer);
				oh._marshallerFamily._object.AddFieldIndices(this, oh._headerAttributes, a_writer
					, oldSlot);
			}
		}

		internal virtual void AddMembers(ObjectContainerBase ocb)
		{
			BitTrue(Const4.CHECKED_CHANGES);
			if (InstallTranslator(ocb) || InstallMarshaller(ocb))
			{
				return;
			}
			if (ocb.DetectSchemaChanges())
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
						members.Add(ocb.GetVersionIndex());
						dirty = true;
					}
				}
				if (GenerateUUIDs())
				{
					if (!HasUUIDField())
					{
						members.Add(ocb.GetUUIDIndex());
						dirty = true;
					}
				}
				dirty = CollectReflectFields(ocb, members) | dirty;
				if (dirty)
				{
					_stream.SetDirtyInSystemTransaction(this);
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
				DiagnosticProcessor dp = _stream.i_handlers._diagnosticProcessor;
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
			_stream.Callbacks().ClassOnRegistered(this);
			SetStateOK();
		}

		private bool CollectReflectFields(ObjectContainerBase stream, Collection4 collectedFields
			)
		{
			bool dirty = false;
			IReflectField[] fields = ReflectFields();
			for (int i = 0; i < fields.Length; i++)
			{
				if (StoreField(fields[i]))
				{
					ITypeHandler4 wrapper = stream.i_handlers.HandlerForClass(stream, fields[i].GetFieldType
						());
					if (wrapper == null)
					{
						continue;
					}
					FieldMetadata field = new FieldMetadata(this, fields[i], wrapper);
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

		private bool InstallMarshaller(ObjectContainerBase ocb)
		{
			IObjectMarshaller om = GetMarshaller();
			if (om == null)
			{
				return false;
			}
			InstallCustomFieldMetadata(ocb, new CustomMarshallerFieldMetadata(this, om));
			return true;
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
				_stream.SetDirtyInSystemTransaction(this);
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
				i_fields[1] = ocb.GetVersionIndex();
			}
			if (uuids)
			{
				i_fields[2] = ocb.GetUUIDIndex();
			}
			SetStateOK();
		}

		private IObjectTranslator GetTranslator()
		{
			return i_config == null ? null : i_config.GetTranslator();
		}

		private IObjectMarshaller GetMarshaller()
		{
			return i_config == null ? null : i_config.GetMarshaller();
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
			if (HasIndex())
			{
				_index.Add(a_trans, a_id);
			}
		}

		internal virtual bool AllowsQueries()
		{
			return HasIndex();
		}

		public virtual bool CanHold(IReflectClass claxx)
		{
			if (claxx == null)
			{
				return true;
			}
			if (_reflector != null)
			{
				if (ClassReflector().IsCollection())
				{
					return true;
				}
				return ClassReflector().IsAssignableFrom(claxx);
			}
			return false;
		}

		public virtual void CascadeActivation(Transaction a_trans, object a_object, int a_depth
			, bool a_activate)
		{
			Config4Class config = ConfigOrAncestorConfig();
			if (config != null)
			{
				if (a_activate)
				{
					a_depth = config.AdjustActivationDepth(a_depth);
				}
			}
			if (a_depth > 0)
			{
				ObjectContainerBase stream = a_trans.Stream();
				if (a_activate)
				{
					if (IsValueType())
					{
						ActivateFields(a_trans, a_object, a_depth - 1);
					}
					else
					{
						stream.StillToActivate(a_object, a_depth - 1);
					}
				}
				else
				{
					stream.StillToDeactivate(a_object, a_depth - 1, false);
				}
			}
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
						AddMembers(_stream);
						if (!_stream.IsClient())
						{
							Write(_stream.SystemTransaction());
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
			if (_stream.i_handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx))
			{
				_internal = true;
			}
			if (_stream.i_handlers.ICLASS_UNVERSIONED.IsAssignableFrom(claxx))
			{
				_unversioned = true;
			}
			if (_stream.i_handlers.ICLASS_DB4OTYPEIMPL.IsAssignableFrom(claxx))
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

		public virtual void CheckUpdateDepth(StatefulBuffer a_bytes)
		{
			int depth = a_bytes.GetUpdateDepth();
			Config4Class config = ConfigOrAncestorConfig();
			if (depth == Const4.UNSPECIFIED)
			{
				depth = CheckUpdateDepthUnspecified(a_bytes.GetStream());
				if (ClassReflector().IsCollection())
				{
					depth = AdjustDepth(depth);
				}
			}
			if ((config != null && (config.CascadeOnDelete() == TernaryBool.YES || config.CascadeOnUpdate
				() == TernaryBool.YES)))
			{
				depth = AdjustDepth(depth);
			}
			a_bytes.SetUpdateDepth(depth - 1);
		}

		private int AdjustDepth(int depth)
		{
			int depthBorder = Reflector().CollectionUpdateDepth(ClassReflector());
			if (depth > int.MinValue && depth < depthBorder)
			{
				depth = depthBorder;
			}
			return depth;
		}

		internal virtual int CheckUpdateDepthUnspecified(ObjectContainerBase a_stream)
		{
			int depth = a_stream.ConfigImpl().UpdateDepth() + 1;
			if (i_config != null && i_config.UpdateDepth() != 0)
			{
				depth = i_config.UpdateDepth() + 1;
			}
			if (i_ancestor != null)
			{
				int ancestordepth = i_ancestor.CheckUpdateDepthUnspecified(a_stream);
				if (ancestordepth > depth)
				{
					return ancestordepth;
				}
			}
			return depth;
		}

		public virtual object Coerce(IReflectClass claxx, object obj)
		{
			return CanHold(claxx) ? obj : No4.INSTANCE;
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
			return _classHandler.CustomizedNewInstance();
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

		public virtual void CopyValue(object a_from, object a_to)
		{
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
				if (a_stream.i_handlers.ICLASS_TRANSIENTCLASS.IsAssignableFrom(a_class) || Platform4
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
			if (a_stream.i_handlers.CreateConstructor(a_class, !CallConstructor()))
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

		public virtual void Deactivate(Transaction a_trans, object a_object, int a_depth)
		{
			if (ObjectCanDeactivate(a_trans.Stream(), a_object))
			{
				Deactivate1(a_trans, a_object, a_depth);
				ObjectOnDeactivate(a_trans.Stream(), a_object);
			}
		}

		private void ObjectOnDeactivate(ObjectContainerBase stream, object obj)
		{
			stream.Callbacks().ObjectOnDeactivate(obj);
			DispatchEvent(stream, obj, EventDispatcher.DEACTIVATE);
		}

		private bool ObjectCanDeactivate(ObjectContainerBase stream, object obj)
		{
			return stream.Callbacks().ObjectCanDeactivate(obj) && DispatchEvent(stream, obj, 
				EventDispatcher.CAN_DEACTIVATE);
		}

		internal virtual void Deactivate1(Transaction a_trans, object a_object, int a_depth
			)
		{
			for (int i = 0; i < i_fields.Length; i++)
			{
				i_fields[i].Deactivate(a_trans, a_object, a_depth);
			}
			if (i_ancestor != null)
			{
				i_ancestor.Deactivate1(a_trans, a_object, a_depth);
			}
		}

		internal void Delete(StatefulBuffer a_bytes, object a_object)
		{
			ObjectHeader oh = new ObjectHeader(_stream, this, a_bytes);
			Delete1(oh._marshallerFamily, oh._headerAttributes, a_bytes, a_object);
		}

		private void Delete1(MarshallerFamily mf, ObjectHeaderAttributes attributes, StatefulBuffer
			 a_bytes, object a_object)
		{
			RemoveFromIndex(a_bytes.GetTransaction(), a_bytes.GetID());
			DeleteMembers(mf, attributes, a_bytes, a_bytes.GetTransaction().Stream().i_handlers
				.ArrayType(a_object), false);
		}

		public virtual void DeleteEmbedded(MarshallerFamily mf, StatefulBuffer a_bytes)
		{
			if (a_bytes.CascadeDeletes() > 0)
			{
				int id = a_bytes.ReadInt();
				if (id > 0)
				{
					DeleteEmbedded1(mf, a_bytes, id);
				}
			}
			else
			{
				a_bytes.IncrementOffset(LinkLength());
			}
		}

		/// <param name="mf"></param>
		public virtual void DeleteEmbedded1(MarshallerFamily mf, StatefulBuffer a_bytes, 
			int a_id)
		{
			if (a_bytes.CascadeDeletes() > 0)
			{
				ObjectContainerBase stream = a_bytes.GetStream();
				object obj = stream.GetByID2(a_bytes.GetTransaction(), a_id);
				int cascade = a_bytes.CascadeDeletes() - 1;
				if (obj != null)
				{
					if (IsCollection(obj))
					{
						cascade += Reflector().CollectionUpdateDepth(Reflector().ForObject(obj)) - 1;
					}
				}
				ObjectReference yo = stream.ReferenceForId(a_id);
				if (yo != null)
				{
					a_bytes.GetStream().Delete2(a_bytes.GetTransaction(), yo, obj, cascade, false);
				}
			}
		}

		internal virtual void DeleteMembers(MarshallerFamily mf, ObjectHeaderAttributes attributes
			, StatefulBuffer a_bytes, int a_type, bool isUpdate)
		{
			try
			{
				Config4Class config = ConfigOrAncestorConfig();
				if (config != null && (config.CascadeOnDelete() == TernaryBool.YES))
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
			}
		}

		public bool DispatchEvent(ObjectContainerBase stream, object obj, int message)
		{
			if (_eventDispatcher == null || !stream.DispatchsEvents())
			{
				return true;
			}
			return _eventDispatcher.Dispatch(stream, obj, message);
		}

		public bool IsEqual(ITypeHandler4 a_dataType)
		{
			return (this == a_dataType);
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

		public MarshallerFamily FindOffset(Db4objects.Db4o.Internal.Buffer a_bytes, FieldMetadata
			 a_field)
		{
			if (a_bytes == null)
			{
				return null;
			}
			a_bytes._offset = 0;
			ObjectHeader oh = new ObjectHeader(_stream, this, a_bytes);
			bool res = oh.ObjectMarshaller().FindOffset(this, oh._headerAttributes, a_bytes, 
				a_field);
			if (!res)
			{
				return null;
			}
			return oh._marshallerFamily;
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
				return trans.Stream().ProduceClassMetadata(reflectClass);
			}
			return trans.Stream().ClassMetadataForReflectClass(reflectClass);
		}

		public virtual bool GenerateUUIDs()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateUUIDs();
			return Generate1(_stream.Config().GenerateUUIDs(), configValue);
		}

		private bool GenerateVersionNumbers()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			bool configValue = (i_config == null) ? false : i_config.GenerateVersionNumbers();
			return Generate1(_stream.Config().GenerateVersionNumbers(), configValue);
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
			lock (_stream.i_lock)
			{
				if (!StateOK())
				{
					return new long[0];
				}
				return GetIDs(_stream.GetTransaction());
			}
		}

		public virtual long[] GetIDs(Transaction trans)
		{
			if (!StateOK())
			{
				return new long[0];
			}
			if (!HasIndex())
			{
				return new long[0];
			}
			return trans.Stream().GetIDsForClass(trans, this);
		}

		public virtual bool HasIndex()
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
			return GetStream().GetByID2(trans, id);
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
			lock (_stream.i_lock)
			{
				if (i_fields == null)
				{
					return null;
				}
				IStoredField[] fields = new IStoredField[i_fields.Length];
				System.Array.Copy(i_fields, 0, fields, 0, i_fields.Length);
				return fields;
			}
		}

		internal virtual ObjectContainerBase GetStream()
		{
			return _stream;
		}

		public virtual int GetTypeID()
		{
			return Const4.TYPE_CLASS;
		}

		public virtual ClassMetadata GetClassMetadata(ObjectContainerBase a_stream)
		{
			return this;
		}

		public virtual FieldMetadata FieldMetadataForName(string name)
		{
			FieldMetadata[] yf = new FieldMetadata[1];
			ForEachFieldMetadata(new _IVisitor4_938(this, name, yf));
			return yf[0];
		}

		private sealed class _IVisitor4_938 : IVisitor4
		{
			public _IVisitor4_938(ClassMetadata _enclosing, string name, FieldMetadata[] yf)
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

		public virtual bool HasFixedLength()
		{
			return true;
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

		internal virtual void IncrementFieldsOffset1(Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			int length = Debug.atHome ? ReadFieldCountSodaAtHome(a_bytes) : ReadFieldCount(a_bytes
				);
			for (int i = 0; i < length; i++)
			{
				i_fields[i].IncrementOffset(a_bytes);
			}
		}

		public virtual object ComparableObject(Transaction a_trans, object a_object)
		{
			return a_object;
		}

		internal bool Init(ObjectContainerBase a_stream, ClassMetadata a_ancestor, IReflectClass
			 claxx)
		{
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
			Config4Class extendedConfig = Platform4.ExtendConfiguration(_reflector, _stream.Configure
				(), i_config);
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

		internal virtual object Instantiate(ObjectReference @ref, object obj, MarshallerFamily
			 mf, ObjectHeaderAttributes attributes, StatefulBuffer buffer, bool addToIDTree)
		{
			AdjustInstantiationDepth(buffer);
			ObjectContainerBase stream = buffer.GetStream();
			bool instantiating = (obj == null);
			if (instantiating)
			{
				obj = InstantiateObject(buffer, mf);
				if (obj == null)
				{
					return null;
				}
				ShareTransaction(obj, buffer.GetTransaction());
				ShareObjectReference(obj, @ref);
				@ref.SetObjectWeak(stream, obj);
				stream.ReferenceSystem().AddExistingReferenceToObjectTree(@ref);
				ObjectOnInstantiate(stream, obj);
			}
			if (addToIDTree)
			{
				@ref.AddExistingReferenceToIdTree(stream);
			}
			if (instantiating)
			{
				if (buffer.GetInstantiationDepth() == 0)
				{
					@ref.SetStateDeactivated();
				}
				else
				{
					Activate(stream, buffer, mf, attributes, @ref, obj);
				}
			}
			else
			{
				if (ActivatingActiveObject(stream, @ref))
				{
					if (buffer.GetInstantiationDepth() > 1)
					{
						ActivateFields(buffer.GetTransaction(), obj, buffer.GetInstantiationDepth() - 1);
					}
				}
				else
				{
					Activate(stream, buffer, mf, attributes, @ref, obj);
				}
			}
			return obj;
		}

		private bool ActivatingActiveObject(ObjectContainerBase stream, ObjectReference @ref
			)
		{
			return !stream.i_refreshInsteadOfActivate && @ref.IsActive();
		}

		private void Activate(ObjectContainerBase stream, StatefulBuffer buffer, MarshallerFamily
			 mf, ObjectHeaderAttributes attributes, ObjectReference @ref, object obj)
		{
			if (ObjectCanActivate(stream, obj))
			{
				@ref.SetStateClean();
				if (buffer.GetInstantiationDepth() > 0 || CascadeOnActivate())
				{
					InstantiateFields(@ref, obj, mf, attributes, buffer);
				}
				ObjectOnActivate(stream, obj);
			}
			else
			{
				@ref.SetStateDeactivated();
			}
		}

		internal virtual object InstantiateObject(StatefulBuffer buffer, MarshallerFamily
			 mf)
		{
			return _classHandler.InstantiateObject(buffer, mf);
		}

		private void ObjectOnInstantiate(ObjectContainerBase container, object instance)
		{
			container.Callbacks().ObjectOnInstantiate(instance);
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

		internal virtual object InstantiateFromConfig(ObjectContainerBase stream, StatefulBuffer
			 a_bytes, MarshallerFamily mf)
		{
			int bytesOffset = a_bytes._offset;
			a_bytes.IncrementOffset(Const4.INT_LENGTH);
			try
			{
				return i_config.Instantiate(stream, i_fields[0].Read(mf, a_bytes));
			}
			catch (CorruptionException e)
			{
				Db4objects.Db4o.Internal.Messages.LogErr(stream.ConfigImpl(), 6, ClassReflector()
					.GetName(), e);
				return null;
			}
			finally
			{
				a_bytes._offset = bytesOffset;
			}
		}

		private void AdjustInstantiationDepth(StatefulBuffer a_bytes)
		{
			if (i_config != null)
			{
				a_bytes.SetInstantiationDepth(i_config.AdjustActivationDepth(a_bytes.GetInstantiationDepth
					()));
			}
		}

		private bool CascadeOnActivate()
		{
			return i_config != null && (i_config.CascadeOnActivate() == TernaryBool.YES);
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

		private void ObjectOnActivate(ObjectContainerBase stream, object obj)
		{
			stream.Callbacks().ObjectOnActivate(obj);
			DispatchEvent(stream, obj, EventDispatcher.ACTIVATE);
		}

		private bool ObjectCanActivate(ObjectContainerBase stream, object obj)
		{
			return stream.Callbacks().ObjectCanActivate(obj) && DispatchEvent(stream, obj, EventDispatcher
				.CAN_ACTIVATE);
		}

		/// <param name="obj"></param>
		internal virtual object InstantiateTransient(ObjectReference yapObject, object obj
			, MarshallerFamily mf, ObjectHeaderAttributes attributes, StatefulBuffer buffer)
		{
			object instantiated = InstantiateObject(buffer, mf);
			if (instantiated == null)
			{
				return null;
			}
			buffer.GetStream().Peeked(yapObject.GetID(), instantiated);
			InstantiateFields(yapObject, instantiated, mf, attributes, buffer);
			return instantiated;
		}

		internal virtual void InstantiateFields(ObjectReference a_yapObject, object a_onObject
			, MarshallerFamily mf, ObjectHeaderAttributes attributes, StatefulBuffer a_bytes
			)
		{
			mf._object.InstantiateFields(this, attributes, a_yapObject, a_onObject, a_bytes);
		}

		public virtual bool IndexNullHandling()
		{
			return true;
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

		public virtual TernaryBool IsSecondClass()
		{
			return TernaryBool.NO;
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

		internal virtual bool IsValueType()
		{
			return Platform4.IsValueType(ClassReflector());
		}

		public virtual void CalculateLengths(Transaction trans, ObjectHeaderAttributes header
			, bool topLevel, object obj, bool withIndirection)
		{
			if (topLevel)
			{
				header.AddBaseLength(LinkLength());
			}
			else
			{
				header.AddPayLoadLength(LinkLength());
			}
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
			return _stream.ConfigImpl().ResolveAliasRuntimeName(i_name);
		}

		public bool CallConstructor()
		{
			TernaryBool specialized = CallConstructorSpecialized();
			if (!specialized.Unspecified())
			{
				return specialized.DefiniteYes();
			}
			return _stream.ConfigImpl().CallConstructors().DefiniteYes();
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
			return MarshallerFamily.Current()._class.MarshalledLength(_stream, this);
		}

		public virtual IReflectClass PrimitiveClassReflector()
		{
			return null;
		}

		internal virtual void Purge()
		{
			_index.Purge();
		}

		public virtual object Read(MarshallerFamily mf, StatefulBuffer a_bytes, bool redirect
			)
		{
			try
			{
				int id = a_bytes.ReadInt();
				int depth = a_bytes.GetInstantiationDepth() - 1;
				Transaction trans = a_bytes.GetTransaction();
				ObjectContainerBase stream = trans.Stream();
				if (a_bytes.GetUpdateDepth() == Const4.TRANSIENT)
				{
					return stream.PeekPersisted(trans, id, depth);
				}
				if (IsValueType())
				{
					if (depth < 1)
					{
						depth = 1;
					}
					ObjectReference yo = stream.ReferenceForId(id);
					if (yo != null)
					{
						object obj = yo.GetObject();
						if (obj == null)
						{
							stream.RemoveReference(yo);
						}
						else
						{
							yo.Activate(trans, obj, depth, false);
							return yo.GetObject();
						}
					}
					return new ObjectReference(id).Read(trans, depth, Const4.ADD_TO_ID_TREE, false);
				}
				object ret = stream.GetByID2(trans, id);
				if (ret is IDb4oTypeImpl)
				{
					depth = ((IDb4oTypeImpl)ret).AdjustReadDepth(depth);
				}
				stream.StillToActivate(ret, depth);
				return ret;
			}
			catch (Exception)
			{
			}
			return null;
		}

		public virtual object ReadQuery(Transaction a_trans, MarshallerFamily mf, bool withRedirection
			, Db4objects.Db4o.Internal.Buffer a_reader, bool a_toArray)
		{
			try
			{
				return a_trans.Stream().GetByID2(a_trans, a_reader.ReadInt());
			}
			catch (Exception e)
			{
			}
			return null;
		}

		public virtual ITypeHandler4 ReadArrayHandler(Transaction a_trans, MarshallerFamily
			 mf, Db4objects.Db4o.Internal.Buffer[] a_bytes)
		{
			if (IsArray())
			{
				return this;
			}
			return null;
		}

		public virtual ITypeHandler4 ReadArrayHandler1(Db4objects.Db4o.Internal.Buffer[] 
			a_bytes)
		{
			if (IsArray())
			{
				if (Platform4.IsCollectionTranslator(this.i_config))
				{
					a_bytes[0].IncrementOffset(Const4.INT_LENGTH);
					return new ArrayHandler(_stream, null, false);
				}
				IncrementFieldsOffset1(a_bytes[0]);
				if (i_ancestor != null)
				{
					return i_ancestor.ReadArrayHandler1(a_bytes);
				}
			}
			return null;
		}

		public virtual QCandidate ReadSubCandidate(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 reader, QCandidates candidates, bool withIndirection)
		{
			int id = reader.ReadInt();
			if (id == 0)
			{
				return null;
			}
			return new QCandidate(candidates, null, id, true);
		}

		public virtual void ReadCandidates(MarshallerFamily mf, Db4objects.Db4o.Internal.Buffer
			 a_bytes, QCandidates a_candidates)
		{
			int id = 0;
			int offset = a_bytes._offset;
			try
			{
				id = a_bytes.ReadInt();
			}
			catch (Exception)
			{
			}
			a_bytes._offset = offset;
			if (id != 0)
			{
				Transaction trans = a_candidates.i_trans;
				object obj = trans.Stream().GetByID1(trans, id);
				if (obj != null)
				{
					a_candidates.i_trans.Stream().Activate1(trans, obj, 2);
					Platform4.ForEachCollectionElement(obj, new _IVisitor4_1426(this, a_candidates, trans
						));
				}
			}
		}

		private sealed class _IVisitor4_1426 : IVisitor4
		{
			public _IVisitor4_1426(ClassMetadata _enclosing, QCandidates a_candidates, Transaction
				 trans)
			{
				this._enclosing = _enclosing;
				this.a_candidates = a_candidates;
				this.trans = trans;
			}

			public void Visit(object elem)
			{
				a_candidates.AddByIdentity(new QCandidate(a_candidates, elem, (int)trans.Stream()
					.GetID(elem), true));
			}

			private readonly ClassMetadata _enclosing;

			private readonly QCandidates a_candidates;

			private readonly Transaction trans;
		}

		public int ReadFieldCount(Db4objects.Db4o.Internal.Buffer a_bytes)
		{
			int count = a_bytes.ReadInt();
			if (count > i_fields.Length)
			{
				return i_fields.Length;
			}
			return count;
		}

		public virtual int ReadFieldCountSodaAtHome(Db4objects.Db4o.Internal.Buffer a_bytes
			)
		{
			return 0;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader)
		{
			return a_reader.ReadInt();
		}

		public virtual object ReadIndexEntry(MarshallerFamily mf, StatefulBuffer a_writer
			)
		{
			return ReadIndexEntry(a_writer);
		}

		internal virtual byte[] ReadName(Transaction a_trans)
		{
			i_reader = a_trans.Stream().ReadReaderByID(a_trans, GetID());
			return ReadName1(a_trans, i_reader);
		}

		public byte[] ReadName1(Transaction trans, Db4objects.Db4o.Internal.Buffer reader
			)
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
			ObjectContainerBase stream = a_trans.Stream();
			Db4objects.Db4o.Internal.Buffer reader = stream.ReadReaderByID(a_trans, id);
			ObjectHeader oh = new ObjectHeader(stream, this, reader);
			oh.ObjectMarshaller().ReadVirtualAttributes(a_trans, this, a_yapObject, oh._headerAttributes
				, reader);
		}

		internal virtual GenericReflector Reflector()
		{
			return _stream.Reflector();
		}

		public virtual void Rename(string newName)
		{
			if (!_stream.IsClient())
			{
				int tempState = i_state;
				SetStateOK();
				i_name = newName;
				SetStateDirty();
				Write(_stream.SystemTransaction());
				i_state = tempState;
			}
			else
			{
				Exceptions4.ThrowRuntimeException(58);
			}
		}

		internal void CreateConfigAndConstructor(Hashtable4 a_byteHashTable, IReflectClass
			 claxx, string name)
		{
			i_name = name;
			SetConfig(_stream.ConfigImpl().ConfigClass(i_name));
			if (claxx == null)
			{
				CreateConstructor(_stream, i_name);
			}
			else
			{
				CreateConstructor(_stream, claxx, i_name, true);
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
				string name = _stream.StringIO().Read(i_nameBytes);
				return _stream.ConfigImpl().ResolveAliasStoredName(name);
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
			MarshallerFamily.ForConverterVersion(_stream.ConverterVersion())._class.Read(_stream
				, this, i_reader);
			i_nameBytes = null;
			i_reader = null;
			BitFalse(Const4.READING);
		}

		public virtual bool ReadArray(object array, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			return false;
		}

		public override void ReadThis(Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_reader)
		{
			throw Exceptions4.VirtualException();
		}

		public virtual void Refresh()
		{
			if (!StateUnread())
			{
				CreateConstructor(_stream, i_name);
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
			if (HasIndex())
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
					_stream.LogMsg(9, "class:" + GetName() + " field:" + a_to);
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
				ICustomClassHandler customHandler = config.CustomHandler();
				if (customHandler != null)
				{
					_classHandler = new CustomizedClassHandler(this, customHandler);
				}
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

		public virtual IStoredField StoredField(string a_name, object a_type)
		{
			lock (_stream.i_lock)
			{
				ClassMetadata yc = _stream.ClassMetadataForReflectClass(ReflectorUtils.ReflectClassFor
					(Reflector(), a_type));
				if (i_fields != null)
				{
					for (int i = 0; i < i_fields.Length; i++)
					{
						if (i_fields[i].GetName().Equals(a_name))
						{
							if (yc == null || yc == i_fields[i].GetFieldYapClass(_stream))
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
			ObjectContainerBase stream = trans.Stream();
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
			return !trans.Stream().Config().IsReadOnly() && (StaticFieldValuesArePersisted() 
				|| Platform4.StoreStaticFieldValues(trans.Reflector(), ClassReflector()));
		}

		private void UpdateStaticClass(Transaction trans, StaticClass sc)
		{
			ObjectContainerBase stream = trans.Stream();
			stream.Activate1(trans, sc, 4);
			StaticField[] existingFields = sc.fields;
			IEnumerator staticFields = Iterators.Map(StaticReflectFields(), new _IFunction4_1765
				(this, existingFields, trans));
			sc.fields = ToStaticFieldArray(staticFields);
			if (!stream.IsClient())
			{
				SetStaticClass(trans, sc);
			}
		}

		private sealed class _IFunction4_1765 : IFunction4
		{
			public _IFunction4_1765(ClassMetadata _enclosing, StaticField[] existingFields, Transaction
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
			if (trans.Stream().IsClient())
			{
				return;
			}
			StaticClass sc = new StaticClass(i_name, ToStaticFieldArray(StaticReflectFieldsToStaticFields
				()));
			SetStaticClass(trans, sc);
		}

		private IEnumerator StaticReflectFieldsToStaticFields()
		{
			return Iterators.Map(StaticReflectFields(), new _IFunction4_1793(this));
		}

		private sealed class _IFunction4_1793 : IFunction4
		{
			public _IFunction4_1793(ClassMetadata _enclosing)
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
			trans.Stream().SetInternal(trans, sc, true);
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
			return Iterators.Filter(ReflectFields(), new _IPredicate4_1823(this));
		}

		private sealed class _IPredicate4_1823 : IPredicate4
		{
			public _IPredicate4_1823(ClassMetadata _enclosing)
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
			ObjectContainerBase stream = trans.Stream();
			object newValue = StaticReflectFieldValue(reflectField);
			if (existingField.value != null && newValue != null && existingField.value.GetType
				() == newValue.GetType())
			{
				long id = stream.GetID1(existingField.value);
				if (id > 0)
				{
					if (existingField.value != newValue)
					{
						stream.Bind1(trans, newValue, id);
						stream.Refresh(newValue, int.MaxValue);
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
			IQuery q = trans.Stream().Query(trans);
			q.Constrain(Const4.CLASS_STATICCLASS);
			q.Descend("name").Constrain(i_name);
			IObjectSet os = q.Execute();
			return os.Size() > 0 ? (StaticClass)os.Next() : null;
		}

		public virtual bool SupportsIndex()
		{
			return true;
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
			LatinStringIO stringIO = _stream == null ? Const4.stringIO : _stream.StringIO();
			return stringIO.Read(i_nameBytes);
		}

		public virtual bool WriteArray(object array, Db4objects.Db4o.Internal.Buffer reader
			)
		{
			return false;
		}

		public override bool WriteObjectBegin()
		{
			if (!StateOK())
			{
				return false;
			}
			return base.WriteObjectBegin();
		}

		public virtual void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object
			 a_object)
		{
			if (a_object == null)
			{
				a_writer.WriteInt(0);
				return;
			}
			a_writer.WriteInt(((int)a_object));
		}

		public virtual object WriteNew(MarshallerFamily mf, object a_object, bool topLevel
			, StatefulBuffer a_bytes, bool withIndirection, bool restoreLinkOffset)
		{
			if (a_object == null)
			{
				a_bytes.WriteInt(0);
				return 0;
			}
			int id = a_bytes.GetStream().SetInternal(a_bytes.GetTransaction(), a_object, a_bytes
				.GetUpdateDepth(), true);
			a_bytes.WriteInt(id);
			return id;
		}

		public sealed override void WriteThis(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 writer)
		{
			MarshallerFamily.Current()._class.Write(trans, this, writer);
		}

		private IReflectClass i_compareTo;

		public virtual void PrepareComparison(Transaction a_trans, object obj)
		{
			PrepareComparison(obj);
		}

		public virtual IComparable4 PrepareComparison(object obj)
		{
			if (obj != null)
			{
				if (obj is int)
				{
					i_lastID = ((int)obj);
				}
				else
				{
					i_lastID = (int)_stream.GetID(obj);
				}
				i_compareTo = Reflector().ForObject(obj);
			}
			else
			{
				i_lastID = 0;
				i_compareTo = null;
			}
			return this;
		}

		public virtual object Current()
		{
			if (i_compareTo == null)
			{
				return null;
			}
			return i_lastID;
		}

		public virtual int CompareTo(object a_obj)
		{
			if (a_obj is int)
			{
				return ((int)a_obj) - i_lastID;
			}
			if ((a_obj == null) && (i_compareTo == null))
			{
				return 0;
			}
			return -1;
		}

		public virtual bool IsEqual(object obj)
		{
			if (obj == null)
			{
				return i_compareTo == null;
			}
			return i_compareTo.IsAssignableFrom(Reflector().ForObject(obj));
		}

		public virtual bool IsGreater(object obj)
		{
			return false;
		}

		public virtual bool IsSmaller(object obj)
		{
			return false;
		}

		public virtual string ToString(MarshallerFamily mf, StatefulBuffer writer, ObjectReference
			 yapObject, int depth, int maxDepth)
		{
			int length = ReadFieldCount(writer);
			string str = string.Empty;
			for (int i = 0; i < length; i++)
			{
				str += i_fields[i].ToString(mf, writer);
			}
			if (i_ancestor != null)
			{
				str += i_ancestor.ToString(mf, writer, yapObject, depth, maxDepth);
			}
			return str;
		}

		public static void DefragObject(ReaderPair readers)
		{
			ObjectHeader header = ObjectHeader.Defrag(readers);
			header._marshallerFamily._object.DefragFields(header.YapClass(), header, readers);
		}

		public virtual void Defrag(MarshallerFamily mf, ReaderPair readers, bool redirect
			)
		{
			if (HasIndex())
			{
				readers.CopyID();
			}
			else
			{
				readers.CopyUnindexedID();
			}
			int restLength = (LinkLength() - Const4.INT_LENGTH);
			readers.IncrementOffset(restLength);
		}

		public virtual void DefragClass(ReaderPair readers, int classIndexID)
		{
			MarshallerFamily mf = MarshallerFamily.Current();
			mf._class.Defrag(this, _stream.StringIO(), readers, classIndexID);
		}

		public static ClassMetadata ReadClass(ObjectContainerBase stream, Db4objects.Db4o.Internal.Buffer
			 reader)
		{
			ObjectHeader oh = new ObjectHeader(stream, reader);
			return oh.YapClass();
		}

		public virtual bool IsAssignableFrom(ClassMetadata other)
		{
			return ClassReflector().IsAssignableFrom(other.ClassReflector());
		}

		public void DefragIndexEntry(ReaderPair readers)
		{
			readers.CopyID();
		}

		public virtual void SetAncestor(ClassMetadata ancestor)
		{
			if (ancestor == this)
			{
				throw new InvalidOperationException();
			}
			i_ancestor = ancestor;
		}
	}
}
