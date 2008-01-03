/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;

namespace Db4objects.Db4o
{
	/// <summary>base class for all database aware objects</summary>
	/// <exclude></exclude>
	/// <persistent></persistent>
	public class P1Object : IDb4oTypeImpl
	{
		[System.NonSerialized]
		private Transaction i_trans;

		[System.NonSerialized]
		private ObjectReference i_yapObject;

		public P1Object()
		{
		}

		internal P1Object(Transaction a_trans)
		{
			i_trans = a_trans;
		}

		public virtual void Activate(object a_obj, int a_depth)
		{
			if (i_trans == null)
			{
				return;
			}
			if (a_depth < 0)
			{
				Stream().Activate(i_trans, a_obj);
			}
			else
			{
				Stream().Activate(i_trans, a_obj, new LegacyActivationDepth(a_depth));
			}
		}

		public virtual bool CanBind()
		{
			return false;
		}

		public virtual void CheckActive()
		{
			if (i_trans == null)
			{
				return;
			}
			if (i_yapObject == null)
			{
				i_yapObject = i_trans.ReferenceForObject(this);
				if (i_yapObject == null)
				{
					Stream().Set(i_trans, this);
					i_yapObject = i_trans.ReferenceForObject(this);
				}
			}
			if (ValidYapObject())
			{
				i_yapObject.Activate(i_trans, this, ActivationDepth(ActivationMode.Activate));
			}
		}

		private LegacyActivationDepth ActivationDepth(ActivationMode mode)
		{
			return new LegacyActivationDepth(3, mode);
		}

		public virtual object CreateDefault(Transaction a_trans)
		{
			throw Exceptions4.VirtualException();
		}

		internal virtual void Deactivate()
		{
			if (ValidYapObject())
			{
				i_yapObject.Deactivate(i_trans, ActivationDepth(ActivationMode.Deactivate));
			}
		}

		internal virtual void Delete()
		{
			if (i_trans == null)
			{
				return;
			}
			if (i_yapObject == null)
			{
				i_yapObject = i_trans.ReferenceForObject(this);
			}
			if (ValidYapObject())
			{
				Stream().Delete2(i_trans, i_yapObject, this, 0, false);
			}
		}

		protected virtual void Delete(object a_obj)
		{
			if (i_trans != null)
			{
				Stream().Delete(i_trans, a_obj);
			}
		}

		protected virtual long GetIDOf(object a_obj)
		{
			if (i_trans == null)
			{
				return 0;
			}
			return Stream().GetID(i_trans, a_obj);
		}

		protected virtual Transaction GetTrans()
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

		public virtual void SetTrans(Transaction a_trans)
		{
			i_trans = a_trans;
		}

		public virtual void SetObjectReference(ObjectReference a_yapObject)
		{
			i_yapObject = a_yapObject;
		}

		protected virtual void Store(object a_obj)
		{
			if (i_trans != null)
			{
				Stream().SetInternal(i_trans, a_obj, true);
			}
		}

		public virtual object StoredTo(Transaction a_trans)
		{
			i_trans = a_trans;
			return this;
		}

		internal virtual object StreamLock()
		{
			if (i_trans != null)
			{
				Stream().CheckClosed();
				return Stream().Lock();
			}
			return this;
		}

		public virtual void Store(int a_depth)
		{
			if (i_trans == null)
			{
				return;
			}
			if (i_yapObject == null)
			{
				i_yapObject = i_trans.ReferenceForObject(this);
				if (i_yapObject == null)
				{
					i_trans.Container().SetInternal(i_trans, this, true);
					i_yapObject = i_trans.ReferenceForObject(this);
					return;
				}
			}
			Update(a_depth);
		}

		internal virtual void Update()
		{
			Update(2);
		}

		internal virtual void Update(int depth)
		{
			if (ValidYapObject())
			{
				ObjectContainerBase stream = Stream();
				stream.BeginTopLevelSet();
				try
				{
					i_yapObject.WriteUpdate(i_trans, depth);
					stream.CheckStillToSet();
					stream.CompleteTopLevelSet();
				}
				catch (Db4oException e)
				{
					stream.CompleteTopLevelSet(e);
				}
				finally
				{
					stream.EndTopLevelSet(i_trans);
				}
			}
		}

		internal virtual void UpdateInternal()
		{
			UpdateInternal(2);
		}

		internal virtual void UpdateInternal(int depth)
		{
			if (ValidYapObject())
			{
				i_yapObject.WriteUpdate(i_trans, depth);
				Stream().FlagAsHandled(i_yapObject);
				Stream().CheckStillToSet();
			}
		}

		private bool ValidYapObject()
		{
			return (i_trans != null) && (i_yapObject != null) && (i_yapObject.GetID() > 0);
		}

		private ObjectContainerBase Stream()
		{
			return i_trans.Container();
		}
	}
}
