/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class TransactionalIdSystemImpl : ITransactionalIdSystem
	{
		private IdSlotChanges _slotChanges;

		private Db4objects.Db4o.Internal.Ids.TransactionalIdSystemImpl _systemIdSystem;

		private readonly IClosure4 _globalIdSystem;

		private readonly IClosure4 _freespaceManager;

		public TransactionalIdSystemImpl(IClosure4 freespaceManager, IClosure4 globalIdSystem
			, Db4objects.Db4o.Internal.Ids.TransactionalIdSystemImpl systemIdSystem)
		{
			_freespaceManager = freespaceManager;
			_globalIdSystem = globalIdSystem;
			_slotChanges = new IdSlotChanges(this, freespaceManager);
			_systemIdSystem = systemIdSystem;
		}

		public virtual void CollectCallBackInfo(ICallbackInfoCollector collector)
		{
			if (!_slotChanges.IsDirty())
			{
				return;
			}
			_slotChanges.TraverseSlotChanges(new _IVisitor4_34(collector));
		}

		private sealed class _IVisitor4_34 : IVisitor4
		{
			public _IVisitor4_34(ICallbackInfoCollector collector)
			{
				this.collector = collector;
			}

			public void Visit(object slotChange)
			{
				int id = ((TreeInt)slotChange)._key;
				if (((SlotChange)slotChange).IsDeleted())
				{
					if (!((SlotChange)slotChange).IsNew())
					{
						collector.Deleted(id);
					}
				}
				else
				{
					if (((SlotChange)slotChange).IsNew())
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

		public virtual bool IsDirty()
		{
			return _slotChanges.IsDirty();
		}

		public virtual void Commit()
		{
			IVisitable slotChangeVisitable = new _IVisitable_55(this);
			GlobalIdSystem().Commit(slotChangeVisitable, new _IRunnable_60(this));
			FreespaceEndCommit();
		}

		private sealed class _IVisitable_55 : IVisitable
		{
			public _IVisitable_55(TransactionalIdSystemImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Accept(IVisitor4 visitor)
			{
				this._enclosing.TraverseSlotChanges(visitor);
			}

			private readonly TransactionalIdSystemImpl _enclosing;
		}

		private sealed class _IRunnable_60 : IRunnable
		{
			public _IRunnable_60(TransactionalIdSystemImpl _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.FreeSlotChanges(false);
				this._enclosing.FreespaceBeginCommit();
				this._enclosing.CommitFreespace();
				this._enclosing.FreeSlotChanges(true);
			}

			private readonly TransactionalIdSystemImpl _enclosing;
		}

		private void FreeSlotChanges(bool forFreespace)
		{
			_slotChanges.FreeSlotChanges(forFreespace, IsSystemIdSystem());
			if (!IsSystemIdSystem())
			{
				_systemIdSystem.FreeSlotChanges(forFreespace);
			}
		}

		private bool IsSystemIdSystem()
		{
			return _systemIdSystem == null;
		}

		public virtual void CompleteInterruptedTransaction(int transactionId1, int transactionId2
			)
		{
			GlobalIdSystem().CompleteInterruptedTransaction(transactionId1, transactionId2);
		}

		public virtual Slot CommittedSlot(int id)
		{
			if (id == 0)
			{
				return null;
			}
			return GlobalIdSystem().CommittedSlot(id);
		}

		public virtual Slot CurrentSlot(int id)
		{
			Slot slot = ModifiedSlot(id);
			if (slot != null)
			{
				return slot;
			}
			return CommittedSlot(id);
		}

		public virtual Slot ModifiedSlot(int id)
		{
			if (id == 0)
			{
				return null;
			}
			SlotChange change = _slotChanges.FindSlotChange(id);
			if (change != null)
			{
				if (change.SlotModified())
				{
					return change.NewSlot();
				}
			}
			return ModifiedSlotInUnderlyingIdSystem(id);
		}

		public virtual Slot ModifiedSlotInUnderlyingIdSystem(int id)
		{
			if (IsSystemIdSystem())
			{
				return null;
			}
			return _systemIdSystem.ModifiedSlot(id);
		}

		public virtual void Rollback()
		{
			_slotChanges.Rollback();
		}

		public virtual void Clear()
		{
			_slotChanges.Clear();
		}

		public virtual bool IsDeleted(int id)
		{
			return _slotChanges.IsDeleted(id);
		}

		public virtual void NotifySlotUpdated(int id, Slot slot, SlotChangeFactory slotChangeFactory
			)
		{
			_slotChanges.NotifySlotUpdated(id, slot, slotChangeFactory);
		}

		private void TraverseSlotChanges(IVisitor4 visitor)
		{
			if (!IsSystemIdSystem())
			{
				_systemIdSystem.TraverseSlotChanges(visitor);
			}
			_slotChanges.TraverseSlotChanges(visitor);
		}

		private void FreespaceBeginCommit()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().BeginCommit();
		}

		private void FreespaceEndCommit()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().EndCommit();
		}

		private void CommitFreespace()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().Commit();
		}

		public virtual int NewId(SlotChangeFactory slotChangeFactory)
		{
			int id = AcquireId();
			_slotChanges.ProduceSlotChange(id, slotChangeFactory).NotifySlotCreated(null);
			return id;
		}

		private int AcquireId()
		{
			return GlobalIdSystem().NewId();
		}

		public virtual int PrefetchID()
		{
			int id = AcquireId();
			_slotChanges.AddPrefetchedID(id);
			return id;
		}

		public virtual void PrefetchedIDConsumed(int id)
		{
			_slotChanges.PrefetchedIDConsumed(id);
		}

		public virtual void NotifySlotCreated(int id, Slot slot, SlotChangeFactory slotChangeFactory
			)
		{
			_slotChanges.NotifySlotCreated(id, slot, slotChangeFactory);
		}

		public virtual void NotifySlotDeleted(int id, SlotChangeFactory slotChangeFactory
			)
		{
			_slotChanges.NotifySlotDeleted(id, slotChangeFactory);
		}

		private IFreespaceManager FreespaceManager()
		{
			return ((IFreespaceManager)_freespaceManager.Run());
		}

		private IIdSystem GlobalIdSystem()
		{
			return ((IIdSystem)_globalIdSystem.Run());
		}

		public virtual void Close()
		{
			_slotChanges.FreePrefetchedIDs(GlobalIdSystem());
		}
	}
}
