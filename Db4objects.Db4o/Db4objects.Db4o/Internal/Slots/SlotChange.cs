/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Slots
{
	/// <exclude></exclude>
	public class SlotChange : TreeInt
	{
		private int _action;

		private Slot _newSlot;

		private ReferencedSlot _shared;

		private const int FreeOnCommitBit = 1;

		private const int FreeOnRollbackBit = 2;

		private const int SetPointerBit = 3;

		private const int FreePointerOnCommitBit = 4;

		private const int FreePointerOnRollbackBit = 5;

		private const int FreespaceBit = 6;

		public SlotChange(int id) : base(id)
		{
		}

		public override object ShallowClone()
		{
			Db4objects.Db4o.Internal.Slots.SlotChange sc = new Db4objects.Db4o.Internal.Slots.SlotChange
				(0);
			sc._action = _action;
			sc._newSlot = _newSlot;
			sc._shared = _shared;
			return base.ShallowCloneInternal(sc);
		}

		private void DoFreeOnCommit()
		{
			SetBit(FreeOnCommitBit);
		}

		private void DoFreeOnRollback()
		{
			SetBit(FreeOnRollbackBit);
		}

		private void DoFreePointerOnCommit()
		{
			SetBit(FreePointerOnCommitBit);
		}

		private void DoFreePointerOnRollback()
		{
			SetBit(FreePointerOnRollbackBit);
		}

		private void DoSetPointer()
		{
			SetBit(SetPointerBit);
		}

		public virtual void FreeDuringCommit(LocalObjectContainer file, bool forFreespace
			)
		{
			if (IsFreeOnCommit() && (IsForFreeSpace() == forFreespace))
			{
				file.FreeDuringCommit(_shared, _newSlot);
			}
		}

		public void FreeOnCommit(LocalObjectContainer file, Slot slot)
		{
			if (_shared != null)
			{
				// second call or later.
				// The object has already been rewritten once, so we can free
				// directly
				file.Free(slot);
				return;
			}
			DoFreeOnCommit();
			ReferencedSlot refSlot = file.ProduceFreeOnCommitEntry(_key);
			if (refSlot.AddReferenceIsFirst())
			{
				refSlot.PointTo(slot);
			}
			_shared = refSlot;
		}

		public virtual void FreeOnRollback(Slot slot)
		{
			DoFreeOnRollback();
			_newSlot = slot;
		}

		public virtual void FreeOnRollbackSetPointer(Slot slot)
		{
			DoSetPointer();
			FreeOnRollback(slot);
		}

		public virtual void FreePointerOnCommit()
		{
			DoFreePointerOnCommit();
		}

		public virtual void FreePointerOnRollback()
		{
			DoFreePointerOnRollback();
		}

		private bool IsBitSet(int bitPos)
		{
			return (_action | (1 << bitPos)) == _action;
		}

		public virtual bool IsDeleted()
		{
			return IsSetPointer() && (_newSlot.Address() == 0);
		}

		public virtual bool IsNew()
		{
			return IsFreePointerOnRollback();
		}

		private bool IsForFreeSpace()
		{
			return IsBitSet(FreespaceBit);
		}

		private bool IsFreeOnCommit()
		{
			return IsBitSet(FreeOnCommitBit);
		}

		private bool IsFreeOnRollback()
		{
			return IsBitSet(FreeOnRollbackBit);
		}

		public bool IsSetPointer()
		{
			return IsBitSet(SetPointerBit);
		}

		/// <summary>FIXME:	Check where pointers should be freed on commit.</summary>
		/// <remarks>
		/// FIXME:	Check where pointers should be freed on commit.
		/// This should be triggered in this class.
		/// </remarks>
		public bool IsFreePointerOnRollback()
		{
			//	private final boolean isFreePointerOnCommit() {
			//		return isBitSet(FREE_POINTER_ON_COMMIT_BIT);
			//	}
			return IsBitSet(FreePointerOnRollbackBit);
		}

		public virtual Slot NewSlot()
		{
			return _newSlot;
		}

		public virtual Slot OldSlot()
		{
			if (_shared == null)
			{
				return null;
			}
			return _shared.Slot();
		}

		public override object Read(ByteArrayBuffer reader)
		{
			Db4objects.Db4o.Internal.Slots.SlotChange change = new Db4objects.Db4o.Internal.Slots.SlotChange
				(reader.ReadInt());
			change._newSlot = new Slot(reader.ReadInt(), reader.ReadInt());
			change.DoSetPointer();
			return change;
		}

		public virtual void Rollback(LocalObjectContainer yapFile)
		{
			if (_shared != null)
			{
				yapFile.ReduceFreeOnCommitReferences(_shared);
			}
			if (IsFreeOnRollback())
			{
				yapFile.Free(_newSlot);
			}
			if (IsFreePointerOnRollback())
			{
				if (DTrace.enabled)
				{
					DTrace.FreePointerOnRollback.LogLength(_key, Const4.PointerLength);
				}
				yapFile.Free(_key, Const4.PointerLength);
			}
		}

		private void SetBit(int bitPos)
		{
			_action |= (1 << bitPos);
		}

		public virtual void SetPointer(Slot slot)
		{
			DoSetPointer();
			_newSlot = slot;
		}

		public override void Write(ByteArrayBuffer writer)
		{
			if (IsSetPointer())
			{
				writer.WriteInt(_key);
				writer.WriteInt(_newSlot.Address());
				writer.WriteInt(_newSlot.Length());
			}
		}

		public void WritePointer(LocalTransaction trans)
		{
			if (IsSetPointer())
			{
				trans.WritePointer(_key, _newSlot);
			}
		}

		public virtual void ForFreespace(bool flag)
		{
			if (flag)
			{
				SetBit(FreespaceBit);
			}
		}
	}
}
