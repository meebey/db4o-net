/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class BTreeIdSystem : IIdSystem
	{
		public BTreeIdSystem(int idSystemId)
		{
		}

		// TODO Auto-generated constructor stub
		public virtual void Close()
		{
		}

		// TODO Auto-generated method stub
		public virtual Slot CommittedSlot(int id)
		{
			// TODO Auto-generated method stub
			return null;
		}

		public virtual void CompleteInterruptedTransaction(int transactionId1, int transactionId2
			)
		{
		}

		// TODO Auto-generated method stub
		public virtual int NewId()
		{
			// TODO Auto-generated method stub
			return 0;
		}

		public virtual void Commit(IVisitable slotChanges, IRunnable commitBlock)
		{
		}

		// TODO implement
		public virtual void ReturnUnusedIds(IVisitable visitable)
		{
		}
		// TODO Auto-generated method stub
	}
}
