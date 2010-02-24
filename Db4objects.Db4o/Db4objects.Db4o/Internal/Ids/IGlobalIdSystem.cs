/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public interface IGlobalIdSystem
	{
		int NewId();

		Slot CommittedSlot(int id);

		void ReturnUnusedIds(IVisitable visitable);

		void Close();

		void CompleteInterruptedTransaction(int transactionId1, int transactionId2);

		void Commit(IVisitable slotChanges, IRunnable commitBlock);
	}
}
