using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class PersistentBase : IPersistent
	{
		protected int i_id;

		protected int i_state = 2;

		internal bool BeginProcessing()
		{
			if (BitIsTrue(Const4.PROCESSING))
			{
				return false;
			}
			BitTrue(Const4.PROCESSING);
			return true;
		}

		internal void BitFalse(int bitPos)
		{
			i_state &= ~(1 << bitPos);
		}

		internal bool BitIsFalse(int bitPos)
		{
			return (i_state | (1 << bitPos)) != i_state;
		}

		internal bool BitIsTrue(int bitPos)
		{
			return (i_state | (1 << bitPos)) == i_state;
		}

		internal void BitTrue(int bitPos)
		{
			i_state |= (1 << bitPos);
		}

		internal virtual void CacheDirty(Collection4 col)
		{
			if (!BitIsTrue(Const4.CACHED_DIRTY))
			{
				BitTrue(Const4.CACHED_DIRTY);
				col.Add(this);
			}
		}

		public virtual void EndProcessing()
		{
			BitFalse(Const4.PROCESSING);
		}

		public virtual int GetID()
		{
			return i_id;
		}

		public bool IsActive()
		{
			return BitIsTrue(Const4.ACTIVE);
		}

		public virtual bool IsDirty()
		{
			return BitIsTrue(Const4.ACTIVE) && (!BitIsTrue(Const4.CLEAN));
		}

		public bool IsNew()
		{
			return i_id == 0;
		}

		public virtual int LinkLength()
		{
			return Const4.ID_LENGTH;
		}

		internal void NotCachedDirty()
		{
			BitFalse(Const4.CACHED_DIRTY);
		}

		public virtual void Read(Transaction trans)
		{
			if (!BeginProcessing())
			{
				return;
			}
			try
			{
				Db4objects.Db4o.Internal.Buffer reader = trans.Stream().ReadReaderByID(trans, GetID
					());
				ReadThis(trans, reader);
				SetStateOnRead(reader);
			}
			finally
			{
				EndProcessing();
			}
		}

		public virtual void SetID(int a_id)
		{
			i_id = a_id;
		}

		public void SetStateClean()
		{
			BitTrue(Const4.ACTIVE);
			BitTrue(Const4.CLEAN);
		}

		public void SetStateDeactivated()
		{
			BitFalse(Const4.ACTIVE);
		}

		public virtual void SetStateDirty()
		{
			BitTrue(Const4.ACTIVE);
			BitFalse(Const4.CLEAN);
		}

		internal virtual void SetStateOnRead(Db4objects.Db4o.Internal.Buffer reader)
		{
			if (BitIsTrue(Const4.CACHED_DIRTY))
			{
				SetStateDirty();
			}
			else
			{
				SetStateClean();
			}
		}

		public void Write(Transaction trans)
		{
			if (!WriteObjectBegin())
			{
				return;
			}
			try
			{
				LocalObjectContainer stream = (LocalObjectContainer)trans.Stream();
				int length = OwnLength();
				Db4objects.Db4o.Internal.Buffer writer = new Db4objects.Db4o.Internal.Buffer(length
					);
				Slot slot;
				if (IsNew())
				{
					Pointer4 pointer = stream.NewSlot(trans, length);
					SetID(pointer._id);
					slot = pointer._slot;
				}
				else
				{
					slot = stream.GetSlot(length);
					trans.SlotFreeOnRollbackCommitSetPointer(i_id, slot, true);
				}
				WriteToFile(trans, writer, slot);
			}
			finally
			{
				EndProcessing();
			}
		}

		private void WriteToFile(Transaction trans, Db4objects.Db4o.Internal.Buffer writer
			, Slot slot)
		{
			LocalObjectContainer container = (LocalObjectContainer)trans.Stream();
			WriteThis(trans, writer);
			writer.WriteEncrypt(container, slot.Address(), 0);
			if (IsActive())
			{
				SetStateClean();
			}
		}

		public virtual bool WriteObjectBegin()
		{
			if (IsDirty())
			{
				return BeginProcessing();
			}
			return false;
		}

		public virtual void WriteOwnID(Transaction trans, Db4objects.Db4o.Internal.Buffer
			 writer)
		{
			Write(trans);
			writer.WriteInt(GetID());
		}

		public override int GetHashCode()
		{
			if (IsNew())
			{
				throw new InvalidOperationException();
			}
			return GetID();
		}

		public abstract byte GetIdentifier();

		public abstract int OwnLength();

		public abstract void ReadThis(Transaction arg1, Db4objects.Db4o.Internal.Buffer arg2
			);

		public abstract void WriteThis(Transaction arg1, Db4objects.Db4o.Internal.Buffer 
			arg2);
	}
}
