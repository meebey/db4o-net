/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public sealed class TransportStandardIdSystem : IIdSystem
	{
		private readonly LocalObjectContainer _container;

		public TransportStandardIdSystem(LocalObjectContainer localObjectContainer)
		{
			_container = localObjectContainer;
		}

		public int NewId(Transaction transaction, SlotChangeFactory slotChangeFactory)
		{
			return LocalContainer().AllocatePointerSlot();
		}

		public void NotifySlotCreated(Transaction transaction, int id, Slot slot, SlotChangeFactory
			 slotChangeFactory)
		{
			WritePointer(id, slot);
		}

		private void WritePointer(int id, Slot slot)
		{
			LocalContainer().WritePointer(id, slot);
		}

		public void NotifySlotChanged(Transaction transaction, int id, Slot slot, SlotChangeFactory
			 slotChangeFactory)
		{
			WritePointer(id, slot);
		}

		public void NotifySlotDeleted(Transaction transaction, int id, SlotChangeFactory 
			slotChangeFactory)
		{
			WritePointer(id, Slot.Zero);
		}

		protected StandardIdSlotChanges SlotChanges(Transaction transaction)
		{
			throw new InvalidOperationException();
		}

		public void Commit(LocalTransaction transaction)
		{
		}

		// don't do anything
		public Slot GetCurrentSlotOfID(LocalTransaction transaction, int id)
		{
			return GetCommittedSlotOfID(id);
		}

		public void AddTransaction(LocalTransaction transaction)
		{
		}

		// do nothing
		public void RemoveTransaction(LocalTransaction transaction)
		{
		}

		// do nothing
		public void CollectCallBackInfo(Transaction transaction, ICallbackInfoCollector collector
			)
		{
		}

		// do nothing
		public void SystemTransaction(LocalTransaction transaction)
		{
		}

		// do nothing
		public void Close()
		{
		}

		// do nothing
		public LocalObjectContainer LocalContainer()
		{
			return _container;
		}

		public void Clear(Transaction transaction)
		{
		}

		// TODO Auto-generated method stub
		public Slot GetCommittedSlotOfID(int id)
		{
			return LocalContainer().ReadPointer(id)._slot;
		}

		public IInterruptedTransactionHandler InterruptedTransactionHandler(ByteArrayBuffer
			 reader)
		{
			return null;
		}

		public bool IsDeleted(Transaction transaction, int id)
		{
			return false;
		}

		public bool IsDirty(Transaction transaction)
		{
			return false;
		}

		public int PrefetchID(Transaction transaction)
		{
			return 0;
		}

		public void PrefetchedIDConsumed(Transaction transaction, int id)
		{
		}

		public void Rollback(Transaction transaction)
		{
		}
	}
}
