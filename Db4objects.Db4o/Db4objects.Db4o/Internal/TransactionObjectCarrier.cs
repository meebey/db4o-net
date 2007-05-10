using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <summary>TODO: Check if all time-consuming stuff is overridden!</summary>
	internal class TransactionObjectCarrier : LocalTransaction
	{
		internal TransactionObjectCarrier(ObjectContainerBase a_stream, Transaction a_parent
			) : base(a_stream, a_parent)
		{
		}

		public override void Commit()
		{
		}

		public override void SlotFreeOnCommit(int id, Slot slot)
		{
		}

		public override void SlotFreeOnRollback(int id, Slot slot)
		{
		}

		internal override void ProduceUpdateSlotChange(int id, Slot slot)
		{
			SetPointer(id, slot);
		}

		internal override void SlotFreeOnRollbackCommitSetPointer(int id, Slot slot, bool
			 freeImmediately)
		{
			SetPointer(id, slot);
		}

		internal override void SlotFreePointerOnCommit(int a_id, Slot slot)
		{
		}

		public override void SlotFreePointerOnCommit(int a_id)
		{
		}

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
