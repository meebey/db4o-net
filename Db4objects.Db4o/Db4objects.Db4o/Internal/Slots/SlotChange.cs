namespace Db4objects.Db4o.Internal.Slots
{
	/// <exclude></exclude>
	public class SlotChange : Db4objects.Db4o.Internal.TreeInt
	{
		private int _action;

		private Db4objects.Db4o.Internal.Slots.Slot _newSlot;

		private Db4objects.Db4o.Internal.Slots.ReferencedSlot _shared;

		private const int FREE_ON_COMMIT_BIT = 1;

		private const int FREE_ON_ROLLBACK_BIT = 2;

		private const int SET_POINTER_BIT = 3;

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

		private void DoSetPointer()
		{
			SetBit(SET_POINTER_BIT);
		}

		public virtual void FreeDuringCommit(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			if (IsFreeOnCommit())
			{
				file.FreeDuringCommit(_shared, _newSlot);
			}
		}

		public virtual void FreeOnCommit(Db4objects.Db4o.Internal.LocalObjectContainer file
			, Db4objects.Db4o.Internal.Slots.Slot slot)
		{
			if (_shared != null)
			{
				file.Free(slot);
				return;
			}
			DoFreeOnCommit();
			Db4objects.Db4o.Internal.Slots.ReferencedSlot refSlot = file.ProduceFreeOnCommitEntry
				(_key);
			if (refSlot.AddReferenceIsFirst())
			{
				refSlot.PointTo(slot);
			}
			_shared = refSlot;
		}

		public virtual void FreeOnRollback(int address, int length)
		{
			DoFreeOnRollback();
			_newSlot = new Db4objects.Db4o.Internal.Slots.Slot(address, length);
		}

		public virtual void FreeOnRollbackSetPointer(int address, int length)
		{
			DoSetPointer();
			FreeOnRollback(address, length);
		}

		private bool IsBitSet(int bitPos)
		{
			return (_action | (1 << bitPos)) == _action;
		}

		public virtual bool IsDeleted()
		{
			return IsSetPointer() && (_newSlot._address == 0);
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

		public virtual Db4objects.Db4o.Internal.Slots.Slot NewSlot()
		{
			return _newSlot;
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot OldSlot()
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
			change._newSlot = new Db4objects.Db4o.Internal.Slots.Slot(reader.ReadInt(), reader
				.ReadInt());
			change.DoSetPointer();
			return change;
		}

		public virtual void Rollback(Db4objects.Db4o.Internal.LocalObjectContainer yapFile
			)
		{
			if (_shared != null)
			{
				yapFile.ReduceFreeOnCommitReferences(_shared);
			}
			if (IsFreeOnRollback())
			{
				yapFile.Free(_newSlot);
			}
		}

		private void SetBit(int bitPos)
		{
			_action |= (1 << bitPos);
		}

		public virtual void SetPointer(int address, int length)
		{
			DoSetPointer();
			_newSlot = new Db4objects.Db4o.Internal.Slots.Slot(address, length);
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

		public virtual void WritePointer(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (IsSetPointer())
			{
				trans.WritePointer(_key, _newSlot._address, _newSlot._length);
			}
		}
	}
}
