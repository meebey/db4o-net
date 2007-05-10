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
		private RamFreespaceManager _delegate;

		private FreespaceBTree _slotsByAddress;

		private FreespaceBTree _slotsByLength;

		private PersistentIntegerArray _btreeIDs;

		private int _recursion;

		public BTreeFreespaceManager(LocalObjectContainer file) : base(file)
		{
			_delegate = new RamFreespaceManager(file);
		}

		public override Slot AllocateTransactionLogSlot(int length)
		{
			return _delegate.AllocateTransactionLogSlot(length);
		}

		public override void FreeTransactionLogSlot(Slot slot)
		{
			_delegate.FreeTransactionLogSlot(slot);
		}

		public override void Free(Slot slot)
		{
			if (!Started())
			{
				return;
			}
			if (RecursiveCall())
			{
				_delegate.Free(slot);
				return;
			}
			try
			{
				_recursion++;
				Slot newFreeSlot = slot;
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
				if (!CanDiscard(newFreeSlot.Length()))
				{
					AddSlot(newFreeSlot);
				}
				_file.OverwriteDeletedBlockedSlot(newFreeSlot);
			}
			finally
			{
				_recursion--;
			}
		}

		public override void FreeSelf()
		{
		}

		private bool RecursiveCall()
		{
			return _recursion > 0;
		}

		public override Slot GetSlot(int length)
		{
			if (!Started())
			{
				return null;
			}
			if (RecursiveCall())
			{
				return _delegate.GetSlot(length);
			}
			try
			{
				_recursion++;
				BTreeNodeSearchResult searchResult = _slotsByLength.SearchLeaf(Transaction(), new 
					Slot(0, length), SearchTarget.HIGHEST);
				BTreePointer pointer = searchResult.FirstValidPointer();
				if (pointer == null)
				{
					return null;
				}
				Slot slot = (Slot)pointer.Key();
				RemoveSlot(slot);
				int remainingLength = slot.Length() - length;
				if (CanDiscard(remainingLength))
				{
					return slot;
				}
				AddSlot(slot.SubSlot(length));
				slot = slot.Truncate(length);
				return slot;
			}
			finally
			{
				_recursion--;
			}
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
			try
			{
				_recursion++;
				if (slotAddress == 0)
				{
					CreateBTrees(new int[] { 0, 0 });
					_slotsByAddress.Write(Transaction());
					_slotsByLength.Write(Transaction());
					int[] ids = new int[] { _slotsByAddress.GetID(), _slotsByLength.GetID() };
					_btreeIDs = new PersistentIntegerArray(ids);
					_btreeIDs.Write(Transaction());
					_file.SystemData().FreespaceAddress(_btreeIDs.GetID());
					return;
				}
				_btreeIDs = new PersistentIntegerArray(slotAddress);
				_btreeIDs.Read(Transaction());
				CreateBTrees(_btreeIDs.Array());
				_slotsByAddress.Read(Transaction());
				_slotsByLength.Read(Transaction());
			}
			finally
			{
				_recursion--;
			}
		}

		private void CreateBTrees(int[] ids)
		{
			_slotsByAddress = new FreespaceBTree(Transaction(), ids[0], new AddressKeySlotHandler
				());
			_slotsByLength = new FreespaceBTree(Transaction(), ids[1], new LengthKeySlotHandler
				());
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
			_recursion--;
		}

		public override void Read(int freeSpaceID)
		{
		}

		public override void Commit()
		{
			_recursion++;
			_slotsByAddress.Commit(Transaction());
			_slotsByLength.Commit(Transaction());
		}
	}
}
