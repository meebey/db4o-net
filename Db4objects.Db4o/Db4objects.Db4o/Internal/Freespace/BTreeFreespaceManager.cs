/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
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

		private PersistentIntegerArray _idArray;

		private int _delegateIndirectionID;

		private int _delegationRequests;

		public BTreeFreespaceManager(LocalObjectContainer file) : base(file)
		{
			_delegate = new RamFreespaceManager(file);
		}

		private void AddSlot(Slot slot)
		{
			_slotsByLength.Add(Transaction(), slot);
			_slotsByAddress.Add(Transaction(), slot);
		}

		public override Slot AllocateTransactionLogSlot(int length)
		{
			return _delegate.AllocateTransactionLogSlot(length);
		}

		public override void BeginCommit()
		{
		}

		private void BeginDelegation()
		{
			_delegationRequests++;
		}

		public override void Commit()
		{
			BeginDelegation();
			_slotsByAddress.Commit(Transaction());
			_slotsByLength.Commit(Transaction());
		}

		private void CreateBTrees(int addressID, int lengthID)
		{
			_slotsByAddress = new FreespaceBTree(Transaction(), addressID, new AddressKeySlotHandler
				());
			_slotsByLength = new FreespaceBTree(Transaction(), lengthID, new LengthKeySlotHandler
				());
		}

		public override void EndCommit()
		{
			EndDelegation();
		}

		private void EndDelegation()
		{
			_delegationRequests--;
		}

		public override void Free(Slot slot)
		{
			if (!Started())
			{
				return;
			}
			if (IsDelegating())
			{
				_delegate.Free(slot);
				return;
			}
			try
			{
				BeginDelegation();
				if (DTrace.enabled)
				{
					DTrace.Free.LogLength(slot.Address(), slot.Length());
				}
				Slot[] remove = new Slot[2];
				Slot newFreeSlot = slot;
				BTreePointer pointer = SearchBTree(_slotsByAddress, slot, SearchTarget.Lowest);
				BTreePointer previousPointer = pointer != null ? pointer.Previous() : _slotsByAddress
					.LastPointer(Transaction());
				if (previousPointer != null)
				{
					Slot previousSlot = (Slot)previousPointer.Key();
					if (previousSlot.IsDirectlyPreceding(newFreeSlot))
					{
						remove[0] = previousSlot;
						newFreeSlot = previousSlot.Append(newFreeSlot);
					}
				}
				if (pointer != null)
				{
					Slot nextSlot = (Slot)pointer.Key();
					if (newFreeSlot.IsDirectlyPreceding(nextSlot))
					{
						remove[1] = nextSlot;
						newFreeSlot = newFreeSlot.Append(nextSlot);
					}
				}
				for (int i = 0; i < remove.Length; i++)
				{
					if (remove[i] != null)
					{
						RemoveSlot(remove[i]);
					}
				}
				if (!CanDiscard(newFreeSlot.Length()))
				{
					AddSlot(newFreeSlot);
				}
				_file.OverwriteDeletedBlockedSlot(slot);
			}
			finally
			{
				EndDelegation();
			}
		}

		public override void FreeSelf()
		{
			_slotsByAddress.Free(Transaction());
			_slotsByLength.Free(Transaction());
		}

		public override void FreeTransactionLogSlot(Slot slot)
		{
			_delegate.FreeTransactionLogSlot(slot);
		}

		public override Slot GetSlot(int length)
		{
			if (!Started())
			{
				return null;
			}
			if (IsDelegating())
			{
				return _delegate.GetSlot(length);
			}
			try
			{
				BeginDelegation();
				BTreePointer pointer = SearchBTree(_slotsByLength, new Slot(0, length), SearchTarget
					.Highest);
				if (pointer == null)
				{
					return null;
				}
				Slot slot = (Slot)pointer.Key();
				RemoveSlot(slot);
				int remainingLength = slot.Length() - length;
				if (!CanDiscard(remainingLength))
				{
					AddSlot(slot.SubSlot(length));
					slot = slot.Truncate(length);
				}
				if (DTrace.enabled)
				{
					DTrace.GetFreespace.LogLength(slot.Address(), slot.Length());
				}
				return slot;
			}
			finally
			{
				EndDelegation();
			}
		}

		private void InitializeExisting(int slotAddress)
		{
			_idArray = new PersistentIntegerArray(slotAddress);
			_idArray.Read(Transaction());
			int[] ids = _idArray.Array();
			int addressId = ids[0];
			int lengthID = ids[1];
			_delegateIndirectionID = ids[2];
			CreateBTrees(addressId, lengthID);
			_slotsByAddress.Read(Transaction());
			_slotsByLength.Read(Transaction());
			Pointer4 delegatePointer = Transaction().ReadPointer(_delegateIndirectionID);
			Transaction().WriteZeroPointer(_delegateIndirectionID);
			Transaction().FlushFile();
			_delegate.Read(delegatePointer._slot);
		}

		private void InitializeNew()
		{
			CreateBTrees(0, 0);
			_slotsByAddress.Write(Transaction());
			_slotsByLength.Write(Transaction());
			_delegateIndirectionID = _file.GetPointerSlot();
			int[] ids = new int[] { _slotsByAddress.GetID(), _slotsByLength.GetID(), _delegateIndirectionID
				 };
			_idArray = new PersistentIntegerArray(ids);
			_idArray.Write(Transaction());
			_file.SystemData().FreespaceAddress(_idArray.GetID());
		}

		private bool IsDelegating()
		{
			return _delegationRequests > 0;
		}

		public override int OnNew(LocalObjectContainer file)
		{
			return 0;
		}

		public override void Read(int freeSpaceID)
		{
		}

		private void RemoveSlot(Slot slot)
		{
			_slotsByLength.Remove(Transaction(), slot);
			_slotsByAddress.Remove(Transaction(), slot);
		}

		private BTreePointer SearchBTree(BTree bTree, Slot slot, SearchTarget target)
		{
			BTreeNodeSearchResult searchResult = bTree.SearchLeaf(Transaction(), slot, target
				);
			return searchResult.FirstValidPointer();
		}

		public override int SlotCount()
		{
			return _slotsByAddress.Size(Transaction()) + _delegate.SlotCount();
		}

		public override void Start(int slotAddress)
		{
			try
			{
				BeginDelegation();
				if (slotAddress == 0)
				{
					InitializeNew();
				}
				else
				{
					InitializeExisting(slotAddress);
				}
			}
			finally
			{
				EndDelegation();
			}
		}

		private bool Started()
		{
			return _idArray != null;
		}

		public override byte SystemType()
		{
			return FmBtree;
		}

		public override string ToString()
		{
			return _slotsByLength.ToString();
		}

		public override int TotalFreespace()
		{
			return base.TotalFreespace() + _delegate.TotalFreespace();
		}

		public override void Traverse(IVisitor4 visitor)
		{
			_slotsByAddress.TraverseKeys(Transaction(), visitor);
		}

		public override int Write()
		{
			try
			{
				BeginDelegation();
				Slot slot = _file.GetSlot(_delegate.MarshalledLength());
				Pointer4 pointer = new Pointer4(_delegateIndirectionID, slot);
				_delegate.Write(pointer);
				return _idArray.GetID();
			}
			finally
			{
				EndDelegation();
			}
		}
	}
}
