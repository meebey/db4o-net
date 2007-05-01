using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public class BTreeFreespaceManager : AbstractFreespaceManager
	{
		private BTree _slotsByAddress;

		private BTree _slotsByLength;

		private PersistentIntegerArray _btreeIDs;

		public BTreeFreespaceManager(LocalObjectContainer file) : base(file)
		{
		}

		public override void Free(Slot slot)
		{
			if (!Started())
			{
				return;
			}
			if (slot._length <= DiscardLimit())
			{
				return;
			}
			Slot newFreeSlot = ToBlocked(slot);
			BTreeNodeSearchResult searchResult = _slotsByAddress.SearchLeaf(Transaction(), slot
				, SearchTarget.LOWEST);
			BTreePointer pointer = searchResult.FirstValidPointer();
			if (pointer != null)
			{
				BTreePointer previousPointer = pointer.Previous();
				if (previousPointer != null)
				{
					Slot previousSlot = (Slot)previousPointer.Key();
					if (previousSlot.IsDirectlyPreceding(newFreeSlot))
					{
						RemoveSlot(previousSlot);
						newFreeSlot = previousSlot.Append(newFreeSlot);
					}
				}
			}
			searchResult = _slotsByAddress.SearchLeaf(Transaction(), slot, SearchTarget.HIGHEST
				);
			pointer = searchResult.FirstValidPointer();
			if (pointer != null)
			{
				Slot nextSlot = (Slot)pointer.Key();
				if (newFreeSlot.IsDirectlyPreceding(nextSlot))
				{
					RemoveSlot(nextSlot);
					newFreeSlot = newFreeSlot.Append(nextSlot);
				}
			}
			AddSlot(newFreeSlot);
			_file.OverwriteDeletedSlot(slot);
		}

		public override void FreeSelf()
		{
		}

		public override Slot GetSlot(int length)
		{
			if (!Started())
			{
				return null;
			}
			int requiredLength = _file.BlocksFor(length);
			BTreeNodeSearchResult searchResult = _slotsByLength.SearchLeaf(Transaction(), new 
				Slot(0, requiredLength), SearchTarget.HIGHEST);
			BTreePointer pointer = searchResult.FirstValidPointer();
			if (pointer == null)
			{
				return null;
			}
			Slot slot = (Slot)pointer.Key();
			RemoveSlot(slot);
			if (slot._length == requiredLength)
			{
				return ToNonBlocked(slot);
			}
			AddSlot(slot.SubSlot(requiredLength));
			return ToNonBlocked(slot.Truncate(requiredLength));
		}

		private void AddSlot(Slot slot)
		{
			_slotsByLength.Add(Transaction(), slot);
			_slotsByAddress.Add(Transaction(), slot);
		}

		private void RemoveSlot(Slot slot)
		{
			_slotsByLength.Remove(Transaction(), slot);
			_slotsByAddress.Remove(Transaction(), slot);
		}

		public override int SlotCount()
		{
			return _slotsByAddress.Size(Transaction());
		}

		public override byte SystemType()
		{
			return FM_BTREE;
		}

		public override void Traverse(IVisitor4 visitor)
		{
			_slotsByAddress.TraverseKeys(Transaction(), visitor);
		}

		public override int Write()
		{
			return _btreeIDs.GetID();
		}

		private Db4objects.Db4o.Internal.Transaction Transaction()
		{
			return _file.SystemTransaction();
		}

		public override void Start(int slotAddress)
		{
			if (slotAddress == 0)
			{
				CreateBTrees(new int[] { 0, 0 });
				_slotsByAddress.Write(Transaction());
				_slotsByLength.Write(Transaction());
				int[] ids = new int[] { _slotsByAddress.GetID(), _slotsByLength.GetID() };
				_btreeIDs = new PersistentIntegerArray(ids);
				_btreeIDs.Write(Transaction());
				return;
			}
			_btreeIDs = new PersistentIntegerArray(slotAddress);
			_btreeIDs.Read(Transaction());
			CreateBTrees(_btreeIDs.Array());
		}

		private void CreateBTrees(int[] ids)
		{
			_slotsByAddress = new BTree(Transaction(), ids[0], new AddressKeySlotHandler());
			_slotsByLength = new BTree(Transaction(), ids[1], new LengthKeySlotHandler());
		}

		private bool Started()
		{
			return _btreeIDs != null;
		}

		public override void BeginCommit()
		{
		}

		public override int OnNew(LocalObjectContainer file)
		{
			return 0;
		}

		public override void EndCommit()
		{
		}

		public override void Read(int freeSpaceID)
		{
		}
	}
}
