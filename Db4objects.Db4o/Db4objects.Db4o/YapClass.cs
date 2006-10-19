namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapClass : Db4objects.Db4o.YapMeta, Db4objects.Db4o.ITypeHandler4, Db4objects.Db4o.Ext.IStoredClass
	{
		public Db4objects.Db4o.YapClass i_ancestor;

		internal Db4objects.Db4o.Config4Class i_config;

		public int _metaClassID;

		public Db4objects.Db4o.YapField[] i_fields;

		private readonly Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy _index;

		protected string i_name;

		protected readonly Db4objects.Db4o.YapStream i_stream;

		internal byte[] i_nameBytes;

		private Db4objects.Db4o.YapReader i_reader;

		private Db4objects.Db4o.IDb4oTypeImpl i_db4oType;

		private Db4objects.Db4o.Reflect.IReflectClass _reflector;

		private bool _isEnum;

		public bool i_dontCallConstructors;

		private Db4objects.Db4o.EventDispatcher _eventDispatcher;

		private bool _internal;

		private bool _unversioned;

		internal virtual bool IsInternal()
		{
			return _internal;
		}

		private Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy CreateIndexStrategy
			()
		{
			return new Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy(this);
		}

		private int i_lastID;

		internal YapClass(Db4objects.Db4o.YapStream stream, Db4objects.Db4o.Reflect.IReflectClass
			 reflector)
		{
			i_stream = stream;
			_reflector = reflector;
			_index = CreateIndexStrategy();
		}

		internal virtual void ActivateFields(Db4objects.Db4o.Transaction a_trans, object 
			a_object, int a_depth)
		{
			if (ObjectCanActivate(a_trans.Stream(), a_object))
			{
				ActivateFields1(a_trans, a_object, a_depth);
			}
		}

		internal virtual void ActivateFields1(Db4objects.Db4o.Transaction a_trans, object
			 a_object, int a_depth)
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

		internal void AddFieldIndices(Db4objects.Db4o.YapWriter a_writer, Db4objects.Db4o.Inside.Slots.Slot
			 oldSlot)
		{
			if (HasIndex() || HasVirtualAttributes())
			{
				Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
					(i_stream, this, a_writer);
				oh._marshallerFamily._object.AddFieldIndices(this, oh._headerAttributes, a_writer
					, oldSlot);
			}
		}

		internal virtual void AddMembers(Db4objects.Db4o.YapStream a_stream)
		{
			BitTrue(Db4objects.Db4o.YapConst.CHECKED_CHANGES);
			if (AddTranslatorFields(a_stream))
			{
				return;
			}
			if (a_stream.DetectSchemaChanges())
			{
				System.Collections.IEnumerator m;
				bool found;
				bool dirty = IsDirty();
				Db4objects.Db4o.YapField field;
				Db4objects.Db4o.ITypeHandler4 wrapper;
				Db4objects.Db4o.Foundation.Collection4 members = new Db4objects.Db4o.Foundation.Collection4
					();
				if (null != i_fields)
				{
					members.AddAll(i_fields);
				}
				if (GenerateVersionNumbers())
				{
					if (!HasVersionField())
					{
						members.Add(a_stream.i_handlers.i_indexes.i_fieldVersion);
						dirty = true;
					}
				}
				if (GenerateUUIDs())
				{
					if (!HasUUIDField())
					{
						members.Add(a_stream.i_handlers.i_indexes.i_fieldUUID);
						dirty = true;
					}
				}
				Db4objects.Db4o.Reflect.IReflectField[] fields = ClassReflector().GetDeclaredFields
					();
				for (int i = 0; i < fields.Length; i++)
				{
					if (StoreField(fields[i]))
					{
						wrapper = a_stream.i_handlers.HandlerForClass(a_stream, fields[i].GetFieldType());
						if (wrapper == null)
						{
							continue;
						}
						field = new Db4objects.Db4o.YapField(this, fields[i], wrapper);
						found = false;
						m = members.GetEnumerator();
						while (m.MoveNext())
						{
							if (((Db4objects.Db4o.YapField)m.Current).Equals(field))
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
						members.Add(field);
					}
				}
				if (dirty)
				{
					i_stream.SetDirtyInSystemTransaction(this);
					i_fields = new Db4objects.Db4o.YapField[members.Size()];
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
						i_fields = new Db4objects.Db4o.YapField[0];
					}
				}
				Db4objects.Db4o.Inside.Diagnostic.DiagnosticProcessor dp = i_stream.i_handlers._diagnosticProcessor;
				if (dp.Enabled())
				{
					dp.CheckClassHasFields(this);
				}
			}
			else
			{
				if (i_fields == null)
				{
					i_fields = new Db4objects.Db4o.YapField[0];
				}
			}
			SetStateOK();
		}

		private bool AddTranslatorFields(Db4objects.Db4o.YapStream a_stream)
		{
			Db4objects.Db4o.Config.IObjectTranslator ot = GetTranslator();
			if (ot == null)
			{
				return false;
			}
			if (IsNewTranslator(ot))
			{
				i_stream.SetDirtyInSystemTransaction(this);
			}
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
			i_fields = new Db4objects.Db4o.YapField[fieldCount];
			i_fields[0] = new Db4objects.Db4o.YapFieldTranslator(this, ot);
			if (versions || uuids)
			{
				i_fields[1] = a_stream.i_handlers.i_indexes.i_fieldVersion;
			}
			if (uuids)
			{
				i_fields[2] = a_stream.i_handlers.i_indexes.i_fieldUUID;
			}
			SetStateOK();
			return true;
		}

		private Db4objects.Db4o.Config.IObjectTranslator GetTranslator()
		{
			return i_config == null ? null : i_config.GetTranslator();
		}

		private bool IsNewTranslator(Db4objects.Db4o.Config.IObjectTranslator ot)
		{
			return !HasFields() || !ot.GetType().FullName.Equals(i_fields[0].GetName());
		}

		private bool HasFields()
		{
			return i_fields != null && i_fields.Length > 0;
		}

		internal virtual void AddToIndex(Db4objects.Db4o.YapFile a_stream, Db4objects.Db4o.Transaction
			 a_trans, int a_id)
		{
			if (a_stream.MaintainsIndices())
			{
				AddToIndex1(a_stream, a_trans, a_id);
			}
		}

		internal virtual void AddToIndex1(Db4objects.Db4o.YapFile a_stream, Db4objects.Db4o.Transaction
			 a_trans, int a_id)
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

		public virtual bool CanHold(Db4objects.Db4o.Reflect.IReflectClass claxx)
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

		public virtual void CascadeActivation(Db4objects.Db4o.Transaction a_trans, object
			 a_object, int a_depth, bool a_activate)
		{
			Db4objects.Db4o.Config4Class config = ConfigOrAncestorConfig();
			if (config != null)
			{
				if (a_activate)
				{
					a_depth = config.AdjustActivationDepth(a_depth);
				}
			}
			if (a_depth > 0)
			{
				Db4objects.Db4o.YapStream stream = a_trans.Stream();
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
				if (!BitIsTrue(Db4objects.Db4o.YapConst.CHECKED_CHANGES))
				{
					BitTrue(Db4objects.Db4o.YapConst.CHECKED_CHANGES);
					if (i_ancestor != null)
					{
						i_ancestor.CheckChanges();
					}
					if (_reflector != null)
					{
						AddMembers(i_stream);
						if (!i_stream.IsClient())
						{
							Write(i_stream.GetSystemTransaction());
						}
					}
				}
			}
		}

		public virtual void CheckDb4oType()
		{
			Db4objects.Db4o.Reflect.IReflectClass claxx = ClassReflector();
			if (claxx == null)
			{
				return;
			}
			if (i_stream.i_handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx))
			{
				_internal = true;
			}
			if (i_stream.i_handlers.ICLASS_UNVERSIONED.IsAssignableFrom(claxx))
			{
				_unversioned = true;
			}
			if (i_stream.i_handlers.ICLASS_DB4OTYPEIMPL.IsAssignableFrom(claxx))
			{
				try
				{
					i_db4oType = (Db4objects.Db4o.IDb4oTypeImpl)claxx.NewInstance();
				}
				catch (System.Exception e)
				{
				}
			}
		}

		public virtual void CheckUpdateDepth(Db4objects.Db4o.YapWriter a_bytes)
		{
			int depth = a_bytes.GetUpdateDepth();
			Db4objects.Db4o.Config4Class config = ConfigOrAncestorConfig();
			if (depth == Db4objects.Db4o.YapConst.UNSPECIFIED)
			{
				depth = CheckUpdateDepthUnspecified(a_bytes.GetStream());
				if (ClassReflector().IsCollection())
				{
					depth = AdjustDepth(depth);
				}
			}
			if ((config != null && (config.CascadeOnDelete() == Db4objects.Db4o.YapConst.YES 
				|| config.CascadeOnUpdate() == Db4objects.Db4o.YapConst.YES)))
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

		internal virtual int CheckUpdateDepthUnspecified(Db4objects.Db4o.YapStream a_stream
			)
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

		public virtual object Coerce(Db4objects.Db4o.Reflect.IReflectClass claxx, object 
			obj)
		{
			return CanHold(claxx) ? obj : Db4objects.Db4o.Foundation.No4.INSTANCE;
		}

		internal virtual void CollectConstraints(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.QConObject
			 a_parent, object a_object, Db4objects.Db4o.Foundation.IVisitor4 a_visitor)
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

		internal Db4objects.Db4o.TreeInt CollectFieldIDs(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes attributes, Db4objects.Db4o.TreeInt
			 tree, Db4objects.Db4o.YapWriter a_bytes, string name)
		{
			return mf._object.CollectFieldIDs(tree, this, attributes, a_bytes, name);
		}

		internal bool ConfigInstantiates()
		{
			return i_config != null && i_config.Instantiates();
		}

		public virtual Db4objects.Db4o.Config4Class ConfigOrAncestorConfig()
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

		private bool CreateConstructor(Db4objects.Db4o.YapStream a_stream, string a_name)
		{
			Db4objects.Db4o.Reflect.IReflectClass claxx;
			try
			{
				claxx = a_stream.Reflector().ForName(a_name);
			}
			catch (System.Exception t)
			{
				claxx = null;
			}
			return CreateConstructor(a_stream, claxx, a_name, true);
		}

		public virtual bool CreateConstructor(Db4objects.Db4o.YapStream a_stream, Db4objects.Db4o.Reflect.IReflectClass
			 a_class, string a_name, bool errMessages)
		{
			_reflector = a_class;
			_eventDispatcher = Db4objects.Db4o.EventDispatcher.ForClass(a_stream, a_class);
			if (ConfigInstantiates())
			{
				return true;
			}
			if (a_class != null)
			{
				if (a_stream.i_handlers.ICLASS_TRANSIENTCLASS.IsAssignableFrom(a_class) || Db4objects.Db4o.Platform4
					.IsTransient(a_class))
				{
					a_class = null;
				}
			}
			if (a_class == null)
			{
				if (a_name == null || a_name.IndexOf("com.db4o") != 0)
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
				throw new Db4objects.Db4o.Ext.ObjectNotStorableException(a_class);
			}
			return false;
		}

		public virtual void Deactivate(Db4objects.Db4o.Transaction a_trans, object a_object
			, int a_depth)
		{
			if (ObjectCanDeactivate(a_trans.Stream(), a_object))
			{
				Deactivate1(a_trans, a_object, a_depth);
				ObjectOnDeactivate(a_trans.Stream(), a_object);
			}
		}

		private void ObjectOnDeactivate(Db4objects.Db4o.YapStream stream, object obj)
		{
			stream.Callbacks().ObjectOnDeactivate(obj);
			DispatchEvent(stream, obj, Db4objects.Db4o.EventDispatcher.DEACTIVATE);
		}

		private bool ObjectCanDeactivate(Db4objects.Db4o.YapStream stream, object obj)
		{
			return stream.Callbacks().ObjectCanDeactivate(obj) && DispatchEvent(stream, obj, 
				Db4objects.Db4o.EventDispatcher.CAN_DEACTIVATE);
		}

		internal virtual void Deactivate1(Db4objects.Db4o.Transaction a_trans, object a_object
			, int a_depth)
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

		internal void Delete(Db4objects.Db4o.YapWriter a_bytes, object a_object)
		{
			Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
				(i_stream, this, a_bytes);
			Delete1(oh._marshallerFamily, oh._headerAttributes, a_bytes, a_object);
		}

		private void Delete1(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter a_bytes, object a_object)
		{
			RemoveFromIndex(a_bytes.GetTransaction(), a_bytes.GetID());
			DeleteMembers(mf, attributes, a_bytes, a_bytes.GetTransaction().Stream().i_handlers
				.ArrayType(a_object), false);
		}

		public virtual void DeleteEmbedded(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_bytes)
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

		public virtual void DeleteEmbedded1(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_bytes, int a_id)
		{
			if (a_bytes.CascadeDeletes() > 0)
			{
				Db4objects.Db4o.YapStream stream = a_bytes.GetStream();
				object obj = stream.GetByID2(a_bytes.GetTransaction(), a_id);
				int cascade = a_bytes.CascadeDeletes() - 1;
				if (obj != null)
				{
					if (IsCollection(obj))
					{
						cascade += Reflector().CollectionUpdateDepth(Reflector().ForObject(obj)) - 1;
					}
				}
				Db4objects.Db4o.YapObject yo = stream.GetYapObject(a_id);
				if (yo != null)
				{
					a_bytes.GetStream().Delete3(a_bytes.GetTransaction(), yo, obj, cascade, false);
				}
			}
		}

		internal virtual void DeleteMembers(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes attributes, Db4objects.Db4o.YapWriter
			 a_bytes, int a_type, bool isUpdate)
		{
			try
			{
				Db4objects.Db4o.Config4Class config = ConfigOrAncestorConfig();
				if (config != null && (config.CascadeOnDelete() == Db4objects.Db4o.YapConst.YES))
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
			catch (System.Exception e)
			{
			}
		}

		public bool DispatchEvent(Db4objects.Db4o.YapStream stream, object obj, int message
			)
		{
			if (_eventDispatcher != null && stream.DispatchsEvents())
			{
				return _eventDispatcher.Dispatch(stream, obj, message);
			}
			return true;
		}

		public bool Equals(Db4objects.Db4o.ITypeHandler4 a_dataType)
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

		internal Db4objects.Db4o.Inside.Marshall.MarshallerFamily FindOffset(Db4objects.Db4o.YapReader
			 a_bytes, Db4objects.Db4o.YapField a_field)
		{
			if (a_bytes == null)
			{
				return null;
			}
			a_bytes._offset = 0;
			Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
				(i_stream, this, a_bytes);
			bool res = oh.ObjectMarshaller().FindOffset(this, oh._headerAttributes, a_bytes, 
				a_field);
			if (!res)
			{
				return null;
			}
			return oh._marshallerFamily;
		}

		internal virtual void ForEachYapField(Db4objects.Db4o.Foundation.IVisitor4 visitor
			)
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
				i_ancestor.ForEachYapField(visitor);
			}
		}

		public static Db4objects.Db4o.YapClass ForObject(Db4objects.Db4o.Transaction trans
			, object obj, bool allowCreation)
		{
			Db4objects.Db4o.Reflect.IReflectClass reflectClass = trans.Reflector().ForObject(
				obj);
			if (reflectClass != null && reflectClass.GetSuperclass() == null && obj != null)
			{
				throw new Db4objects.Db4o.Ext.ObjectNotStorableException(obj.ToString());
			}
			return trans.Stream().GetYapClass(reflectClass, allowCreation);
		}

		public virtual bool GenerateUUIDs()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			int configValue = (i_config == null) ? 0 : i_config.GenerateUUIDs();
			return Generate1(i_stream.Config().GenerateUUIDs(), configValue);
		}

		private bool GenerateVersionNumbers()
		{
			if (!GenerateVirtual())
			{
				return false;
			}
			int configValue = (i_config == null) ? 0 : i_config.GenerateVersionNumbers();
			return Generate1(i_stream.Config().GenerateVersionNumbers(), configValue);
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
			if (!(i_stream is Db4objects.Db4o.YapFile))
			{
				return false;
			}
			return true;
		}

		private bool Generate1(int bootRecordValue, int configValue)
		{
			if (bootRecordValue < 0)
			{
				return false;
			}
			if (configValue < 0)
			{
				return false;
			}
			if (bootRecordValue > 1)
			{
				return true;
			}
			return configValue > 0;
		}

		internal virtual Db4objects.Db4o.YapClass GetAncestor()
		{
			return i_ancestor;
		}

		internal virtual object GetComparableObject(object forObject)
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

		internal virtual Db4objects.Db4o.YapClass GetHigherHierarchy(Db4objects.Db4o.YapClass
			 a_yapClass)
		{
			Db4objects.Db4o.YapClass yc = GetHigherHierarchy1(a_yapClass);
			if (yc != null)
			{
				return yc;
			}
			return a_yapClass.GetHigherHierarchy1(this);
		}

		private Db4objects.Db4o.YapClass GetHigherHierarchy1(Db4objects.Db4o.YapClass a_yapClass
			)
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

		internal virtual Db4objects.Db4o.YapClass GetHigherOrCommonHierarchy(Db4objects.Db4o.YapClass
			 a_yapClass)
		{
			Db4objects.Db4o.YapClass yc = GetHigherHierarchy1(a_yapClass);
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
			return Db4objects.Db4o.YapConst.YAPCLASS;
		}

		public virtual long[] GetIDs()
		{
			lock (i_stream.i_lock)
			{
				if (!StateOK())
				{
					return new long[0];
				}
				return GetIDs(i_stream.GetTransaction());
			}
		}

		public virtual long[] GetIDs(Db4objects.Db4o.Transaction trans)
		{
			if (!StateOK())
			{
				return new long[0];
			}
			if (!HasIndex())
			{
				return new long[0];
			}
			if (trans.Stream().IsClient())
			{
				return GetClientIDs(trans);
			}
			return GetIndexIDs(trans);
		}

		private long[] GetIndexIDs(Db4objects.Db4o.Transaction trans)
		{
			Db4objects.Db4o.Foundation.IntArrayList ids = new Db4objects.Db4o.Foundation.IntArrayList
				();
			_index.TraverseAll(trans, new _AnonymousInnerClass761(this, ids));
			return ids.AsLong();
		}

		private sealed class _AnonymousInnerClass761 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass761(YapClass _enclosing, Db4objects.Db4o.Foundation.IntArrayList
				 ids)
			{
				this._enclosing = _enclosing;
				this.ids = ids;
			}

			public void Visit(object obj)
			{
				ids.Add(((int)obj));
			}

			private readonly YapClass _enclosing;

			private readonly Db4objects.Db4o.Foundation.IntArrayList ids;
		}

		private long[] GetClientIDs(Db4objects.Db4o.Transaction trans)
		{
			Db4objects.Db4o.YapClient stream = (Db4objects.Db4o.YapClient)trans.Stream();
			stream.WriteMsg(Db4objects.Db4o.Msg.GET_INTERNAL_IDS.GetWriterForInt(trans, GetID
				()));
			Db4objects.Db4o.YapReader reader = stream.ExpectedByteResponse(Db4objects.Db4o.Msg
				.ID_LIST);
			int size = reader.ReadInt();
			long[] ids = new long[size];
			for (int i = 0; i < size; i++)
			{
				ids[i] = reader.ReadInt();
			}
			return ids;
		}

		public virtual bool HasIndex()
		{
			return i_db4oType == null || i_db4oType.HasClassIndex();
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
			if (i_fields != null)
			{
				for (int i = 0; i < i_fields.Length; i++)
				{
					if (i_fields[i] is Db4objects.Db4o.YapFieldUUID)
					{
						return true;
					}
				}
			}
			return false;
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
			if (i_fields != null)
			{
				for (int i = 0; i < i_fields.Length; i++)
				{
					if (i_fields[i] is Db4objects.Db4o.YapFieldVersion)
					{
						return true;
					}
				}
			}
			return false;
		}

		public virtual Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy Index()
		{
			return _index;
		}

		internal virtual int IndexEntryCount(Db4objects.Db4o.Transaction ta)
		{
			if (!StateOK())
			{
				return 0;
			}
			return _index.EntryCount(ta);
		}

		public virtual object IndexEntryToObject(Db4objects.Db4o.Transaction trans, object
			 indexEntry)
		{
			if (indexEntry == null)
			{
				return null;
			}
			int id = ((int)indexEntry);
			return GetStream().GetByID2(trans, id);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ClassReflector()
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

		public virtual Db4objects.Db4o.Ext.IStoredClass GetParentStoredClass()
		{
			return GetAncestor();
		}

		public virtual Db4objects.Db4o.Ext.IStoredField[] GetStoredFields()
		{
			lock (i_stream.i_lock)
			{
				if (i_fields == null)
				{
					return null;
				}
				Db4objects.Db4o.Ext.IStoredField[] fields = new Db4objects.Db4o.Ext.IStoredField[
					i_fields.Length];
				System.Array.Copy(i_fields, 0, fields, 0, i_fields.Length);
				return fields;
			}
		}

		internal virtual Db4objects.Db4o.YapStream GetStream()
		{
			return i_stream;
		}

		public virtual int GetTypeID()
		{
			return Db4objects.Db4o.YapConst.TYPE_CLASS;
		}

		public virtual Db4objects.Db4o.YapClass GetYapClass(Db4objects.Db4o.YapStream a_stream
			)
		{
			return this;
		}

		public virtual Db4objects.Db4o.YapField GetYapField(string name)
		{
			Db4objects.Db4o.YapField[] yf = new Db4objects.Db4o.YapField[1];
			ForEachYapField(new _AnonymousInnerClass888(this, name, yf));
			return yf[0];
		}

		private sealed class _AnonymousInnerClass888 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass888(YapClass _enclosing, string name, Db4objects.Db4o.YapField[]
				 yf)
			{
				this._enclosing = _enclosing;
				this.name = name;
				this.yf = yf;
			}

			public void Visit(object obj)
			{
				if (name.Equals(((Db4objects.Db4o.YapField)obj).GetName()))
				{
					yf[0] = (Db4objects.Db4o.YapField)obj;
				}
			}

			private readonly YapClass _enclosing;

			private readonly string name;

			private readonly Db4objects.Db4o.YapField[] yf;
		}

		public virtual bool HasFixedLength()
		{
			return true;
		}

		public virtual bool HasField(Db4objects.Db4o.YapStream a_stream, string a_field)
		{
			if (ClassReflector().IsCollection())
			{
				return true;
			}
			return GetYapField(a_field) != null;
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

		internal virtual void IncrementFieldsOffset1(Db4objects.Db4o.YapReader a_bytes)
		{
			int length = Db4objects.Db4o.Debug.atHome ? ReadFieldCountSodaAtHome(a_bytes) : ReadFieldCount
				(a_bytes);
			for (int i = 0; i < length; i++)
			{
				i_fields[i].IncrementOffset(a_bytes);
			}
		}

		public virtual object ComparableObject(Db4objects.Db4o.Transaction a_trans, object
			 a_object)
		{
			return a_object;
		}

		internal bool Init(Db4objects.Db4o.YapStream a_stream, Db4objects.Db4o.YapClass a_ancestor
			, Db4objects.Db4o.Reflect.IReflectClass claxx)
		{
			i_ancestor = a_ancestor;
			Db4objects.Db4o.Config4Impl config = a_stream.ConfigImpl();
			string className = claxx.GetName();
			SetConfig(config.ConfigClass(className));
			if (!CreateConstructor(a_stream, claxx, className, false))
			{
				return false;
			}
			CheckDb4oType();
			if (AllowsQueries())
			{
				_index.Initialize(a_stream);
			}
			i_name = className;
			i_ancestor = a_ancestor;
			BitTrue(Db4objects.Db4o.YapConst.CHECKED_CHANGES);
			return true;
		}

		internal void InitConfigOnUp(Db4objects.Db4o.Transaction systemTrans)
		{
			Db4objects.Db4o.Config4Class extendedConfig = Db4objects.Db4o.Platform4.ExtendConfiguration
				(_reflector, i_stream.Configure(), i_config);
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
				i_fields[i].InitConfigOnUp(systemTrans);
			}
		}

		internal virtual void InitOnUp(Db4objects.Db4o.Transaction systemTrans)
		{
			if (!StateOK())
			{
				return;
			}
			InitConfigOnUp(systemTrans);
			StoreStaticFieldValues(systemTrans, false);
		}

		internal virtual object Instantiate(Db4objects.Db4o.YapObject a_yapObject, object
			 a_object, Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter a_bytes, bool a_addToIDTree)
		{
			Db4objects.Db4o.YapStream stream = a_bytes.GetStream();
			Db4objects.Db4o.Transaction trans = a_bytes.GetTransaction();
			bool create = (a_object == null);
			if (i_config != null)
			{
				a_bytes.SetInstantiationDepth(i_config.AdjustActivationDepth(a_bytes.GetInstantiationDepth
					()));
			}
			bool doFields = (a_bytes.GetInstantiationDepth() > 0) || (i_config != null && (i_config
				.CascadeOnActivate() == Db4objects.Db4o.YapConst.YES));
			if (create)
			{
				if (ConfigInstantiates())
				{
					int bytesOffset = a_bytes._offset;
					a_bytes.IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
					try
					{
						a_object = i_config.Instantiate(stream, i_fields[0].Read(mf, a_bytes));
					}
					catch (System.Exception e)
					{
						Db4objects.Db4o.Messages.LogErr(stream.ConfigImpl(), 6, ClassReflector().GetName(
							), e);
						return null;
					}
					a_bytes._offset = bytesOffset;
				}
				else
				{
					if (_reflector == null)
					{
						return null;
					}
					stream.Instantiating(true);
					try
					{
						a_object = _reflector.NewInstance();
					}
					catch (System.MissingMethodException e)
					{
						stream.LogMsg(7, ClassReflector().GetName());
						stream.Instantiating(false);
						return null;
					}
					catch (System.Exception e)
					{
						stream.Instantiating(false);
						return null;
					}
					stream.Instantiating(false);
				}
				if (a_object is Db4objects.Db4o.ITransactionAware)
				{
					((Db4objects.Db4o.ITransactionAware)a_object).SetTrans(a_bytes.GetTransaction());
				}
				if (a_object is Db4objects.Db4o.IDb4oTypeImpl)
				{
					((Db4objects.Db4o.IDb4oTypeImpl)a_object).SetYapObject(a_yapObject);
				}
				a_yapObject.SetObjectWeak(stream, a_object);
				stream.HcTreeAdd(a_yapObject);
			}
			else
			{
				if (!stream.i_refreshInsteadOfActivate)
				{
					if (a_yapObject.IsActive())
					{
						doFields = false;
					}
				}
			}
			if (a_addToIDTree)
			{
				a_yapObject.AddToIDTree(stream);
			}
			if (doFields)
			{
				if (ObjectCanActivate(stream, a_object))
				{
					a_yapObject.SetStateClean();
					InstantiateFields(a_yapObject, a_object, mf, attributes, a_bytes);
					ObjectOnActivate(stream, a_object);
				}
				else
				{
					if (create)
					{
						a_yapObject.SetStateDeactivated();
					}
				}
			}
			else
			{
				if (create)
				{
					a_yapObject.SetStateDeactivated();
				}
				else
				{
					if (a_bytes.GetInstantiationDepth() > 1)
					{
						ActivateFields(trans, a_object, a_bytes.GetInstantiationDepth() - 1);
					}
				}
			}
			return a_object;
		}

		private void ObjectOnActivate(Db4objects.Db4o.YapStream stream, object obj)
		{
			stream.Callbacks().ObjectOnActivate(obj);
			DispatchEvent(stream, obj, Db4objects.Db4o.EventDispatcher.ACTIVATE);
		}

		private bool ObjectCanActivate(Db4objects.Db4o.YapStream stream, object obj)
		{
			return stream.Callbacks().ObjectCanActivate(obj) && DispatchEvent(stream, obj, Db4objects.Db4o.EventDispatcher
				.CAN_ACTIVATE);
		}

		internal virtual object InstantiateTransient(Db4objects.Db4o.YapObject a_yapObject
			, object a_object, Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter a_bytes)
		{
			Db4objects.Db4o.YapStream stream = a_bytes.GetStream();
			if (ConfigInstantiates())
			{
				int bytesOffset = a_bytes._offset;
				a_bytes.IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
				try
				{
					a_object = i_config.Instantiate(stream, i_fields[0].Read(mf, a_bytes));
				}
				catch (System.Exception e)
				{
					Db4objects.Db4o.Messages.LogErr(stream.ConfigImpl(), 6, ClassReflector().GetName(
						), e);
					return null;
				}
				a_bytes._offset = bytesOffset;
			}
			else
			{
				if (_reflector == null)
				{
					return null;
				}
				stream.Instantiating(true);
				try
				{
					a_object = _reflector.NewInstance();
				}
				catch (System.MissingMethodException e)
				{
					stream.LogMsg(7, ClassReflector().GetName());
					stream.Instantiating(false);
					return null;
				}
				catch (System.Exception e)
				{
					stream.Instantiating(false);
					return null;
				}
				stream.Instantiating(false);
			}
			stream.Peeked(a_yapObject.GetID(), a_object);
			InstantiateFields(a_yapObject, a_object, mf, attributes, a_bytes);
			return a_object;
		}

		internal virtual void InstantiateFields(Db4objects.Db4o.YapObject a_yapObject, object
			 a_onObject, Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 attributes, Db4objects.Db4o.YapWriter a_bytes)
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

		public virtual int IsSecondClass()
		{
			return Db4objects.Db4o.YapConst.NO;
		}

		/// <summary>no any, primitive, array or other tricks.</summary>
		/// <remarks>
		/// no any, primitive, array or other tricks. overriden in YapClassAny and
		/// YapClassPrimitive
		/// </remarks>
		internal virtual bool IsStrongTyped()
		{
			return true;
		}

		internal virtual bool IsValueType()
		{
			return Db4objects.Db4o.Platform4.IsValueType(ClassReflector());
		}

		public virtual void CalculateLengths(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Inside.Marshall.ObjectHeaderAttributes
			 header, bool topLevel, object obj, bool withIndirection)
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
				return "";
			}
			return i_name;
		}

		internal bool CallConstructor()
		{
			i_dontCallConstructors = !CallConstructor1();
			return !i_dontCallConstructors;
		}

		private bool CallConstructor1()
		{
			int res = CallConstructorSpecialized();
			if (res != Db4objects.Db4o.YapConst.DEFAULT)
			{
				return res == Db4objects.Db4o.YapConst.YES;
			}
			return (i_stream.ConfigImpl().CallConstructors() == Db4objects.Db4o.YapConst.YES);
		}

		private int CallConstructorSpecialized()
		{
			if (i_config != null)
			{
				int res = i_config.CallConstructor();
				if (res != Db4objects.Db4o.YapConst.DEFAULT)
				{
					return res;
				}
			}
			if (_isEnum)
			{
				return Db4objects.Db4o.YapConst.NO;
			}
			if (i_ancestor != null)
			{
				return i_ancestor.CallConstructorSpecialized();
			}
			return Db4objects.Db4o.YapConst.DEFAULT;
		}

		public override int OwnLength()
		{
			return Db4objects.Db4o.Inside.Marshall.MarshallerFamily.Current()._class.MarshalledLength
				(i_stream, this);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass PrimitiveClassReflector()
		{
			return null;
		}

		internal virtual void Purge()
		{
			_index.Purge();
		}

		public virtual object Read(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapWriter
			 a_bytes, bool redirect)
		{
			try
			{
				int id = a_bytes.ReadInt();
				int depth = a_bytes.GetInstantiationDepth() - 1;
				Db4objects.Db4o.Transaction trans = a_bytes.GetTransaction();
				Db4objects.Db4o.YapStream stream = trans.Stream();
				if (a_bytes.GetUpdateDepth() == Db4objects.Db4o.YapConst.TRANSIENT)
				{
					return stream.PeekPersisted1(trans, id, depth);
				}
				if (IsValueType())
				{
					if (depth < 1)
					{
						depth = 1;
					}
					Db4objects.Db4o.YapObject yo = stream.GetYapObject(id);
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
					return new Db4objects.Db4o.YapObject(id).Read(trans, null, null, depth, Db4objects.Db4o.YapConst
						.ADD_TO_ID_TREE, false);
				}
				object ret = stream.GetByID2(trans, id);
				if (ret is Db4objects.Db4o.IDb4oTypeImpl)
				{
					depth = ((Db4objects.Db4o.IDb4oTypeImpl)ret).AdjustReadDepth(depth);
				}
				stream.StillToActivate(ret, depth);
				return ret;
			}
			catch (System.Exception e)
			{
			}
			return null;
		}

		public virtual object ReadQuery(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, bool withRedirection, Db4objects.Db4o.YapReader a_reader, bool a_toArray)
		{
			try
			{
				return a_trans.Stream().GetByID2(a_trans, a_reader.ReadInt());
			}
			catch (System.Exception e)
			{
			}
			return null;
		}

		public virtual Db4objects.Db4o.ITypeHandler4 ReadArrayHandler(Db4objects.Db4o.Transaction
			 a_trans, Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.YapReader[]
			 a_bytes)
		{
			if (IsArray())
			{
				return this;
			}
			return null;
		}

		public virtual Db4objects.Db4o.ITypeHandler4 ReadArrayHandler1(Db4objects.Db4o.YapReader[]
			 a_bytes)
		{
			if (IsArray())
			{
				if (Db4objects.Db4o.Platform4.IsCollectionTranslator(this.i_config))
				{
					a_bytes[0].IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
					return new Db4objects.Db4o.YapArray(i_stream, null, false);
				}
				IncrementFieldsOffset1(a_bytes[0]);
				if (i_ancestor != null)
				{
					return i_ancestor.ReadArrayHandler1(a_bytes);
				}
			}
			return null;
		}

		public virtual Db4objects.Db4o.QCandidate ReadSubCandidate(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapReader reader, Db4objects.Db4o.QCandidates candidates, bool
			 withIndirection)
		{
			int id = reader.ReadInt();
			if (id == 0)
			{
				return null;
			}
			return new Db4objects.Db4o.QCandidate(candidates, null, id, true);
		}

		public virtual void ReadCandidates(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapReader a_bytes, Db4objects.Db4o.QCandidates a_candidates
			)
		{
			int id = 0;
			int offset = a_bytes._offset;
			try
			{
				id = a_bytes.ReadInt();
			}
			catch (System.Exception e)
			{
			}
			a_bytes._offset = offset;
			if (id != 0)
			{
				Db4objects.Db4o.Transaction trans = a_candidates.i_trans;
				object obj = trans.Stream().GetByID1(trans, id);
				if (obj != null)
				{
					a_candidates.i_trans.Stream().Activate1(trans, obj, 2);
					Db4objects.Db4o.Platform4.ForEachCollectionElement(obj, new _AnonymousInnerClass1365
						(this, a_candidates, trans));
				}
			}
		}

		private sealed class _AnonymousInnerClass1365 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass1365(YapClass _enclosing, Db4objects.Db4o.QCandidates 
				a_candidates, Db4objects.Db4o.Transaction trans)
			{
				this._enclosing = _enclosing;
				this.a_candidates = a_candidates;
				this.trans = trans;
			}

			public void Visit(object elem)
			{
				a_candidates.AddByIdentity(new Db4objects.Db4o.QCandidate(a_candidates, elem, (int
					)trans.Stream().GetID(elem), true));
			}

			private readonly YapClass _enclosing;

			private readonly Db4objects.Db4o.QCandidates a_candidates;

			private readonly Db4objects.Db4o.Transaction trans;
		}

		public int ReadFieldCount(Db4objects.Db4o.YapReader a_bytes)
		{
			int count = a_bytes.ReadInt();
			if (count > i_fields.Length)
			{
				return i_fields.Length;
			}
			return count;
		}

		public virtual int ReadFieldCountSodaAtHome(Db4objects.Db4o.YapReader a_bytes)
		{
			return 0;
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.YapReader a_reader)
		{
			return a_reader.ReadInt();
		}

		public virtual object ReadIndexEntry(Db4objects.Db4o.Inside.Marshall.MarshallerFamily
			 mf, Db4objects.Db4o.YapWriter a_writer)
		{
			return ReadIndexEntry(a_writer);
		}

		internal virtual byte[] ReadName(Db4objects.Db4o.Transaction a_trans)
		{
			i_reader = a_trans.Stream().ReadReaderByID(a_trans, GetID());
			if (i_reader != null)
			{
				return ReadName1(a_trans, i_reader);
			}
			return null;
		}

		internal byte[] ReadName1(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 reader)
		{
			i_reader = reader;
			try
			{
				Db4objects.Db4o.Inside.Marshall.ClassMarshaller marshaller = Db4objects.Db4o.Inside.Marshall.MarshallerFamily
					.Current()._class;
				i_nameBytes = marshaller.ReadName(trans, reader);
				_metaClassID = marshaller.ReadMetaClassID(reader);
				SetStateUnread();
				BitFalse(Db4objects.Db4o.YapConst.CHECKED_CHANGES);
				BitFalse(Db4objects.Db4o.YapConst.STATIC_FIELDS_STORED);
				return i_nameBytes;
			}
			catch (System.Exception t)
			{
				SetStateDead();
			}
			return null;
		}

		internal virtual void ReadVirtualAttributes(Db4objects.Db4o.Transaction a_trans, 
			Db4objects.Db4o.YapObject a_yapObject)
		{
			int id = a_yapObject.GetID();
			Db4objects.Db4o.YapStream stream = a_trans.Stream();
			Db4objects.Db4o.YapReader reader = stream.ReadReaderByID(a_trans, id);
			Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
				(stream, this, reader);
			oh.ObjectMarshaller().ReadVirtualAttributes(a_trans, this, a_yapObject, oh._headerAttributes
				, reader);
		}

		internal virtual Db4objects.Db4o.Reflect.Generic.GenericReflector Reflector()
		{
			return i_stream.Reflector();
		}

		public virtual void Rename(string newName)
		{
			if (!i_stream.IsClient())
			{
				int tempState = i_state;
				SetStateOK();
				i_name = newName;
				SetStateDirty();
				Write(i_stream.GetSystemTransaction());
				i_state = tempState;
			}
			else
			{
				Db4objects.Db4o.Inside.Exceptions4.ThrowRuntimeException(58);
			}
		}

		internal virtual void CreateConfigAndConstructor(Db4objects.Db4o.Foundation.Hashtable4
			 a_byteHashTable, Db4objects.Db4o.YapStream a_stream, Db4objects.Db4o.Reflect.IReflectClass
			 a_class)
		{
			if (a_class == null)
			{
				if (i_nameBytes != null)
				{
					i_name = a_stream.StringIO().Read(i_nameBytes);
				}
			}
			else
			{
				i_name = a_class.GetName();
			}
			SetConfig(i_stream.ConfigImpl().ConfigClass(i_name));
			if (a_class == null)
			{
				CreateConstructor(a_stream, i_name);
			}
			else
			{
				CreateConstructor(a_stream, a_class, i_name, true);
			}
			if (i_nameBytes != null)
			{
				a_byteHashTable.Remove(i_nameBytes);
				i_nameBytes = null;
			}
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
			if (i_reader == null || BitIsTrue(Db4objects.Db4o.YapConst.READING))
			{
				return;
			}
			BitTrue(Db4objects.Db4o.YapConst.READING);
			Db4objects.Db4o.Inside.Marshall.MarshallerFamily.ForConverterVersion(i_stream.ConverterVersion
				())._class.Read(i_stream, this, i_reader);
			i_nameBytes = null;
			i_reader = null;
			BitFalse(Db4objects.Db4o.YapConst.READING);
		}

		public virtual bool ReadArray(object array, Db4objects.Db4o.YapReader reader)
		{
			return false;
		}

		public override void ReadThis(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapReader
			 a_reader)
		{
			throw Db4objects.Db4o.Inside.Exceptions4.VirtualException();
		}

		internal virtual void Refresh()
		{
			if (!StateUnread())
			{
				CreateConstructor(i_stream, i_name);
				BitFalse(Db4objects.Db4o.YapConst.CHECKED_CHANGES);
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

		internal virtual void RemoveFromIndex(Db4objects.Db4o.Transaction ta, int id)
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
					i_stream.LogMsg(9, "class:" + GetName() + " field:" + a_to);
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

		internal virtual void SetConfig(Db4objects.Db4o.Config4Class config)
		{
			if (i_config == null)
			{
				i_config = config;
			}
		}

		internal virtual void SetName(string a_name)
		{
			i_name = a_name;
		}

		private void SetStateDead()
		{
			BitTrue(Db4objects.Db4o.YapConst.DEAD);
			BitFalse(Db4objects.Db4o.YapConst.CONTINUE);
		}

		private void SetStateUnread()
		{
			BitFalse(Db4objects.Db4o.YapConst.DEAD);
			BitTrue(Db4objects.Db4o.YapConst.CONTINUE);
		}

		private void SetStateOK()
		{
			BitFalse(Db4objects.Db4o.YapConst.DEAD);
			BitFalse(Db4objects.Db4o.YapConst.CONTINUE);
		}

		internal virtual bool StateDead()
		{
			return BitIsTrue(Db4objects.Db4o.YapConst.DEAD);
		}

		private bool StateOK()
		{
			return BitIsFalse(Db4objects.Db4o.YapConst.CONTINUE) && BitIsFalse(Db4objects.Db4o.YapConst
				.DEAD) && BitIsFalse(Db4objects.Db4o.YapConst.READING);
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
			return BitIsTrue(Db4objects.Db4o.YapConst.CONTINUE) && BitIsFalse(Db4objects.Db4o.YapConst
				.DEAD) && BitIsFalse(Db4objects.Db4o.YapConst.READING);
		}

		internal virtual bool StoreField(Db4objects.Db4o.Reflect.IReflectField a_field)
		{
			if (a_field.IsStatic())
			{
				return false;
			}
			if (a_field.IsTransient())
			{
				Db4objects.Db4o.Config4Class config = ConfigOrAncestorConfig();
				if (config == null)
				{
					return false;
				}
				if (!config.StoreTransientFields())
				{
					return false;
				}
			}
			return Db4objects.Db4o.Platform4.CanSetAccessible() || a_field.IsPublic();
		}

		public virtual Db4objects.Db4o.Ext.IStoredField StoredField(string a_name, object
			 a_type)
		{
			lock (i_stream.i_lock)
			{
				Db4objects.Db4o.YapClass yc = i_stream.GetYapClass(i_stream.ConfigImpl().ReflectorFor
					(a_type), false);
				if (i_fields != null)
				{
					for (int i = 0; i < i_fields.Length; i++)
					{
						if (i_fields[i].GetName().Equals(a_name))
						{
							if (yc == null || yc == i_fields[i].GetFieldYapClass(i_stream))
							{
								return (i_fields[i]);
							}
						}
					}
				}
				return null;
			}
		}

		internal virtual void StoreStaticFieldValues(Db4objects.Db4o.Transaction trans, bool
			 force)
		{
			if (!BitIsTrue(Db4objects.Db4o.YapConst.STATIC_FIELDS_STORED) || force)
			{
				BitTrue(Db4objects.Db4o.YapConst.STATIC_FIELDS_STORED);
				bool store = (i_config != null && i_config.StaticFieldValuesArePersisted()) || Db4objects.Db4o.Platform4
					.StoreStaticFieldValues(trans.Reflector(), ClassReflector());
				if (store)
				{
					Db4objects.Db4o.YapStream stream = trans.Stream();
					stream.ShowInternalClasses(true);
					Db4objects.Db4o.Query.IQuery q = stream.Query(trans);
					q.Constrain(Db4objects.Db4o.YapConst.CLASS_STATICCLASS);
					q.Descend("name").Constrain(i_name);
					Db4objects.Db4o.StaticClass sc = new Db4objects.Db4o.StaticClass();
					sc.name = i_name;
					Db4objects.Db4o.IObjectSet os = q.Execute();
					Db4objects.Db4o.StaticField[] oldFields = null;
					if (os.Size() > 0)
					{
						sc = (Db4objects.Db4o.StaticClass)os.Next();
						stream.Activate1(trans, sc, 4);
						oldFields = sc.fields;
					}
					Db4objects.Db4o.Reflect.IReflectField[] fields = ClassReflector().GetDeclaredFields
						();
					Db4objects.Db4o.Foundation.Collection4 newFields = new Db4objects.Db4o.Foundation.Collection4
						();
					for (int i = 0; i < fields.Length; i++)
					{
						if (fields[i].IsStatic())
						{
							fields[i].SetAccessible();
							string fieldName = fields[i].GetName();
							object value = fields[i].Get(null);
							bool handled = false;
							if (oldFields != null)
							{
								for (int j = 0; j < oldFields.Length; j++)
								{
									if (fieldName.Equals(oldFields[j].name))
									{
										if (oldFields[j].value != null && value != null && oldFields[j].value.GetType() ==
											 value.GetType())
										{
											long id = stream.GetID1(trans, oldFields[j].value);
											if (id > 0)
											{
												if (oldFields[j].value != value)
												{
													stream.Bind1(trans, value, id);
													stream.Refresh(value, int.MaxValue);
													oldFields[j].value = value;
												}
												handled = true;
											}
										}
										if (!handled)
										{
											if (value == null)
											{
												try
												{
													fields[i].Set(null, oldFields[j].value);
												}
												catch (System.Exception ex)
												{
												}
											}
											else
											{
												oldFields[j].value = value;
												if (!stream.IsClient())
												{
													stream.SetInternal(trans, oldFields[j], true);
												}
											}
										}
										newFields.Add(oldFields[j]);
										handled = true;
										break;
									}
								}
							}
							if (!handled)
							{
								newFields.Add(new Db4objects.Db4o.StaticField(fieldName, value));
							}
						}
					}
					if (newFields.Size() > 0)
					{
						sc.fields = new Db4objects.Db4o.StaticField[newFields.Size()];
						newFields.ToArray(sc.fields);
						if (!stream.IsClient())
						{
							stream.SetInternal(trans, sc, true);
						}
					}
					stream.ShowInternalClasses(false);
				}
			}
		}

		public virtual bool SupportsIndex()
		{
			return true;
		}

		public override string ToString()
		{
			if (i_name == null && i_nameBytes != null)
			{
				Db4objects.Db4o.YapStringIO stringIO = i_stream == null ? Db4objects.Db4o.YapConst
					.stringIO : i_stream.StringIO();
				return stringIO.Read(i_nameBytes);
			}
			return i_name;
		}

		public virtual bool WriteArray(object array, Db4objects.Db4o.YapReader reader)
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

		public virtual void WriteIndexEntry(Db4objects.Db4o.YapReader a_writer, object a_object
			)
		{
			if (a_object == null)
			{
				a_writer.WriteInt(0);
				return;
			}
			a_writer.WriteInt(((int)a_object));
		}

		public virtual object WriteNew(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf
			, object a_object, bool topLevel, Db4objects.Db4o.YapWriter a_bytes, bool withIndirection
			, bool restoreLinkOffset)
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

		public sealed override void WriteThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 writer)
		{
			Db4objects.Db4o.Inside.Marshall.MarshallerFamily.Current()._class.Write(trans, this
				, writer);
		}

		private Db4objects.Db4o.Reflect.IReflectClass i_compareTo;

		public virtual void PrepareComparison(Db4objects.Db4o.Transaction a_trans, object
			 obj)
		{
			PrepareComparison(obj);
		}

		public virtual Db4objects.Db4o.IYapComparable PrepareComparison(object obj)
		{
			if (obj != null)
			{
				if (obj is int)
				{
					i_lastID = ((int)obj);
				}
				else
				{
					i_lastID = (int)i_stream.GetID(obj);
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

		public virtual string ToString(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf
			, Db4objects.Db4o.YapWriter writer, Db4objects.Db4o.YapObject yapObject, int depth
			, int maxDepth)
		{
			int length = ReadFieldCount(writer);
			string str = "";
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

		public virtual void DefragObject(Db4objects.Db4o.ReaderPair readers)
		{
			Db4objects.Db4o.Inside.Marshall.ObjectHeader header = Db4objects.Db4o.Inside.Marshall.ObjectHeader
				.Defrag(this, readers);
			header._marshallerFamily._object.DefragFields(this, header, readers);
		}

		public virtual void Defrag(Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf, Db4objects.Db4o.ReaderPair
			 readers)
		{
			readers.CopyID();
			int restLength = (LinkLength() - Db4objects.Db4o.YapConst.INT_LENGTH);
			readers.IncrementOffset(restLength);
		}

		public virtual void DefragClass(Db4objects.Db4o.ReaderPair readers, int classIndexID
			)
		{
			Db4objects.Db4o.Inside.Marshall.MarshallerFamily mf = Db4objects.Db4o.Inside.Marshall.MarshallerFamily
				.Current();
			mf._class.Defrag(this, i_stream.StringIO(), readers, classIndexID);
		}

		public static Db4objects.Db4o.YapClass ReadClass(Db4objects.Db4o.YapStream stream
			, Db4objects.Db4o.YapReader reader)
		{
			Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
				(stream, reader);
			return oh.YapClass();
		}

		public virtual bool IsAssignableFrom(Db4objects.Db4o.YapClass other)
		{
			return ClassReflector().IsAssignableFrom(other.ClassReflector());
		}

		public void DefragIndexEntry(Db4objects.Db4o.ReaderPair readers)
		{
			readers.CopyID();
		}
	}
}
