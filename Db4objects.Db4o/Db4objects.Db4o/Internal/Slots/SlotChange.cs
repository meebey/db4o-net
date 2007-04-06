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

		private const int FREE_ON_COMMIT_BIT = 1;

		private const int FREE_ON_ROLLBACK_BIT = 2;

		private const int SET_POINTER_BIT = 3;

		private const int FREE_POINTER_ON_COMMIT_BIT = 4;

		private const int FREE_POINTER_ON_ROLLBACK_BIT = 5;

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
			SetBit(FREE_ON_COMMIT_BIT);
		}

		private void DoFreeOnRollback()
		{
			SetBit(FREE_ON_ROLLBACK_BIT);
		}

		private void DoFreePointerOnCommit()
		{
			SetBit(FREE_POINTER_ON_COMMIT_BIT);
		}

		private void DoFreePointerOnRollback()
		{
			SetBit(FREE_POINTER_ON_ROLLBACK_BIT);
		}

		private void DoSetPointer()
		{
			SetBit(SET_POINTER_BIT);
		}

		public virtual void FreeDuringCommit(LocalObjectContainer file)
		{
			if (IsFreeOnCommit())
			{
				file.FreeDuringCommit(_shared, _newSlot);
			}
		}

		public virtual void FreeOnCommit(LocalObjectContainer file, Slot slot)
		{
			if (_shared != null)
			{
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

		public virtual void FreeOnRollback(int address, int length)
		{
			DoFreeOnRollback();
			_newSlot = new Slot(address, length);
		}

		public virtual void FreeOnRollbackSetPointer(int address, int length)
		{
			DoSetPointer();
			FreeOnRollback(address, length);
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
			return IsSetPointer() && (_newSlot._address == 0);
		}

		public virtual bool IsNew()
		{
			return IsFreePointerOnRollback();
		}

		private bool IsFreeOnCommit()
		{
			return IsBitSet(FREE_ON_COMMIT_BIT);
		}

		private bool IsFreeOnRollback()
		{
			return IsBitSet(FREE_ON_ROLLBACK_BIT);
		}

		public bool IsSetPointer()
		{
			return IsBitSet(SET_POINTER_BIT);
		}

		/// <summary>FIXME:	Check where pointers should be freed on commit.</summary>
		/// <remarks>
		/// FIXME:	Check where pointers should be freed on commit.
		/// This should be triggered in this class.
		/// </remarks>
		public bool IsFreePointerOnRollback()
		{
			return IsBitSet(FREE_POINTER_ON_ROLLBACK_BIT);
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

		public override object Read(Db4objects.Db4o.Internal.Buffer reader)
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
				yapFile.Free(_key, Const4.POINTER_LENGTH);
			}
		}

		private void SetBit(int bitPos)
		{
			_action |= (1 << bitPos);
		}

		public virtual void SetPointer(int address, int length)
		{
			DoSetPointer();
			_newSlot = new Slot(address, length);
		}

		public override void Write(Db4objects.Db4o.Internal.Buffer writer)
		{
			if (IsSetPointer())
			{
				writer.WriteInt(_key);
				writer.WriteInt(_newSlot._address);
				writer.WriteInt(_newSlot._length);
			}
		}

		public void WritePointer(LocalTransaction trans)
		{
			if (IsSetPointer())
			{
				trans.WritePointer(_key, _newSlot._address, _newSlot._length);
			}
		}
	}
}
