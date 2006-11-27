namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	/// <renameto>com.db4o.inside.PersistentBase</renameto>
	public abstract class YapMeta
	{
		/// <moveto>
		/// new com.db4o.inside.Persistent interface
		/// all four of the following abstract methods
		/// </moveto>
		public abstract byte GetIdentifier();

		public abstract int OwnLength();

		public abstract void ReadThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 reader);

		public abstract void WriteThis(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 writer);

		protected int i_id;

		protected int i_state = 2;

		internal bool BeginProcessing()
		{
			if (BitIsTrue(Db4objects.Db4o.YapConst.PROCESSING))
			{
				return false;
			}
			BitTrue(Db4objects.Db4o.YapConst.PROCESSING);
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
			if (!BitIsTrue(Db4objects.Db4o.YapConst.CACHED_DIRTY))
			{
				BitTrue(Db4objects.Db4o.YapConst.CACHED_DIRTY);
				col.Add(this);
			}
		}

		public virtual void EndProcessing()
		{
			BitFalse(Db4objects.Db4o.YapConst.PROCESSING);
		}

		public virtual int GetID()
		{
			return i_id;
		}

		public bool IsActive()
		{
			return BitIsTrue(Db4objects.Db4o.YapConst.ACTIVE);
		}

		public virtual bool IsDirty()
		{
			return BitIsTrue(Db4objects.Db4o.YapConst.ACTIVE) && (!BitIsTrue(Db4objects.Db4o.YapConst
				.CLEAN));
		}

		public bool IsNew()
		{
			return i_id == 0;
		}

		public virtual int LinkLength()
		{
			return Db4objects.Db4o.YapConst.ID_LENGTH;
		}

		internal void NotCachedDirty()
		{
			BitFalse(Db4objects.Db4o.YapConst.CACHED_DIRTY);
		}

		public virtual void Read(Db4objects.Db4o.Transaction trans)
		{
			try
			{
				if (BeginProcessing())
				{
					Db4objects.Db4o.YapReader reader = trans.Stream().ReadReaderByID(trans, GetID());
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
			BitTrue(Db4objects.Db4o.YapConst.ACTIVE);
			BitTrue(Db4objects.Db4o.YapConst.CLEAN);
		}

		public void SetStateDeactivated()
		{
			BitFalse(Db4objects.Db4o.YapConst.ACTIVE);
		}

		public virtual void SetStateDirty()
		{
			BitTrue(Db4objects.Db4o.YapConst.ACTIVE);
			BitFalse(Db4objects.Db4o.YapConst.CLEAN);
		}

		internal virtual void SetStateOnRead(Db4objects.Db4o.YapReader reader)
		{
			if (BitIsTrue(Db4objects.Db4o.YapConst.CACHED_DIRTY))
			{
				SetStateDirty();
			}
			else
			{
				SetStateClean();
			}
		}

		public void Write(Db4objects.Db4o.Transaction trans)
		{
			if (!WriteObjectBegin())
			{
				return;
			}
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)trans.Stream();
			int address = 0;
			int length = OwnLength();
			Db4objects.Db4o.YapReader writer = new Db4objects.Db4o.YapReader(length);
			if (IsNew())
			{
				Db4objects.Db4o.Inside.Slots.Pointer4 ptr = stream.NewSlot(trans, length);
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

		public virtual void WriteOwnID(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapReader
			 writer)
		{
			Write(trans);
			writer.WriteInt(GetID());
		}
	}
}
