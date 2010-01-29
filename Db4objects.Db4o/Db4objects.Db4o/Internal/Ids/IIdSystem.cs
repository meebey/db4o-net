/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public interface IIdSystem
	{
		void AddTransaction(LocalTransaction transaction);

		void RemoveTransaction(LocalTransaction trans);

		void CollectCallBackInfo(Transaction transaction, ICallbackInfoCollector collector
			);

		bool IsDirty(Transaction transaction);

		void Commit(LocalTransaction transaction);

		IInterruptedTransactionHandler InterruptedTransactionHandler(ByteArrayBuffer reader
			);

		Slot GetCommittedSlotOfID(int id);

		Slot GetCurrentSlotOfID(LocalTransaction transaction, int id);

		void Rollback(Transaction transaction);

		void Clear(Transaction transaction);

		bool IsDeleted(Transaction transaction, int id);

		void NotifySlotChanged(Transaction transaction, int id, Slot slot, SlotChangeFactory
			 slotChangeFactory);

		void NotifySlotCreated(Transaction transaction, int id, Slot slot, SlotChangeFactory
			 slotChangeFactory);

		void NotifySlotDeleted(Transaction transaction, int id, SlotChangeFactory slotChangeFactory
			);

		void SystemTransaction(LocalTransaction transaction);

		void Close();

		int NewId(Transaction trans, SlotChangeFactory slotChangeFactory);

		int PrefetchID(Transaction transaction);

		void PrefetchedIDConsumed(Transaction transaction, int id);
	}
}
