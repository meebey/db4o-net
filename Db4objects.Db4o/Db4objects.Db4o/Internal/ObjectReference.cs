/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;
using Sharpen;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class ObjectReference : PersistentBase, IObjectInfo, IActivator
	{
		private ClassMetadata _class;

		private object _object;

		private Db4objects.Db4o.Internal.VirtualAttributes _virtualAttributes;

		private Db4objects.Db4o.Internal.ObjectReference id_preceding;

		private Db4objects.Db4o.Internal.ObjectReference id_subsequent;

		private int id_size;

		private Db4objects.Db4o.Internal.ObjectReference hc_preceding;

		private Db4objects.Db4o.Internal.ObjectReference hc_subsequent;

		private int hc_size;

		private int hc_code;

		private int _lastTopLevelCallId;

		public ObjectReference()
		{
		}

		public ObjectReference(int a_id)
		{
			i_id = a_id;
		}

		internal ObjectReference(ClassMetadata a_yapClass, int a_id)
		{
			_class = a_yapClass;
			i_id = a_id;
		}

		public virtual void Activate()
		{
			if (IsActive())
			{
				return;
			}
			Activate(Stream().GetTransaction(), GetObject(), 1, false);
		}

		private ObjectContainerBase Stream()
		{
			return _class.Stream();
		}

		public virtual void Activate(Transaction ta, object a_object, int a_depth, bool a_refresh
			)
		{
			Activate1(ta, a_object, a_depth, a_refresh);
			ta.Stream().Activate3CheckStill(ta);
		}

		internal virtual void Activate1(Transaction ta, object a_object, int a_depth, bool
			 a_refresh)
		{
			if (a_object is IDb4oTypeImpl)
			{
				a_depth = ((IDb4oTypeImpl)a_object).AdjustReadDepth(a_depth);
			}
			if (a_depth > 0)
			{
				ObjectContainerBase stream = ta.Stream();
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
								if (_class.Config() != null)
								{
									a_depth = _class.Config().AdjustActivationDepth(a_depth);
								}
								_class.ActivateFields(ta, a_object, a_depth);
							}
							return;
						}
					}
					LogActivation(stream, "activate");
				}
				Read(ta, null, a_object, a_depth, Const4.ADD_MEMBERS_TO_ID_TREE_ONLY, false);
			}
		}

		private void LogActivation(ObjectContainerBase stream, string @event)
		{
			LogEvent(stream, @event, Const4.ACTIVATION);
		}

		private void LogEvent(ObjectContainerBase stream, string @event, int level)
		{
			if (stream.ConfigImpl().MessageLevel() > level)
			{
				stream.Message(string.Empty + GetID() + " " + @event + " " + _class.GetName());
			}
		}

		internal void AddExistingReferenceToIdTree(ObjectContainerBase a_stream)
		{
			if (!(_class is PrimitiveFieldHandler))
			{
				a_stream.ReferenceSystem().AddExistingReferenceToIdTree(this);
			}
		}

		/// <summary>return false if class not completely initialized, otherwise true *</summary>
		internal virtual bool ContinueSet(Transaction a_trans, int a_updateDepth)
		{
			if (BitIsTrue(Const4.CONTINUE))
			{
				if (!_class.StateOKAndAncestors())
				{
					return false;
				}
				BitFalse(Const4.CONTINUE);
				StatefulBuffer writer = MarshallerFamily.Current()._object.MarshallNew(a_trans, this
					, a_updateDepth);
				ObjectContainerBase stream = a_trans.Stream();
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

		private void ObjectOnNew(ObjectContainerBase stream, object obj)
		{
			stream.Callbacks().ObjectOnNew(obj);
			_class.DispatchEvent(stream, obj, EventDispatcher.NEW);
		}

		public virtual void Deactivate(Transaction a_trans, int a_depth)
		{
			if (a_depth > 0)
			{
				object obj = GetObject();
				if (obj != null)
				{
					if (obj is IDb4oTypeImpl)
					{
						((IDb4oTypeImpl)obj).PreDeactivate();
					}
					ObjectContainerBase stream = a_trans.Stream();
					LogActivation(stream, "deactivate");
					SetStateDeactivated();
					_class.Deactivate(a_trans, obj, a_depth);
				}
			}
		}

		public override byte GetIdentifier()
		{
			return Const4.YAPOBJECT;
		}

		public virtual long GetInternalID()
		{
			return GetID();
		}

		public virtual object GetObject()
		{
			if (Platform4.HasWeakReferences())
			{
				return Platform4.GetYapRefObject(_object);
			}
			return _object;
		}

		public virtual object GetObjectReference()
		{
			return _object;
		}

		public virtual ObjectContainerBase GetStream()
		{
			if (_class == null)
			{
				return null;
			}
			return _class.GetStream();
		}

		public virtual Transaction GetTrans()
		{
			ObjectContainerBase stream = GetStream();
			if (stream != null)
			{
				return stream.GetTransaction();
			}
			return null;
		}

		public virtual Db4oUUID GetUUID()
		{
			Db4objects.Db4o.Internal.VirtualAttributes va = VirtualAttributes(GetTrans());
			if (va != null && va.i_database != null)
			{
				return new Db4oUUID(va.i_uuid, va.i_database.i_signature);
			}
			return null;
		}

		public virtual long GetVersion()
		{
			Db4objects.Db4o.Internal.VirtualAttributes va = VirtualAttributes(GetTrans());
			if (va == null)
			{
				return 0;
			}
			return va.i_version;
		}

		public virtual ClassMetadata GetYapClass()
		{
			return _class;
		}

		public override int OwnLength()
		{
			throw Exceptions4.ShouldNeverBeCalled();
		}

		public virtual Db4objects.Db4o.Internal.VirtualAttributes ProduceVirtualAttributes
			()
		{
			if (_virtualAttributes == null)
			{
				_virtualAttributes = new Db4objects.Db4o.Internal.VirtualAttributes();
			}
			return _virtualAttributes;
		}

		internal object PeekPersisted(Transaction trans, int depth)
		{
			return Read(trans, depth, Const4.TRANSIENT, false);
		}

		internal object Read(Transaction trans, int instantiationDepth, int addToIDTree, 
			bool checkIDTree)
		{
			return Read(trans, null, null, instantiationDepth, addToIDTree, checkIDTree);
		}

		internal object Read(Transaction ta, StatefulBuffer a_reader, object a_object, int
			 a_instantiationDepth, int addToIDTree, bool checkIDTree)
		{
			if (BeginProcessing())
			{
				ObjectContainerBase stream = ta.Stream();
				int id = GetID();
				if (a_reader == null && id > 0)
				{
					a_reader = stream.ReadWriterByID(ta, id);
				}
				if (a_reader != null)
				{
					ObjectHeader header = new ObjectHeader(stream, a_reader);
					_class = header.YapClass();
					if (_class == null)
					{
						return null;
					}
					if (checkIDTree)
					{
						object objectInCacheFromClassCreation = stream.ObjectForIdFromCache(GetID());
						if (objectInCacheFromClassCreation != null)
						{
							return objectInCacheFromClassCreation;
						}
					}
					a_reader.SetInstantiationDepth(a_instantiationDepth);
					a_reader.SetUpdateDepth(addToIDTree);
					if (addToIDTree == Const4.TRANSIENT)
					{
						a_object = _class.InstantiateTransient(this, a_object, header._marshallerFamily, 
							header._headerAttributes, a_reader);
					}
					else
					{
						a_object = _class.Instantiate(this, a_object, header._marshallerFamily, header._headerAttributes
							, a_reader, addToIDTree == Const4.ADD_TO_ID_TREE);
					}
				}
				EndProcessing();
			}
			return a_object;
		}

		public object ReadPrefetch(ObjectContainerBase a_stream, StatefulBuffer a_reader)
		{
			object readObject = null;
			if (BeginProcessing())
			{
				ObjectHeader header = new ObjectHeader(a_stream, a_reader);
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

		public sealed override void ReadThis(Transaction a_trans, Db4objects.Db4o.Internal.Buffer
			 a_bytes)
		{
		}

		internal virtual void SetObjectWeak(ObjectContainerBase a_stream, object a_object
			)
		{
			if (a_stream.i_references._weak)
			{
				if (_object != null)
				{
					Platform4.KillYapRef(_object);
				}
				_object = Platform4.CreateYapRef(a_stream.i_references._queue, this, a_object);
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

		internal void Store(Transaction trans, ClassMetadata yapClass, object obj)
		{
			_object = obj;
			_class = yapClass;
			WriteObjectBegin();
			int id = trans.Stream().NewUserObject();
			trans.SlotFreePointerOnRollback(id);
			SetID(id);
			BeginProcessing();
			BitTrue(Const4.CONTINUE);
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

		public bool IsValid()
		{
			return IsValidId(GetID()) && GetObject() != null;
		}

		public static bool IsValidId(int id)
		{
			return id > 0;
		}

		public virtual Db4objects.Db4o.Internal.VirtualAttributes VirtualAttributes()
		{
			return _virtualAttributes;
		}

		public virtual Db4objects.Db4o.Internal.VirtualAttributes VirtualAttributes(Transaction
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
					_virtualAttributes = new Db4objects.Db4o.Internal.VirtualAttributes();
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

		public virtual void SetVirtualAttributes(Db4objects.Db4o.Internal.VirtualAttributes
			 at)
		{
			_virtualAttributes = at;
		}

		public override void WriteThis(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 a_writer)
		{
		}

		public virtual void WriteUpdate(Transaction a_trans, int a_updatedepth)
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
					LogEvent(a_trans.Stream(), "update", Const4.STATE);
					SetStateClean();
					a_trans.WriteUpdateDeleteMembers(GetID(), _class, a_trans.Stream().i_handlers.ArrayType
						(obj), 0);
					MarshallerFamily.Current()._object.MarshallUpdate(a_trans, a_updatedepth, this, obj
						);
				}
				else
				{
					EndProcessing();
				}
			}
		}

		private bool ObjectCanUpdate(ObjectContainerBase stream, object obj)
		{
			return stream.Callbacks().ObjectCanUpdate(obj) && _class.DispatchEvent(stream, obj
				, EventDispatcher.CAN_UPDATE);
		}

		/// <summary>HCTREE ****</summary>
		public virtual Db4objects.Db4o.Internal.ObjectReference Hc_add(Db4objects.Db4o.Internal.ObjectReference
			 a_add)
		{
			if (a_add.GetObject() == null)
			{
				return this;
			}
			a_add.Hc_init();
			return Hc_add1(a_add);
		}

		public virtual void Hc_init()
		{
			hc_preceding = null;
			hc_subsequent = null;
			hc_size = 1;
			hc_code = Hc_getCode(GetObject());
		}

		private Db4objects.Db4o.Internal.ObjectReference Hc_add1(Db4objects.Db4o.Internal.ObjectReference
			 a_new)
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

		private Db4objects.Db4o.Internal.ObjectReference Hc_balance()
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

		private int Hc_compare(Db4objects.Db4o.Internal.ObjectReference a_to)
		{
			int cmp = a_to.hc_code - hc_code;
			if (cmp == 0)
			{
				cmp = a_to.i_id - i_id;
			}
			return cmp;
		}

		public virtual Db4objects.Db4o.Internal.ObjectReference Hc_find(object obj)
		{
			return Hc_find(Hc_getCode(obj), obj);
		}

		private Db4objects.Db4o.Internal.ObjectReference Hc_find(int a_id, object obj)
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
						Db4objects.Db4o.Internal.ObjectReference inPreceding = hc_preceding.Hc_find(a_id, 
							obj);
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
			int hcode = Runtime.IdentityHashCode(obj);
			if (hcode < 0)
			{
				hcode = ~hcode;
			}
			return hcode;
		}

		private Db4objects.Db4o.Internal.ObjectReference Hc_rotateLeft()
		{
			Db4objects.Db4o.Internal.ObjectReference tree = hc_subsequent;
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

		private Db4objects.Db4o.Internal.ObjectReference Hc_rotateRight()
		{
			Db4objects.Db4o.Internal.ObjectReference tree = hc_preceding;
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

		private Db4objects.Db4o.Internal.ObjectReference Hc_rotateSmallestUp()
		{
			if (hc_preceding != null)
			{
				hc_preceding = hc_preceding.Hc_rotateSmallestUp();
				return Hc_rotateRight();
			}
			return this;
		}

		internal virtual Db4objects.Db4o.Internal.ObjectReference Hc_remove(Db4objects.Db4o.Internal.ObjectReference
			 a_find)
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

		public virtual void Hc_traverse(IVisitor4 visitor)
		{
			if (hc_preceding != null)
			{
				hc_preceding.Hc_traverse(visitor);
			}
			if (hc_subsequent != null)
			{
				hc_subsequent.Hc_traverse(visitor);
			}
			visitor.Visit(this);
		}

		private Db4objects.Db4o.Internal.ObjectReference Hc_remove()
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
		internal virtual Db4objects.Db4o.Internal.ObjectReference Id_add(Db4objects.Db4o.Internal.ObjectReference
			 a_add)
		{
			a_add.id_preceding = null;
			a_add.id_subsequent = null;
			a_add.id_size = 1;
			return Id_add1(a_add);
		}

		private Db4objects.Db4o.Internal.ObjectReference Id_add1(Db4objects.Db4o.Internal.ObjectReference
			 a_new)
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

		private Db4objects.Db4o.Internal.ObjectReference Id_balance()
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

		internal virtual Db4objects.Db4o.Internal.ObjectReference Id_find(int a_id)
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

		private Db4objects.Db4o.Internal.ObjectReference Id_rotateLeft()
		{
			Db4objects.Db4o.Internal.ObjectReference tree = id_subsequent;
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

		private Db4objects.Db4o.Internal.ObjectReference Id_rotateRight()
		{
			Db4objects.Db4o.Internal.ObjectReference tree = id_preceding;
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

		private Db4objects.Db4o.Internal.ObjectReference Id_rotateSmallestUp()
		{
			if (id_preceding != null)
			{
				id_preceding = id_preceding.Id_rotateSmallestUp();
				return Id_rotateRight();
			}
			return this;
		}

		internal virtual Db4objects.Db4o.Internal.ObjectReference Id_remove(int a_id)
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

		private Db4objects.Db4o.Internal.ObjectReference Id_remove()
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
					ObjectContainerBase stream = _class.GetStream();
					if (stream != null && id > 0)
					{
						StatefulBuffer writer = stream.ReadWriterByID(stream.GetTransaction(), id);
						if (writer != null)
						{
							str += "\nAddress=" + writer.GetAddress();
						}
						ObjectHeader oh = new ObjectHeader(stream, writer);
						ClassMetadata yc = oh.YapClass();
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
					catch (Exception)
					{
					}
					IReflectClass claxx = GetYapClass().Reflector().ForObject(obj);
					str += "\n" + claxx.GetName() + "\n" + objToString;
				}
				return str;
			}
			catch (Exception)
			{
			}
			return "Exception in YapObject analyzer";
		}
	}
}
