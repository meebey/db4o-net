namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class PersistentBase : Db4objects.Db4o.Internal.IPersistent
	{
		protected int i_id;

		protected int i_state = 2;

		internal bool BeginProcessing()
		{
			if (BitIsTrue(Db4objects.Db4o.Internal.Const4.PROCESSING))
			{
				return false;
			}
			BitTrue(Db4objects.Db4o.Internal.Const4.PROCESSING);
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

		internal virtual void CacheDirty(Db4objects.Db4o.Foundation.Collection4 col)
		{
			if (!BitIsTrue(Db4objects.Db4o.Internal.Const4.CACHED_DIRTY))
			{
				BitTrue(Db4objects.Db4o.Internal.Const4.CACHED_DIRTY);
				col.Add(this);
			}
		}

		public virtual void EndProcessing()
		{
			BitFalse(Db4objects.Db4o.Internal.Const4.PROCESSING);
		}

		public virtual int GetID()
		{
			return i_id;
		}

		public bool IsActive()
		{
			return BitIsTrue(Db4objects.Db4o.Internal.Const4.ACTIVE);
		}

		public virtual bool IsDirty()
		{
			return BitIsTrue(Db4objects.Db4o.Internal.Const4.ACTIVE) && (!BitIsTrue(Db4objects.Db4o.Internal.Const4
				.CLEAN));
		}

		public bool IsNew()
		{
			return i_id == 0;
		}

		public virtual int LinkLength()
		{
			return Db4objects.Db4o.Internal.Const4.ID_LENGTH;
		}

		internal void NotCachedDirty()
		{
			BitFalse(Db4objects.Db4o.Internal.Const4.CACHED_DIRTY);
		}

		public virtual void Read(Db4objects.Db4o.Internal.Transaction trans)
		{
			try
			{
				if (BeginProcessing())
				{
					Db4objects.Db4o.Internal.Buffer reader = trans.Stream().ReadReaderByID(trans, GetID
						());
					if (reader != null)
					{
						ReadThis(trans, reader);
						SetStateOnRead(reader);
					}
					EndProcessing();
				}
			}
			catch (System.Exception t)
			{
			}
		}

		public virtual void SetID(int a_id)
		{
			i_id = a_id;
		}

		public void SetStateClean()
		{
			BitTrue(Db4objects.Db4o.Internal.Const4.ACTIVE);
			BitTrue(Db4objects.Db4o.Internal.Const4.CLEAN);
		}

		public void SetStateDeactivated()
		{
			BitFalse(Db4objects.Db4o.Internal.Const4.ACTIVE);
		}

		public virtual void SetStateDirty()
		{
			BitTrue(Db4objects.Db4o.Internal.Const4.ACTIVE);
			BitFalse(Db4objects.Db4o.Internal.Const4.CLEAN);
		}

		internal virtual void SetStateOnRead(Db4objects.Db4o.Internal.Buffer reader)
		{
			if (BitIsTrue(Db4objects.Db4o.Internal.Const4.CACHED_DIRTY))
			{
				SetStateDirty();
			}
			else
			{
				SetStateClean();
			}
		}

		public void Write(Db4objects.Db4o.Internal.Transaction trans)
		{
			if (!WriteObjectBegin())
			{
				return;
			}
			Db4objects.Db4o.Internal.LocalObjectContainer stream = (Db4objects.Db4o.Internal.LocalObjectContainer
				)trans.Stream();
			int address = 0;
			int length = OwnLength();
			Db4objects.Db4o.Internal.Buffer writer = new Db4objects.Db4o.Internal.Buffer(length
				);
			if (IsNew())
			{
				Db4objects.Db4o.Internal.Slots.Pointer4 ptr = stream.NewSlot(trans, length);
				SetID(ptr._id);
				address = ptr._address;
			}
			else
			{
				address = stream.GetSlot(length);
				trans.SlotFreeOnRollbackCommitSetPointer(i_id, address, length);
			}
			WriteThis(trans, writer);
			writer.WriteEncrypt(stream, address, 0);
			if (IsActive())
			{
				SetStateClean();
			}
			EndProcessing();
		}

		public virtual bool WriteObjectBegin()
		{
			if (IsDirty())
			{
				return BeginProcessing();
			}
			return false;
		}

		public virtual void WriteOwnID(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.Buffer
			 writer)
		{
			Write(trans);
			writer.WriteInt(GetID());
		}

		public abstract byte GetIdentifier();

		public abstract int OwnLength();

		public abstract void ReadThis(Db4objects.Db4o.Internal.Transaction arg1, Db4objects.Db4o.Internal.Buffer
			 arg2);

		public abstract void WriteThis(Db4objects.Db4o.Internal.Transaction arg1, Db4objects.Db4o.Internal.Buffer
			 arg2);
	}
}
