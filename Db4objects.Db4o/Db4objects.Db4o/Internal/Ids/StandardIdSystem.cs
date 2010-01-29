/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class StandardIdSystem : IIdSystem
	{
		private readonly IDictionary _slotChanges = new Hashtable();

		private StandardIdSlotChanges _systemSlotChanges;

		private readonly TransactionLogHandler _transactionLogHandler;

		public StandardIdSystem(LocalObjectContainer localContainer)
		{
			_transactionLogHandler = NewTransactionLogHandler(localContainer);
		}

		public virtual void AddTransaction(LocalTransaction transaction)
		{
			AddSlotChanges(transaction, new StandardIdSlotChanges(transaction.LocalContainer(
				)));
		}

		public virtual void RemoveTransaction(LocalTransaction transaction)
		{
			SlotChanges(transaction).FreePrefetchedIDs();
			RemoveSlotChanges(transaction);
		}

		private void RemoveSlotChanges(LocalTransaction transaction)
		{
			CheckSynchronization(transaction);
			Sharpen.Collections.Remove(_slotChanges, transaction);
		}

		protected virtual StandardIdSlotChanges SlotChanges(Transaction transaction)
		{
			return ((StandardIdSlotChanges)_slotChanges[transaction]);
		}

		public virtual void CollectCallBackInfo(Transaction transaction, ICallbackInfoCollector
			 collector)
		{
			SlotChanges(transaction).CollectSlotChanges(collector);
		}

		public virtual bool IsDirty(Transaction transaction)
		{
			return SlotChanges(transaction).IsDirty();
		}

		public virtual void Commit(LocalTransaction transaction)
		{
			Slot reservedSlot = _transactionLogHandler.AllocateSlot(transaction, false);
			FreeSlotChanges(transaction, false);
			FreespaceBeginCommit();
			CommitFreespace();
			FreeSlotChanges(transaction, true);
			_transactionLogHandler.ApplySlotChanges(transaction, reservedSlot);
			FreespaceEndCommit();
		}

		private void FreeSlotChanges(LocalTransaction transaction, bool forFreespace)
		{
			if (!IsSystemTransaction(transaction))
			{
				SlotChanges(transaction).FreeSlotChanges(forFreespace, false);
			}
			_systemSlotChanges.FreeSlotChanges(forFreespace, true);
		}

		public virtual void FreeAndClearSystemSlotChanges()
		{
			_systemSlotChanges.FreeSlotChanges(false, true);
			_systemSlotChanges.Clear();
		}

		private bool IsSystemTransaction(LocalTransaction transaction)
		{
			return SlotChanges(transaction) == _systemSlotChanges;
		}

		public virtual IInterruptedTransactionHandler InterruptedTransactionHandler(ByteArrayBuffer
			 reader)
		{
			return _transactionLogHandler.InterruptedTransactionHandler(reader);
		}

		public virtual Slot GetCommittedSlotOfID(int id)
		{
			if (id == 0)
			{
				return null;
			}
			return LocalContainer().ReadPointer(id)._slot;
		}

		public virtual LocalTransaction SystemTransaction()
		{
			return (LocalTransaction)LocalContainer().SystemTransaction();
		}

		public virtual Slot GetCurrentSlotOfID(LocalTransaction transaction, int id)
		{
			if (id == 0)
			{
				return null;
			}
			SlotChange change = SlotChanges(transaction).FindSlotChange(id);
			if (change != null)
			{
				if (change.SlotModified())
				{
					return change.NewSlot();
				}
			}
			if (!IsSystemTransaction(transaction))
			{
				Slot parentSlot = GetCurrentSlotOfID(SystemTransaction(), id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return LocalContainer().ReadPointer(id)._slot;
		}

		public virtual void Rollback(Transaction transaction)
		{
			SlotChanges(transaction).Rollback();
		}

		public virtual void Clear(Transaction transaction)
		{
			SlotChanges(transaction).Clear();
		}

		public virtual bool IsDeleted(Transaction transaction, int id)
		{
			return SlotChanges(transaction).IsDeleted(id);
		}

		public virtual void NotifySlotChanged(Transaction transaction, int id, Slot slot, 
			SlotChangeFactory slotChangeFactory)
		{
			SlotChanges(transaction).NotifySlotChanged(id, slot, slotChangeFactory);
		}

		public virtual void SystemTransaction(LocalTransaction transaction)
		{
			_systemSlotChanges = new StandardIdSlotChanges(transaction.LocalContainer());
			AddSlotChanges(transaction, _systemSlotChanges);
		}

		private void AddSlotChanges(LocalTransaction transaction, StandardIdSlotChanges slotChanges
			)
		{
			CheckSynchronization(transaction);
			_slotChanges[transaction] = slotChanges;
		}

		public virtual void Close()
		{
			_transactionLogHandler.Close();
		}

		private TransactionLogHandler NewTransactionLogHandler(LocalObjectContainer container
			)
		{
			bool fileBased = container.Config().FileBasedTransactionLog() && container is IoAdaptedObjectContainer;
			if (!fileBased)
			{
				return new EmbeddedTransactionLogHandler(this);
			}
			string fileName = ((IoAdaptedObjectContainer)container).FileName();
			return new FileBasedTransactionLogHandler(this, fileName);
		}

		public virtual void TraverseSlotChanges(LocalTransaction transaction, IVisitor4 visitor
			)
		{
			_systemSlotChanges.TraverseSlotChanges(visitor);
			if (transaction == SystemTransaction())
			{
				return;
			}
			SlotChanges(transaction).TraverseSlotChanges(visitor);
		}

		public virtual void FlushFile()
		{
			if (DTrace.enabled)
			{
				DTrace.TransFlush.Log();
			}
			LocalContainer().SyncFiles();
		}

		public virtual LocalObjectContainer LocalContainer()
		{
			return _systemSlotChanges.SystemTransaction().LocalContainer();
		}

		public virtual IFreespaceManager FreespaceManager()
		{
			return LocalContainer().FreespaceManager();
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

		public virtual bool WriteSlots(LocalTransaction transaction)
		{
			LocalObjectContainer container = transaction.LocalContainer();
			BooleanByRef ret = new BooleanByRef();
			TraverseSlotChanges(transaction, new _IVisitor4_210(container, ret));
			return ret.value;
		}

		private sealed class _IVisitor4_210 : IVisitor4
		{
			public _IVisitor4_210(LocalObjectContainer container, BooleanByRef ret)
			{
				this.container = container;
				this.ret = ret;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).WritePointer(container);
				ret.value = true;
			}

			private readonly LocalObjectContainer container;

			private readonly BooleanByRef ret;
		}

		public virtual bool IsReadOnly()
		{
			return Config().IsReadOnly();
		}

		public virtual Config4Impl Config()
		{
			return LocalContainer().Config();
		}

		public virtual void ReadWriteSlotChanges(ByteArrayBuffer buffer)
		{
			_systemSlotChanges.ReadSlotChanges(buffer);
			if (WriteSlots(SystemTransaction()))
			{
				FlushFile();
			}
		}

		public virtual int NewId(Transaction transaction, SlotChangeFactory slotChangeFactory
			)
		{
			int id = LocalContainer().AllocatePointerSlot();
			SlotChanges(transaction).ProduceSlotChange(id, slotChangeFactory).NotifySlotCreated
				(null);
			return id;
		}

		public virtual int PrefetchID(Transaction transaction)
		{
			int id = LocalContainer().AllocatePointerSlot();
			SlotChanges(transaction).AddPrefetchedID(id);
			return id;
		}

		public virtual void PrefetchedIDConsumed(Transaction transaction, int id)
		{
			StandardIdSlotChanges slotChanges = SlotChanges(transaction);
			slotChanges.PrefetchedIDConsumed(id);
		}

		public virtual void NotifySlotCreated(Transaction transaction, int id, Slot slot, 
			SlotChangeFactory slotChangeFactory)
		{
			SlotChanges(transaction).NotifySlotCreated(id, slot, slotChangeFactory);
		}

		public void CheckSynchronization(LocalTransaction transaction)
		{
		}

		public virtual void NotifySlotDeleted(Transaction transaction, int id, SlotChangeFactory
			 slotChangeFactory)
		{
			SlotChanges(transaction).NotifySlotDeleted(id, slotChangeFactory);
		}
	}
}
