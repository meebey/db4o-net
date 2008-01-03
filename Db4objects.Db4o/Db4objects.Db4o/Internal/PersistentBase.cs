/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class PersistentBase : IPersistent
	{
		protected int _id;

		protected int _state = 2;

		public bool BeginProcessing()
		{
			if (BitIsTrue(Const4.Processing))
			{
				return false;
			}
			BitTrue(Const4.Processing);
			return true;
		}

		internal void BitFalse(int bitPos)
		{
			_state &= ~(1 << bitPos);
		}

		internal bool BitIsFalse(int bitPos)
		{
			return (_state | (1 << bitPos)) != _state;
		}

		internal bool BitIsTrue(int bitPos)
		{
			return (_state | (1 << bitPos)) == _state;
		}

		internal void BitTrue(int bitPos)
		{
			_state |= (1 << bitPos);
		}

		internal virtual void CacheDirty(Collection4 col)
		{
			if (!BitIsTrue(Const4.CachedDirty))
			{
				BitTrue(Const4.CachedDirty);
				col.Add(this);
			}
		}

		public virtual void EndProcessing()
		{
			BitFalse(Const4.Processing);
		}

		public virtual void Free(Transaction trans)
		{
			trans.SystemTransaction().SlotFreePointerOnCommit(GetID());
		}

		public virtual int GetID()
		{
			return _id;
		}

		public bool IsActive()
		{
			return BitIsTrue(Const4.Active);
		}

		public virtual bool IsDirty()
		{
			return BitIsTrue(Const4.Active) && (!BitIsTrue(Const4.Clean));
		}

		public bool IsNew()
		{
			return GetID() == 0;
		}

		public virtual int LinkLength()
		{
			return Const4.IdLength;
		}

		internal void NotCachedDirty()
		{
			BitFalse(Const4.CachedDirty);
		}

		public virtual void Read(Transaction trans)
		{
			if (!BeginProcessing())
			{
				return;
			}
			try
			{
				BufferImpl reader = trans.Container().ReadReaderByID(trans, GetID());
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
			if (DTrace.enabled)
			{
				DTrace.YapmetaSetId.Log(a_id);
			}
			_id = a_id;
		}

		public void SetStateClean()
		{
			BitTrue(Const4.Active);
			BitTrue(Const4.Clean);
		}

		public void SetStateDeactivated()
		{
			BitFalse(Const4.Active);
		}

		public virtual void SetStateDirty()
		{
			BitTrue(Const4.Active);
			BitFalse(Const4.Clean);
		}

		internal virtual void SetStateOnRead(BufferImpl reader)
		{
			if (BitIsTrue(Const4.CachedDirty))
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
				LocalObjectContainer stream = (LocalObjectContainer)trans.Container();
				int length = OwnLength();
				length = stream.BlockAlignedBytes(length);
				BufferImpl writer = new BufferImpl(length);
				Slot slot;
				if (IsNew())
				{
					Pointer4 pointer = stream.NewSlot(length);
					SetID(pointer._id);
					slot = pointer._slot;
					trans.SetPointer(pointer);
				}
				else
				{
					slot = stream.GetSlot(length);
					trans.SlotFreeOnRollbackCommitSetPointer(_id, slot, IsFreespaceComponent());
				}
				WriteToFile(trans, writer, slot);
			}
			finally
			{
				EndProcessing();
			}
		}

		public virtual bool IsFreespaceComponent()
		{
			return false;
		}

		private void WriteToFile(Transaction trans, BufferImpl writer, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.YapmetaWrite.Log(GetID());
			}
			LocalObjectContainer container = (LocalObjectContainer)trans.Container();
			WriteThis(trans, writer);
			container.WriteEncrypt(writer, slot.Address(), 0);
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

		public virtual void WriteOwnID(Transaction trans, BufferImpl writer)
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

		public abstract void ReadThis(Transaction arg1, BufferImpl arg2);

		public abstract void WriteThis(Transaction arg1, BufferImpl arg2);
	}
}
