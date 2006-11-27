namespace Db4objects.Db4o
{
	/// <renameto>ObjectReference</renameto>
	/// <exclude></exclude>
	public class YapObject : Db4objects.Db4o.YapMeta, Db4objects.Db4o.Ext.IObjectInfo
	{
		private Db4objects.Db4o.YapClass _class;

		private object _object;

		private Db4objects.Db4o.VirtualAttributes _virtualAttributes;

		private Db4objects.Db4o.YapObject id_preceding;

		private Db4objects.Db4o.YapObject id_subsequent;

		private int id_size;

		private Db4objects.Db4o.YapObject hc_preceding;

		private Db4objects.Db4o.YapObject hc_subsequent;

		private int hc_size;

		private int hc_code;

		private int _lastTopLevelCallId;

		public YapObject()
		{
		}

		public YapObject(int a_id)
		{
			i_id = a_id;
		}

		internal YapObject(Db4objects.Db4o.YapClass a_yapClass, int a_id)
		{
			_class = a_yapClass;
			i_id = a_id;
		}

		internal virtual void Activate(Db4objects.Db4o.Transaction ta, object a_object, int
			 a_depth, bool a_refresh)
		{
			Activate1(ta, a_object, a_depth, a_refresh);
			ta.Stream().Activate3CheckStill(ta);
		}

		internal virtual void Activate1(Db4objects.Db4o.Transaction ta, object a_object, 
			int a_depth, bool a_refresh)
		{
			if (a_object is Db4objects.Db4o.IDb4oTypeImpl)
			{
				a_depth = ((Db4objects.Db4o.IDb4oTypeImpl)a_object).AdjustReadDepth(a_depth);
			}
			if (a_depth > 0)
			{
				Db4objects.Db4o.YapStream stream = ta.Stream();
				if (a_refresh)
				{
					LogActivation(stream, "refresh");
				}
				else
				{
					if (IsActive())
					{
						if (a_object != null)
						{
							if (a_depth > 1)
							{
								if (_class.i_config != null)
								{
									a_depth = _class.i_config.AdjustActivationDepth(a_depth);
								}
								_class.ActivateFields(ta, a_object, a_depth);
							}
							return;
						}
					}
					LogActivation(stream, "activate");
				}
				Read(ta, null, a_object, a_depth, Db4objects.Db4o.YapConst.ADD_MEMBERS_TO_ID_TREE_ONLY
					, false);
			}
		}

		private void LogActivation(Db4objects.Db4o.YapStream stream, string @event)
		{
			LogEvent(stream, @event, Db4objects.Db4o.YapConst.ACTIVATION);
		}

		private void LogEvent(Db4objects.Db4o.YapStream stream, string @event, int level)
		{
			if (stream.ConfigImpl().MessageLevel() > level)
			{
				stream.Message(string.Empty + GetID() + " " + @event + " " + _class.GetName());
			}
		}

		internal void AddToIDTree(Db4objects.Db4o.YapStream a_stream)
		{
			if (!(_class is Db4objects.Db4o.YapClassPrimitive))
			{
				a_stream.IdTreeAdd(this);
			}
		}

		/// <summary>return false if class not completely initialized, otherwise true *</summary>
		internal virtual bool ContinueSet(Db4objects.Db4o.Transaction a_trans, int a_updateDepth
			)
		{
			if (BitIsTrue(Db4objects.Db4o.YapConst.CONTINUE))
			{
				if (!_class.StateOKAndAncestors())
				{
					return false;
				}
				BitFalse(Db4objects.Db4o.YapConst.CONTINUE);
				Db4objects.Db4o.YapWriter writer = Db4objects.Db4o.Inside.Marshall.MarshallerFamily
					.Current()._object.MarshallNew(a_trans, this, a_updateDepth);
				Db4objects.Db4o.YapStream stream = a_trans.Stream();
				stream.WriteNew(_class, writer);
				object obj = _object;
				ObjectOnNew(stream, obj);
				if (!_class.IsPrimitive())
				{
					_object = stream.i_references.CreateYapRef(this, obj);
				}
				SetStateClean();
				EndProcessing();
			}
			return true;
		}

		private void ObjectOnNew(Db4objects.Db4o.YapStream stream, object obj)
		{
			stream.Callbacks().ObjectOnNew(obj);
			_class.DispatchEvent(stream, obj, Db4objects.Db4o.EventDispatcher.NEW);
		}

		internal virtual void Deactivate(Db4objects.Db4o.Transaction a_trans, int a_depth
			)
		{
			if (a_depth > 0)
			{
				object obj = GetObject();
				if (obj != null)
				{
					if (obj is Db4objects.Db4o.IDb4oTypeImpl)
					{
						((Db4objects.Db4o.IDb4oTypeImpl)obj).PreDeactivate();
					}
					Db4objects.Db4o.YapStream stream = a_trans.Stream();
					LogActivation(stream, "deactivate");
					SetStateDeactivated();
					_class.Deactivate(a_trans, obj, a_depth);
				}
			}
		}

		public override byte GetIdentifier()
		{
			return Db4objects.Db4o.YapConst.YAPOBJECT;
		}

		public virtual object GetObject()
		{
			if (Db4objects.Db4o.Platform4.HasWeakReferences())
			{
				return Db4objects.Db4o.Platform4.GetYapRefObject(_object);
			}
			return _object;
		}

		public virtual object GetObjectReference()
		{
			return _object;
		}

		public virtual Db4objects.Db4o.YapStream GetStream()
		{
			if (_class == null)
			{
				return null;
			}
			return _class.GetStream();
		}

		public virtual Db4objects.Db4o.Transaction GetTrans()
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			if (stream != null)
			{
				return stream.GetTransaction();
			}
			return null;
		}

		public virtual Db4objects.Db4o.Ext.Db4oUUID GetUUID()
		{
			Db4objects.Db4o.VirtualAttributes va = VirtualAttributes(GetTrans());
			if (va != null && va.i_database != null)
			{
				return new Db4objects.Db4o.Ext.Db4oUUID(va.i_uuid, va.i_database.i_signature);
			}
			return null;
		}

		public virtual long GetVersion()
		{
			Db4objects.Db4o.VirtualAttributes va = VirtualAttributes(GetTrans());
			if (va == null)
			{
				return 0;
			}
			return va.i_version;
		}

		public virtual Db4objects.Db4o.YapClass GetYapClass()
		{
			return _class;
		}

		public override int OwnLength()
		{
			throw Db4objects.Db4o.Inside.Exceptions4.ShouldNeverBeCalled();
		}

		public virtual Db4objects.Db4o.VirtualAttributes ProduceVirtualAttributes()
		{
			if (_virtualAttributes == null)
			{
				_virtualAttributes = new Db4objects.Db4o.VirtualAttributes();
			}
			return _virtualAttributes;
		}

		internal object Read(Db4objects.Db4o.Transaction ta, Db4objects.Db4o.YapWriter a_reader
			, object a_object, int a_instantiationDepth, int addToIDTree, bool checkIDTree)
		{
			if (BeginProcessing())
			{
				Db4objects.Db4o.YapStream stream = ta.Stream();
				if (a_reader == null)
				{
					a_reader = stream.ReadWriterByID(ta, GetID());
				}
				if (a_reader != null)
				{
					Db4objects.Db4o.Inside.Marshall.ObjectHeader header = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
						(stream, a_reader);
					_class = header.YapClass();
					if (_class == null)
					{
						return null;
					}
					if (checkIDTree)
					{
						object objectInCacheFromClassCreation = stream.ObjectForIDFromCache(GetID());
						if (objectInCacheFromClassCreation != null)
						{
							return objectInCacheFromClassCreation;
						}
					}
					a_reader.SetInstantiationDepth(a_instantiationDepth);
					a_reader.SetUpdateDepth(addToIDTree);
					if (addToIDTree == Db4objects.Db4o.YapConst.TRANSIENT)
					{
						a_object = _class.InstantiateTransient(this, a_object, header._marshallerFamily, 
							header._headerAttributes, a_reader);
					}
					else
					{
						a_object = _class.Instantiate(this, a_object, header._marshallerFamily, header._headerAttributes
							, a_reader, addToIDTree == Db4objects.Db4o.YapConst.ADD_TO_ID_TREE);
					}
				}
				EndProcessing();
			}
			return a_object;
		}

		public object ReadPrefetch(Db4objects.Db4o.YapStream a_stream, Db4objects.Db4o.YapWriter
			 a_reader)
		{
			object readObject = null;
			if (BeginProcessing())
			{
				Db4objects.Db4o.Inside.Marshall.ObjectHeader header = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
					(a_stream, a_reader);
				_class = header.YapClass();
				if (_class == null)
				{
					return null;
				}
				a_reader.SetInstantiationDepth(_class.ConfigOrAncestorConfig() == null ? 1 : 0);
				readObject = _class.Instantiate(this, GetObject(), header._marshallerFamily, header
					._headerAttributes, a_reader, true);
				EndProcessing();
			}
			return readObject;
		}

		public sealed override void ReadThis(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.YapReader
			 a_bytes)
		{
		}

		internal virtual void SetObjectWeak(Db4objects.Db4o.YapStream a_stream, object a_object
			)
		{
			if (a_stream.i_references._weak)
			{
				if (_object != null)
				{
					Db4objects.Db4o.Platform4.KillYapRef(_object);
				}
				_object = Db4objects.Db4o.Platform4.CreateYapRef(a_stream.i_references._queue, this
					, a_object);
			}
			else
			{
				_object = a_object;
			}
		}

		public virtual void SetObject(object a_object)
		{
			_object = a_object;
		}

		internal void Store(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClass yapClass
			, object obj)
		{
			_object = obj;
			_class = yapClass;
			WriteObjectBegin();
			SetID(trans.Stream().NewUserObject());
			BeginProcessing();
			BitTrue(Db4objects.Db4o.YapConst.CONTINUE);
		}

		public virtual void FlagForDelete(int callId)
		{
			_lastTopLevelCallId = -callId;
		}

		public virtual bool IsFlaggedForDelete()
		{
			return _lastTopLevelCallId < 0;
		}

		public virtual void FlagAsHandled(int callId)
		{
			_lastTopLevelCallId = callId;
		}

		public bool IsFlaggedAsHandled(int callID)
		{
			return _lastTopLevelCallId == callID;
		}

		public virtual Db4objects.Db4o.VirtualAttributes VirtualAttributes()
		{
			return _virtualAttributes;
		}

		public virtual Db4objects.Db4o.VirtualAttributes VirtualAttributes(Db4objects.Db4o.Transaction
			 a_trans)
		{
			if (a_trans == null)
			{
				return _virtualAttributes;
			}
			if (_virtualAttributes == null)
			{
				if (_class.HasVirtualAttributes())
				{
					_virtualAttributes = new Db4objects.Db4o.VirtualAttributes();
					_class.ReadVirtualAttributes(a_trans, this);
				}
			}
			else
			{
				if (!_virtualAttributes.SuppliesUUID())
				{
					if (_class.HasVirtualAttributes())
					{
						_class.ReadVirtualAttributes(a_trans, this);
					}
				}
			}
			return _virtualAttributes;
		}

		public virtual void SetVirtualAttributes(Db4objects.Db4o.VirtualAttributes at)
		{
			_virtualAttributes = at;
		}

		public override void WriteThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 a_writer)
		{
		}

		internal virtual void WriteUpdate(Db4objects.Db4o.Transaction a_trans, int a_updatedepth
			)
		{
			ContinueSet(a_trans, a_updatedepth);
			if (BeginProcessing())
			{
				object obj = GetObject();
				if (ObjectCanUpdate(a_trans.Stream(), obj))
				{
					if ((!IsActive()) || obj == null)
					{
						EndProcessing();
						return;
					}
					LogEvent(a_trans.Stream(), "update", Db4objects.Db4o.YapConst.STATE);
					SetStateClean();
					a_trans.WriteUpdateDeleteMembers(GetID(), _class, a_trans.Stream().i_handlers.ArrayType
						(obj), 0);
					Db4objects.Db4o.Inside.Marshall.MarshallerFamily.Current()._object.MarshallUpdate
						(a_trans, a_updatedepth, this, obj);
				}
				else
				{
					EndProcessing();
				}
			}
		}

		private bool ObjectCanUpdate(Db4objects.Db4o.YapStream stream, object obj)
		{
			return stream.Callbacks().ObjectCanUpdate(obj) && _class.DispatchEvent(stream, obj
				, Db4objects.Db4o.EventDispatcher.CAN_UPDATE);
		}

		/// <summary>HCTREE ****</summary>
		public virtual Db4objects.Db4o.YapObject Hc_add(Db4objects.Db4o.YapObject a_add)
		{
			object obj = a_add.GetObject();
			if (obj != null)
			{
				a_add.Hc_init(obj);
				return Hc_add1(a_add);
			}
			return this;
		}

		public virtual void Hc_init(object obj)
		{
			hc_preceding = null;
			hc_subsequent = null;
			hc_size = 1;
			hc_code = Hc_getCode(obj);
		}

		private Db4objects.Db4o.YapObject Hc_add1(Db4objects.Db4o.YapObject a_new)
		{
			int cmp = Hc_compare(a_new);
			if (cmp < 0)
			{
				if (hc_preceding == null)
				{
					hc_preceding = a_new;
					hc_size++;
				}
				else
				{
					hc_preceding = hc_preceding.Hc_add1(a_new);
					if (hc_subsequent == null)
					{
						return Hc_rotateRight();
					}
					return Hc_balance();
				}
			}
			else
			{
				if (hc_subsequent == null)
				{
					hc_subsequent = a_new;
					hc_size++;
				}
				else
				{
					hc_subsequent = hc_subsequent.Hc_add1(a_new);
					if (hc_preceding == null)
					{
						return Hc_rotateLeft();
					}
					return Hc_balance();
				}
			}
			return this;
		}

		private Db4objects.Db4o.YapObject Hc_balance()
		{
			int cmp = hc_subsequent.hc_size - hc_preceding.hc_size;
			if (cmp < -2)
			{
				return Hc_rotateRight();
			}
			else
			{
				if (cmp > 2)
				{
					return Hc_rotateLeft();
				}
				else
				{
					hc_size = hc_preceding.hc_size + hc_subsequent.hc_size + 1;
					return this;
				}
			}
		}

		private void Hc_calculateSize()
		{
			if (hc_preceding == null)
			{
				if (hc_subsequent == null)
				{
					hc_size = 1;
				}
				else
				{
					hc_size = hc_subsequent.hc_size + 1;
				}
			}
			else
			{
				if (hc_subsequent == null)
				{
					hc_size = hc_preceding.hc_size + 1;
				}
				else
				{
					hc_size = hc_preceding.hc_size + hc_subsequent.hc_size + 1;
				}
			}
		}

		private int Hc_compare(Db4objects.Db4o.YapObject a_to)
		{
			int cmp = a_to.hc_code - hc_code;
			if (cmp == 0)
			{
				cmp = a_to.i_id - i_id;
			}
			return cmp;
		}

		public virtual Db4objects.Db4o.YapObject Hc_find(object obj)
		{
			return Hc_find(Hc_getCode(obj), obj);
		}

		private Db4objects.Db4o.YapObject Hc_find(int a_id, object obj)
		{
			int cmp = a_id - hc_code;
			if (cmp < 0)
			{
				if (hc_preceding != null)
				{
					return hc_preceding.Hc_find(a_id, obj);
				}
			}
			else
			{
				if (cmp > 0)
				{
					if (hc_subsequent != null)
					{
						return hc_subsequent.Hc_find(a_id, obj);
					}
				}
				else
				{
					if (obj == GetObject())
					{
						return this;
					}
					if (hc_preceding != null)
					{
						Db4objects.Db4o.YapObject inPreceding = hc_preceding.Hc_find(a_id, obj);
						if (inPreceding != null)
						{
							return inPreceding;
						}
					}
					if (hc_subsequent != null)
					{
						return hc_subsequent.Hc_find(a_id, obj);
					}
				}
			}
			return null;
		}

		private int Hc_getCode(object obj)
		{
			int hcode = Sharpen.Runtime.IdentityHashCode(obj);
			if (hcode < 0)
			{
				hcode = ~hcode;
			}
			return hcode;
		}

		private Db4objects.Db4o.YapObject Hc_rotateLeft()
		{
			Db4objects.Db4o.YapObject tree = hc_subsequent;
			hc_subsequent = tree.hc_preceding;
			Hc_calculateSize();
			tree.hc_preceding = this;
			if (tree.hc_subsequent == null)
			{
				tree.hc_size = 1 + hc_size;
			}
			else
			{
				tree.hc_size = 1 + hc_size + tree.hc_subsequent.hc_size;
			}
			return tree;
		}

		private Db4objects.Db4o.YapObject Hc_rotateRight()
		{
			Db4objects.Db4o.YapObject tree = hc_preceding;
			hc_preceding = tree.hc_subsequent;
			Hc_calculateSize();
			tree.hc_subsequent = this;
			if (tree.hc_preceding == null)
			{
				tree.hc_size = 1 + hc_size;
			}
			else
			{
				tree.hc_size = 1 + hc_size + tree.hc_preceding.hc_size;
			}
			return tree;
		}

		private Db4objects.Db4o.YapObject Hc_rotateSmallestUp()
		{
			if (hc_preceding != null)
			{
				hc_preceding = hc_preceding.Hc_rotateSmallestUp();
				return Hc_rotateRight();
			}
			return this;
		}

		internal virtual Db4objects.Db4o.YapObject Hc_remove(Db4objects.Db4o.YapObject a_find
			)
		{
			if (this == a_find)
			{
				return Hc_remove();
			}
			int cmp = Hc_compare(a_find);
			if (cmp <= 0)
			{
				if (hc_preceding != null)
				{
					hc_preceding = hc_preceding.Hc_remove(a_find);
				}
			}
			if (cmp >= 0)
			{
				if (hc_subsequent != null)
				{
					hc_subsequent = hc_subsequent.Hc_remove(a_find);
				}
			}
			Hc_calculateSize();
			return this;
		}

		public virtual void Hc_traverse(Db4objects.Db4o.Foundation.IVisitor4 visitor)
		{
			if (hc_preceding != null)
			{
				hc_preceding.Hc_traverse(visitor);
			}
			visitor.Visit(this);
			if (hc_subsequent != null)
			{
				hc_subsequent.Hc_traverse(visitor);
			}
		}

		private Db4objects.Db4o.YapObject Hc_remove()
		{
			if (hc_subsequent != null && hc_preceding != null)
			{
				hc_subsequent = hc_subsequent.Hc_rotateSmallestUp();
				hc_subsequent.hc_preceding = hc_preceding;
				hc_subsequent.Hc_calculateSize();
				return hc_subsequent;
			}
			if (hc_subsequent != null)
			{
				return hc_subsequent;
			}
			return hc_preceding;
		}

		/// <summary>IDTREE ****</summary>
		internal virtual Db4objects.Db4o.YapObject Id_add(Db4objects.Db4o.YapObject a_add
			)
		{
			a_add.id_preceding = null;
			a_add.id_subsequent = null;
			a_add.id_size = 1;
			return Id_add1(a_add);
		}

		private Db4objects.Db4o.YapObject Id_add1(Db4objects.Db4o.YapObject a_new)
		{
			int cmp = a_new.i_id - i_id;
			if (cmp < 0)
			{
				if (id_preceding == null)
				{
					id_preceding = a_new;
					id_size++;
				}
				else
				{
					id_preceding = id_preceding.Id_add1(a_new);
					if (id_subsequent == null)
					{
						return Id_rotateRight();
					}
					return Id_balance();
				}
			}
			else
			{
				if (cmp > 0)
				{
					if (id_subsequent == null)
					{
						id_subsequent = a_new;
						id_size++;
					}
					else
					{
						id_subsequent = id_subsequent.Id_add1(a_new);
						if (id_preceding == null)
						{
							return Id_rotateLeft();
						}
						return Id_balance();
					}
				}
			}
			return this;
		}

		private Db4objects.Db4o.YapObject Id_balance()
		{
			int cmp = id_subsequent.id_size - id_preceding.id_size;
			if (cmp < -2)
			{
				return Id_rotateRight();
			}
			else
			{
				if (cmp > 2)
				{
					return Id_rotateLeft();
				}
				else
				{
					id_size = id_preceding.id_size + id_subsequent.id_size + 1;
					return this;
				}
			}
		}

		private void Id_calculateSize()
		{
			if (id_preceding == null)
			{
				if (id_subsequent == null)
				{
					id_size = 1;
				}
				else
				{
					id_size = id_subsequent.id_size + 1;
				}
			}
			else
			{
				if (id_subsequent == null)
				{
					id_size = id_preceding.id_size + 1;
				}
				else
				{
					id_size = id_preceding.id_size + id_subsequent.id_size + 1;
				}
			}
		}

		internal virtual Db4objects.Db4o.YapObject Id_find(int a_id)
		{
			int cmp = a_id - i_id;
			if (cmp > 0)
			{
				if (id_subsequent != null)
				{
					return id_subsequent.Id_find(a_id);
				}
			}
			else
			{
				if (cmp < 0)
				{
					if (id_preceding != null)
					{
						return id_preceding.Id_find(a_id);
					}
				}
				else
				{
					return this;
				}
			}
			return null;
		}

		private Db4objects.Db4o.YapObject Id_rotateLeft()
		{
			Db4objects.Db4o.YapObject tree = id_subsequent;
			id_subsequent = tree.id_preceding;
			Id_calculateSize();
			tree.id_preceding = this;
			if (tree.id_subsequent == null)
			{
				tree.id_size = id_size + 1;
			}
			else
			{
				tree.id_size = id_size + 1 + tree.id_subsequent.id_size;
			}
			return tree;
		}

		private Db4objects.Db4o.YapObject Id_rotateRight()
		{
			Db4objects.Db4o.YapObject tree = id_preceding;
			id_preceding = tree.id_subsequent;
			Id_calculateSize();
			tree.id_subsequent = this;
			if (tree.id_preceding == null)
			{
				tree.id_size = id_size + 1;
			}
			else
			{
				tree.id_size = id_size + 1 + tree.id_preceding.id_size;
			}
			return tree;
		}

		private Db4objects.Db4o.YapObject Id_rotateSmallestUp()
		{
			if (id_preceding != null)
			{
				id_preceding = id_preceding.Id_rotateSmallestUp();
				return Id_rotateRight();
			}
			return this;
		}

		internal virtual Db4objects.Db4o.YapObject Id_remove(int a_id)
		{
			int cmp = a_id - i_id;
			if (cmp < 0)
			{
				if (id_preceding != null)
				{
					id_preceding = id_preceding.Id_remove(a_id);
				}
			}
			else
			{
				if (cmp > 0)
				{
					if (id_subsequent != null)
					{
						id_subsequent = id_subsequent.Id_remove(a_id);
					}
				}
				else
				{
					return Id_remove();
				}
			}
			Id_calculateSize();
			return this;
		}

		private Db4objects.Db4o.YapObject Id_remove()
		{
			if (id_subsequent != null && id_preceding != null)
			{
				id_subsequent = id_subsequent.Id_rotateSmallestUp();
				id_subsequent.id_preceding = id_preceding;
				id_subsequent.Id_calculateSize();
				return id_subsequent;
			}
			if (id_subsequent != null)
			{
				return id_subsequent;
			}
			return id_preceding;
		}

		public override string ToString()
		{
			return base.ToString();
			try
			{
				int id = GetID();
				string str = "YapObject\nID=" + id;
				if (_class != null)
				{
					Db4objects.Db4o.YapStream stream = _class.GetStream();
					if (stream != null && id > 0)
					{
						Db4objects.Db4o.YapWriter writer = stream.ReadWriterByID(stream.GetTransaction(), 
							id);
						if (writer != null)
						{
							str += "\nAddress=" + writer.GetAddress();
						}
						Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
							(stream, writer);
						Db4objects.Db4o.YapClass yc = oh.YapClass();
						if (yc != _class)
						{
							str += "\nYapClass corruption";
						}
						else
						{
							str += yc.ToString(oh._marshallerFamily, writer, this, 0, 5);
						}
					}
				}
				object obj = GetObject();
				if (obj == null)
				{
					str += "\nfor [null]";
				}
				else
				{
					string objToString = string.Empty;
					try
					{
						objToString = obj.ToString();
					}
					catch
					{
					}
					Db4objects.Db4o.Reflect.IReflectClass claxx = GetYapClass().Reflector().ForObject
						(obj);
					str += "\n" + claxx.GetName() + "\n" + objToString;
				}
				return str;
			}
			catch
			{
			}
			return "Exception in YapObject analyzer";
		}
	}
}
