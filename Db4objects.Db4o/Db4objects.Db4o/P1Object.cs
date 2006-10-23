namespace Db4objects.Db4o
{
	/// <summary>base class for all database aware objects</summary>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class P1Object : Db4objects.Db4o.IDb4oTypeImpl
	{
		[Db4objects.Db4o.Transient]
		private Db4objects.Db4o.Transaction i_trans;

		[Db4objects.Db4o.Transient]
		private Db4objects.Db4o.YapObject i_yapObject;

		public P1Object()
		{
		}

		internal P1Object(Db4objects.Db4o.Transaction a_trans)
		{
			i_trans = a_trans;
		}

		public virtual void Activate(object a_obj, int a_depth)
		{
			if (i_trans != null)
			{
				if (a_depth < 0)
				{
					i_trans.Stream().Activate1(i_trans, a_obj);
				}
				else
				{
					i_trans.Stream().Activate1(i_trans, a_obj, a_depth);
				}
			}
		}

		public virtual int ActivationDepth()
		{
			return 1;
		}

		public virtual int AdjustReadDepth(int a_depth)
		{
			return a_depth;
		}

		public virtual bool CanBind()
		{
			return false;
		}

		public virtual void CheckActive()
		{
			if (i_trans != null)
			{
				if (i_yapObject == null)
				{
					i_yapObject = i_trans.Stream().GetYapObject(this);
					if (i_yapObject == null)
					{
						i_trans.Stream().Set(this);
						i_yapObject = i_trans.Stream().GetYapObject(this);
					}
				}
				if (ValidYapObject())
				{
					i_yapObject.Activate(i_trans, this, ActivationDepth(), false);
				}
			}
		}

		public virtual object CreateDefault(Db4objects.Db4o.Transaction a_trans)
		{
			throw Db4objects.Db4o.Inside.Exceptions4.VirtualException();
		}

		internal virtual void Deactivate()
		{
			if (ValidYapObject())
			{
				i_yapObject.Deactivate(i_trans, ActivationDepth());
			}
		}

		internal virtual void Delete()
		{
			if (i_trans != null)
			{
				if (i_yapObject == null)
				{
					i_yapObject = i_trans.Stream().GetYapObject(this);
				}
				if (ValidYapObject())
				{
					i_trans.Stream().Delete3(i_trans, i_yapObject, this, 0, false);
				}
			}
		}

		protected virtual void Delete(object a_obj)
		{
			if (i_trans != null)
			{
				i_trans.Stream().Delete(a_obj);
			}
		}

		protected virtual long GetIDOf(object a_obj)
		{
			if (i_trans == null)
			{
				return 0;
			}
			return i_trans.Stream().GetID(a_obj);
		}

		protected virtual Db4objects.Db4o.Transaction GetTrans()
		{
			return i_trans;
		}

		public virtual bool HasClassIndex()
		{
			return false;
		}

		public virtual void PreDeactivate()
		{
		}

		protected virtual object Replicate(Db4objects.Db4o.Transaction fromTrans, Db4objects.Db4o.Transaction
			 toTrans)
		{
			Db4objects.Db4o.YapStream fromStream = fromTrans.Stream();
			Db4objects.Db4o.YapStream toStream = toTrans.Stream();
			Db4objects.Db4o.Inside.Replication.MigrationConnection mgc = fromStream.i_handlers
				.i_migration;
			lock (fromStream.Lock())
			{
				int id = toStream.OldReplicationHandles(this);
				if (id == -1)
				{
					return this;
				}
				if (id > 0)
				{
					return toStream.GetByID(id);
				}
				if (mgc != null)
				{
					object otherObj = mgc.IdentityFor(this);
					if (otherObj != null)
					{
						return otherObj;
					}
				}
				Db4objects.Db4o.P1Object replica = (Db4objects.Db4o.P1Object)CreateDefault(toTrans
					);
				if (mgc != null)
				{
					mgc.MapReference(replica, i_yapObject);
					mgc.MapIdentity(this, replica);
				}
				replica.Store(0);
				return replica;
			}
		}

		public virtual void ReplicateFrom(object obj)
		{
		}

		public virtual void SetTrans(Db4objects.Db4o.Transaction a_trans)
		{
			i_trans = a_trans;
		}

		public virtual void SetYapObject(Db4objects.Db4o.YapObject a_yapObject)
		{
			i_yapObject = a_yapObject;
		}

		protected virtual void Store(object a_obj)
		{
			if (i_trans != null)
			{
				i_trans.Stream().SetInternal(i_trans, a_obj, true);
			}
		}

		public virtual object StoredTo(Db4objects.Db4o.Transaction a_trans)
		{
			i_trans = a_trans;
			return this;
		}

		internal virtual object StreamLock()
		{
			if (i_trans != null)
			{
				i_trans.Stream().CheckClosed();
				return i_trans.Stream().Lock();
			}
			return this;
		}

		public virtual void Store(int a_depth)
		{
			if (i_trans != null)
			{
				if (i_yapObject == null)
				{
					i_yapObject = i_trans.Stream().GetYapObject(this);
					if (i_yapObject == null)
					{
						i_trans.Stream().SetInternal(i_trans, this, true);
						i_yapObject = i_trans.Stream().GetYapObject(this);
						return;
					}
				}
				Update(a_depth);
			}
		}

		internal virtual void Update()
		{
			Update(ActivationDepth());
		}

		internal virtual void Update(int depth)
		{
			if (ValidYapObject())
			{
				i_trans.Stream().BeginEndSet(i_trans);
				i_yapObject.WriteUpdate(i_trans, depth);
				i_trans.Stream().CheckStillToSet();
				i_trans.Stream().BeginEndSet(i_trans);
			}
		}

		internal virtual void UpdateInternal()
		{
			UpdateInternal(ActivationDepth());
		}

		internal virtual void UpdateInternal(int depth)
		{
			if (ValidYapObject())
			{
				i_yapObject.WriteUpdate(i_trans, depth);
				i_trans.Stream().RememberJustSet(i_yapObject.GetID());
				i_trans.Stream().CheckStillToSet();
			}
		}

		private bool ValidYapObject()
		{
			return (i_trans != null) && (i_yapObject != null) && (i_yapObject.GetID() > 0);
		}
	}
}