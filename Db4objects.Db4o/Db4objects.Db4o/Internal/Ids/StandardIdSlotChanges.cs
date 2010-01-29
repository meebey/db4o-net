/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Ids
{
	public class StandardIdSlotChanges
	{
		private readonly LockedTree _slotChanges = new LockedTree();

		private readonly LocalObjectContainer _container;

		private Tree _prefetchedIDs;

		public StandardIdSlotChanges(LocalObjectContainer container)
		{
			_container = container;
		}

		public virtual Config4Impl Config()
		{
			return LocalContainer().Config();
		}

		private LocalObjectContainer LocalContainer()
		{
			return _container;
		}

		public void FreeSlotChanges(bool forFreespace, bool traverseMutable)
		{
			IVisitor4 visitor = new _IVisitor4_31(this, forFreespace);
			if (traverseMutable)
			{
				_slotChanges.TraverseMutable(visitor);
			}
			else
			{
				_slotChanges.TraverseLocked(visitor);
			}
		}

		private sealed class _IVisitor4_31 : IVisitor4
		{
			public _IVisitor4_31(StandardIdSlotChanges _enclosing, bool forFreespace)
			{
				this._enclosing = _enclosing;
				this.forFreespace = forFreespace;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).FreeDuringCommit(this._enclosing.LocalContainer(), forFreespace
					);
			}

			private readonly StandardIdSlotChanges _enclosing;

			private readonly bool forFreespace;
		}

		public virtual void Clear()
		{
			_slotChanges.Clear();
		}

		public virtual void Rollback()
		{
			_slotChanges.TraverseLocked(new _IVisitor4_48(this));
		}

		private sealed class _IVisitor4_48 : IVisitor4
		{
			public _IVisitor4_48(StandardIdSlotChanges _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object slotChange)
			{
				((SlotChange)slotChange).Rollback(this._enclosing.LocalContainer());
			}

			private readonly StandardIdSlotChanges _enclosing;
		}

		public virtual bool IsDeleted(int id)
		{
			return SlotChangeIsFlaggedDeleted(id);
		}

		public virtual SlotChange ProduceSlotChange(int id, SlotChangeFactory slotChangeFactory
			)
		{
			if (DTrace.enabled)
			{
				DTrace.ProduceSlotChange.Log(id);
			}
			SlotChange slot = slotChangeFactory.NewInstance(id);
			_slotChanges.Add(slot);
			return (SlotChange)slot.AddedOrExisting();
		}

		public SlotChange FindSlotChange(int id)
		{
			return (SlotChange)_slotChanges.Find(id);
		}

		private bool SlotChangeIsFlaggedDeleted(int id)
		{
			SlotChange slot = FindSlotChange(id);
			if (slot != null)
			{
				return slot.IsDeleted();
			}
			return false;
		}

		public virtual void TraverseSlotChanges(IVisitor4 visitor)
		{
			_slotChanges.TraverseLocked(visitor);
		}

		public virtual bool IsDirty()
		{
			return _slotChanges != null;
		}

		public virtual void CollectSlotChanges(ICallbackInfoCollector collector)
		{
			if (!IsDirty())
			{
				return;
			}
			_slotChanges.TraverseLocked(new _IVisitor4_92(collector));
		}

		private sealed class _IVisitor4_92 : IVisitor4
		{
			public _IVisitor4_92(ICallbackInfoCollector collector)
			{
				this.collector = collector;
			}

			public void Visit(object obj)
			{
				SlotChange slotChange = ((SlotChange)obj);
				int id = slotChange._key;
				if (slotChange.IsDeleted())
				{
					if (!slotChange.IsNew())
					{
						collector.Deleted(id);
					}
				}
				else
				{
					if (slotChange.IsNew())
					{
						collector.Added(id);
					}
					else
					{
						collector.Updated(id);
					}
				}
			}

			private readonly ICallbackInfoCollector collector;
		}

		public virtual void ReadSlotChanges(ByteArrayBuffer buffer)
		{
			_slotChanges.Read(buffer, new SlotChange(0));
		}

		public virtual LocalTransaction SystemTransaction()
		{
			return (LocalTransaction)LocalContainer().SystemTransaction();
		}

		public virtual void AddPrefetchedID(int id)
		{
			_prefetchedIDs = Tree.Add(_prefetchedIDs, new TreeInt(id));
		}

		public virtual void PrefetchedIDConsumed(int id)
		{
			_prefetchedIDs = _prefetchedIDs.RemoveLike(new TreeInt(id));
		}

		internal void FreePrefetchedIDs()
		{
			if (_prefetchedIDs == null)
			{
				return;
			}
			LocalObjectContainer container = LocalContainer();
			_prefetchedIDs.Traverse(new _IVisitor4_131(container));
			_prefetchedIDs = null;
		}

		private sealed class _IVisitor4_131 : IVisitor4
		{
			public _IVisitor4_131(LocalObjectContainer container)
			{
				this.container = container;
			}

			public void Visit(object node)
			{
				TreeInt intNode = (TreeInt)node;
				container.Free(intNode._key, Const4.PointerLength);
			}

			private readonly LocalObjectContainer container;
		}

		public virtual void NotifySlotCreated(int id, Slot slot, SlotChangeFactory slotChangeFactory
			)
		{
			ProduceSlotChange(id, slotChangeFactory).NotifySlotCreated(slot);
		}

		internal virtual void NotifySlotChanged(int id, Slot slot, SlotChangeFactory slotChangeFactory
			)
		{
			ProduceSlotChange(id, slotChangeFactory).NotifySlotChanged(LocalContainer(), slot
				);
		}

		public virtual void NotifySlotDeleted(int id, SlotChangeFactory slotChangeFactory
			)
		{
			ProduceSlotChange(id, slotChangeFactory).NotifyDeleted(LocalContainer());
		}
	}
}
