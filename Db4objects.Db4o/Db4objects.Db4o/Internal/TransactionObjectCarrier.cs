/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <summary>TODO: Check if all time-consuming stuff is overridden!</summary>
	internal class TransactionObjectCarrier : LocalTransaction
	{
		internal TransactionObjectCarrier(ObjectContainerBase container, Transaction parentTransaction
			, TransactionalReferenceSystem referenceSystem) : base(container, parentTransaction
			, referenceSystem)
		{
		}

		public override void Commit()
		{
		}

		// do nothing
		public override void SlotFreeOnCommit(int id, Slot slot)
		{
		}

		//      do nothing
		public override void SlotFreeOnRollback(int id, Slot slot)
		{
		}

		//      do nothing
		internal override void ProduceUpdateSlotChange(int id, Slot slot)
		{
			SetPointer(id, slot);
		}

		internal override void SlotFreeOnRollbackCommitSetPointer(int id, Slot slot, bool
			 forFreespace)
		{
			SetPointer(id, slot);
		}

		internal override void SlotFreePointerOnCommit(int a_id, Slot slot)
		{
		}

		//      do nothing
		public override void SlotFreePointerOnCommit(int a_id)
		{
		}

		// do nothing
		public override void SetPointer(int a_id, Slot slot)
		{
			WritePointer(a_id, slot);
		}

		internal override bool SupportsVirtualFields()
		{
			return false;
		}
	}
}
