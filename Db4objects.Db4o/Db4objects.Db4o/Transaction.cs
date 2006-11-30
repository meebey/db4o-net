namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class Transaction
	{
		private Db4objects.Db4o.Foundation.Tree _slotChanges;

		private int i_address;

		private readonly byte[] _pointerBuffer = new byte[Db4objects.Db4o.YapConst.POINTER_LENGTH
			];

		public Db4objects.Db4o.Foundation.Tree i_delete;

		private Db4objects.Db4o.Foundation.List4 i_dirtyFieldIndexes;

		public readonly Db4objects.Db4o.YapFile i_file;

		internal readonly Db4objects.Db4o.Transaction i_parentTransaction;

		private readonly Db4objects.Db4o.YapWriter i_pointerIo;

		private readonly Db4objects.Db4o.YapStream i_stream;

		private Db4objects.Db4o.Foundation.List4 i_transactionListeners;

		protected Db4objects.Db4o.Foundation.Tree i_writtenUpdateDeletedMembers;

		private readonly Db4objects.Db4o.Foundation.Collection4 _participants = new Db4objects.Db4o.Foundation.Collection4
			();

		public Transaction(Db4objects.Db4o.YapStream a_stream, Db4objects.Db4o.Transaction
			 a_parent)
		{
			i_stream = a_stream;
			i_file = (a_stream is Db4objects.Db4o.YapFile) ? (Db4objects.Db4o.YapFile)a_stream
				 : null;
			i_parentTransaction = a_parent;
			i_pointerIo = new Db4objects.Db4o.YapWriter(this, Db4objects.Db4o.YapConst.POINTER_LENGTH
				);
		}

		public virtual void AddDirtyFieldIndex(Db4objects.Db4o.Inside.IX.IndexTransaction
			 a_xft)
		{
			i_dirtyFieldIndexes = new Db4objects.Db4o.Foundation.List4(i_dirtyFieldIndexes, a_xft
				);
		}

		public void CheckSynchronization()
		{
		}

		public virtual void AddTransactionListener(Db4objects.Db4o.ITransactionListener a_listener
			)
		{
			i_transactionListeners = new Db4objects.Db4o.Foundation.List4(i_transactionListeners
				, a_listener);
		}

		private void AppendSlotChanges(Db4objects.Db4o.YapReader writer)
		{
			if (i_parentTransaction != null)
			{
				i_parentTransaction.AppendSlotChanges(writer);
			}
			Db4objects.Db4o.Foundation.Tree.Traverse(_slotChanges, new _AnonymousInnerClass70
				(this, writer));
		}

		private sealed class _AnonymousInnerClass70 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass70(Transaction _enclosing, Db4objects.Db4o.YapReader writer
				)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.TreeInt)obj).Write(writer);
			}

			private readonly Transaction _enclosing;

			private readonly Db4objects.Db4o.YapReader writer;
		}

		private void ClearAll()
		{
			_slotChanges = null;
			i_dirtyFieldIndexes = null;
			i_transactionListeners = null;
			DisposeParticipants();
			_participants.Clear();
		}

		private void DisposeParticipants()
		{
			System.Collections.IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((Db4objects.Db4o.ITransactionParticipant)iterator.Current).Dispose(this);
			}
		}

		public virtual void Close(bool a_rollbackOnClose)
		{
			try
			{
				if (Stream() != null)
				{
					CheckSynchronization();
					Stream().ReleaseSemaphores(this);
				}
			}
			catch (System.Exception e)
			{
			}
			if (a_rollbackOnClose)
			{
				try
				{
					Rollback();
				}
				catch (System.Exception e)
				{
				}
			}
		}

		public virtual void Commit()
		{
			lock (Stream().i_lock)
			{
				i_file.FreeSpaceBeginCommit();
				CommitExceptForFreespace();
				i_file.FreeSpaceEndCommit();
			}
		}

		private void CommitExceptForFreespace()
		{
			Commit2Listeners();
			Commit3Stream();
			Commit4FieldIndexes();
			Commit5Participants();
			Stream().WriteDirty();
			Commit6WriteChanges();
			FreeOnCommit();
			Commit7ClearAll();
		}

		private void Commit2Listeners()
		{
			if (i_parentTransaction != null)
			{
				i_parentTransaction.Commit2Listeners();
			}
			CommitTransactionListeners();
		}

		private void Commit3Stream()
		{
			Stream().CheckNeededUpdates();
			Stream().WriteDirty();
			Stream().ClassCollection().Write(Stream().GetSystemTransaction());
		}

		private void Commit4FieldIndexes()
		{
			if (i_parentTransaction != null)
			{
				i_parentTransaction.Commit4FieldIndexes();
			}
			if (i_dirtyFieldIndexes != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_dirtyFieldIndexes
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Inside.IX.IndexTransaction)i.Current).Commit();
				}
			}
		}

		private void Commit5Participants()
		{
			if (i_parentTransaction != null)
			{
				i_parentTransaction.Commit5Participants();
			}
			System.Collections.IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((Db4objects.Db4o.ITransactionParticipant)iterator.Current).Commit(this);
			}
		}

		private void Commit6WriteChanges()
		{
			CheckSynchronization();
			int slotSetPointerCount = CountSlotChanges();
			if (slotSetPointerCount > 0)
			{
				int length = (((slotSetPointerCount * 3) + 2) * Db4objects.Db4o.YapConst.INT_LENGTH
					);
				int address = i_file.GetSlot(length);
				Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(this, address, length
					);
				bytes.WriteInt(length);
				bytes.WriteInt(slotSetPointerCount);
				AppendSlotChanges(bytes);
				bytes.Write();
				FlushFile();
				Stream().WriteTransactionPointer(address);
				FlushFile();
				if (WriteSlots())
				{
					FlushFile();
				}
				Stream().WriteTransactionPointer(0);
				FlushFile();
				i_file.Free(address, length);
			}
		}

		private void Commit7ClearAll()
		{
			if (i_parentTransaction != null)
			{
				i_parentTransaction.Commit7ClearAll();
			}
			ClearAll();
		}

		protected virtual void CommitTransactionListeners()
		{
			CheckSynchronization();
			if (i_transactionListeners != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_transactionListeners
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.ITransactionListener)i.Current).PreCommit();
				}
				i_transactionListeners = null;
			}
		}

		private int CountSlotChanges()
		{
			int count = 0;
			if (i_parentTransaction != null)
			{
				count += i_parentTransaction.CountSlotChanges();
			}
			int[] slotSetPointerCount = { count };
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass243(this, slotSetPointerCount));
			}
			return slotSetPointerCount[0];
		}

		private sealed class _AnonymousInnerClass243 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass243(Transaction _enclosing, int[] slotSetPointerCount)
			{
				this._enclosing = _enclosing;
				this.slotSetPointerCount = slotSetPointerCount;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Inside.Slots.SlotChange slot = (Db4objects.Db4o.Inside.Slots.SlotChange
					)obj;
				if (slot.IsSetPointer())
				{
					slotSetPointerCount[0]++;
				}
			}

			private readonly Transaction _enclosing;

			private readonly int[] slotSetPointerCount;
		}

		public virtual bool Delete(Db4objects.Db4o.YapObject @ref, int id, int cascade)
		{
			CheckSynchronization();
			if (@ref != null)
			{
				if (!i_stream.FlagForDelete(@ref))
				{
					return false;
				}
			}
			if (SlotChangeIsFlaggedDeleted(id))
			{
				return false;
			}
			Db4objects.Db4o.DeleteInfo info = (Db4objects.Db4o.DeleteInfo)Db4objects.Db4o.TreeInt
				.Find(i_delete, id);
			if (info == null)
			{
				info = new Db4objects.Db4o.DeleteInfo(id, @ref, cascade);
				i_delete = Db4objects.Db4o.Foundation.Tree.Add(i_delete, info);
				return true;
			}
			info._reference = @ref;
			if (cascade > info._cascade)
			{
				info._cascade = cascade;
			}
			return true;
		}

		public virtual void DontDelete(int a_id)
		{
			if (i_delete == null)
			{
				return;
			}
			i_delete = Db4objects.Db4o.TreeInt.RemoveLike((Db4objects.Db4o.TreeInt)i_delete, 
				a_id);
		}

		internal virtual void DontRemoveFromClassIndex(int a_yapClassID, int a_id)
		{
			CheckSynchronization();
			Db4objects.Db4o.YapClass yapClass = Stream().GetYapClass(a_yapClassID);
			yapClass.Index().Add(this, a_id);
		}

		private Db4objects.Db4o.Inside.Slots.SlotChange FindSlotChange(int a_id)
		{
			CheckSynchronization();
			return (Db4objects.Db4o.Inside.Slots.SlotChange)Db4objects.Db4o.TreeInt.Find(_slotChanges
				, a_id);
		}

		private void FlushFile()
		{
			if (i_file.ConfigImpl().FlushFileBuffers())
			{
				i_file.SyncFiles();
			}
		}

		private void FreeOnCommit()
		{
			CheckSynchronization();
			if (i_parentTransaction != null)
			{
				i_parentTransaction.FreeOnCommit();
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass324(this));
			}
		}

		private sealed class _AnonymousInnerClass324 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass324(Transaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.Inside.Slots.SlotChange)obj).FreeDuringCommit(this._enclosing.i_file
					);
			}

			private readonly Transaction _enclosing;
		}

		public virtual Db4objects.Db4o.Inside.Slots.Slot GetCurrentSlotOfID(int id)
		{
			CheckSynchronization();
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Inside.Slots.SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				if (change.IsSetPointer())
				{
					return change.NewSlot();
				}
			}
			if (i_parentTransaction != null)
			{
				Db4objects.Db4o.Inside.Slots.Slot parentSlot = i_parentTransaction.GetCurrentSlotOfID
					(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		public virtual Db4objects.Db4o.Inside.Slots.Slot GetCommittedSlotOfID(int id)
		{
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Inside.Slots.SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				Db4objects.Db4o.Inside.Slots.Slot slot = change.OldSlot();
				if (slot != null)
				{
					return slot;
				}
			}
			if (i_parentTransaction != null)
			{
				Db4objects.Db4o.Inside.Slots.Slot parentSlot = i_parentTransaction.GetCommittedSlotOfID
					(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		private Db4objects.Db4o.Inside.Slots.Slot ReadCommittedSlotOfID(int id)
		{
			i_file.ReadBytes(_pointerBuffer, id, Db4objects.Db4o.YapConst.POINTER_LENGTH);
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			return new Db4objects.Db4o.Inside.Slots.Slot(address, length);
		}

		private bool SlotChangeIsFlaggedDeleted(int id)
		{
			Db4objects.Db4o.Inside.Slots.SlotChange slot = FindSlotChange(id);
			if (slot != null)
			{
				return slot.IsDeleted();
			}
			if (i_parentTransaction != null)
			{
				return i_parentTransaction.SlotChangeIsFlaggedDeleted(id);
			}
			return false;
		}

		public virtual bool IsDeleted(int id)
		{
			return SlotChangeIsFlaggedDeleted(id);
		}

		public virtual object[] ObjectAndYapObjectBySignature(long a_uuid, byte[] a_signature
			)
		{
			CheckSynchronization();
			return Stream().GetFieldUUID().ObjectAndYapObjectBySignature(this, a_uuid, a_signature
				);
		}

		public virtual void ProcessDeletes()
		{
			if (i_delete == null)
			{
				i_writtenUpdateDeletedMembers = null;
				return;
			}
			while (i_delete != null)
			{
				Db4objects.Db4o.Foundation.Tree delete = i_delete;
				i_delete = null;
				delete.Traverse(new _AnonymousInnerClass425(this));
			}
			i_writtenUpdateDeletedMembers = null;
		}

		private sealed class _AnonymousInnerClass425 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass425(Transaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.DeleteInfo info = (Db4objects.Db4o.DeleteInfo)a_object;
				object obj = null;
				if (info._reference != null)
				{
					obj = info._reference.GetObject();
				}
				if (obj == null)
				{
					object[] arr = this._enclosing.Stream().GetObjectAndYapObjectByID(this._enclosing
						, info._key);
					obj = arr[0];
					info._reference = (Db4objects.Db4o.YapObject)arr[1];
					info._reference.FlagForDelete(this._enclosing.Stream().TopLevelCallId());
				}
				this._enclosing.Stream().Delete3(this._enclosing, info._reference, info._cascade, 
					false);
			}

			private readonly Transaction _enclosing;
		}

		private Db4objects.Db4o.Inside.Slots.SlotChange ProduceSlotChange(int id)
		{
			Db4objects.Db4o.Inside.Slots.SlotChange slot = new Db4objects.Db4o.Inside.Slots.SlotChange
				(id);
			_slotChanges = Db4objects.Db4o.Foundation.Tree.Add(_slotChanges, slot);
			return (Db4objects.Db4o.Inside.Slots.SlotChange)slot.DuplicateOrThis();
		}

		public virtual Db4objects.Db4o.Reflect.IReflector Reflector()
		{
			return Stream().Reflector();
		}

		public virtual void Rollback()
		{
			lock (Stream().i_lock)
			{
				RollbackParticipants();
				RollbackFieldIndexes();
				RollbackSlotChanges();
				RollBackTransactionListeners();
				ClearAll();
			}
		}

		private void RollbackSlotChanges()
		{
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass482(this));
			}
		}

		private sealed class _AnonymousInnerClass482 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass482(Transaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Inside.Slots.SlotChange)a_object).Rollback(this._enclosing.i_file
					);
			}

			private readonly Transaction _enclosing;
		}

		private void RollbackFieldIndexes()
		{
			if (i_dirtyFieldIndexes != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_dirtyFieldIndexes
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.Inside.IX.IndexTransaction)i.Current).Rollback();
				}
			}
		}

		private void RollbackParticipants()
		{
			System.Collections.IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((Db4objects.Db4o.ITransactionParticipant)iterator.Current).Rollback(this);
			}
		}

		protected virtual void RollBackTransactionListeners()
		{
			CheckSynchronization();
			if (i_transactionListeners != null)
			{
				System.Collections.IEnumerator i = new Db4objects.Db4o.Foundation.Iterator4Impl(i_transactionListeners
					);
				while (i.MoveNext())
				{
					((Db4objects.Db4o.ITransactionListener)i.Current).PostRollback();
				}
				i_transactionListeners = null;
			}
		}

		internal virtual void SetAddress(int a_address)
		{
			i_address = a_address;
		}

		public virtual void SetPointer(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).SetPointer(a_address, a_length);
		}

		internal virtual void SlotDelete(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			if (a_id == 0)
			{
				return;
			}
			Db4objects.Db4o.Inside.Slots.SlotChange slot = ProduceSlotChange(a_id);
			slot.FreeOnCommit(i_file, new Db4objects.Db4o.Inside.Slots.Slot(a_address, a_length
				));
			slot.SetPointer(0, 0);
		}

		public virtual void SlotFreeOnCommit(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			if (a_id == 0)
			{
				return;
			}
			ProduceSlotChange(a_id).FreeOnCommit(i_file, new Db4objects.Db4o.Inside.Slots.Slot
				(a_address, a_length));
		}

		internal virtual void SlotFreeOnRollback(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).FreeOnRollback(a_address, a_length);
		}

		internal virtual void SlotFreeOnRollbackCommitSetPointer(int a_id, int newAddress
			, int newLength)
		{
			Db4objects.Db4o.Inside.Slots.Slot slot = GetCurrentSlotOfID(a_id);
			CheckSynchronization();
			Db4objects.Db4o.Inside.Slots.SlotChange change = ProduceSlotChange(a_id);
			change.FreeOnRollbackSetPointer(newAddress, newLength);
			change.FreeOnCommit(i_file, slot);
		}

		internal virtual void SlotFreeOnRollbackSetPointer(int a_id, int a_address, int a_length
			)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).FreeOnRollbackSetPointer(a_address, a_length);
		}

		public virtual void SlotFreePointerOnCommit(int a_id)
		{
			CheckSynchronization();
			Db4objects.Db4o.Inside.Slots.Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			SlotFreeOnCommit(a_id, slot._address, slot._length);
		}

		internal virtual void SlotFreePointerOnCommit(int a_id, int a_address, int a_length
			)
		{
			CheckSynchronization();
			SlotFreeOnCommit(a_address, a_address, a_length);
			SlotFreeOnCommit(a_id, a_id, Db4objects.Db4o.YapConst.POINTER_LENGTH);
		}

		internal virtual bool SupportsVirtualFields()
		{
			return true;
		}

		public virtual Db4objects.Db4o.Transaction SystemTransaction()
		{
			if (i_parentTransaction != null)
			{
				return i_parentTransaction;
			}
			return this;
		}

		public override string ToString()
		{
			return Stream().ToString();
		}

		internal virtual void WriteOld()
		{
			lock (Stream().i_lock)
			{
				i_pointerIo.UseSlot(i_address);
				i_pointerIo.Read();
				int length = i_pointerIo.ReadInt();
				if (length > 0)
				{
					Db4objects.Db4o.YapWriter bytes = new Db4objects.Db4o.YapWriter(this, i_address, 
						length);
					bytes.Read();
					bytes.IncrementOffset(Db4objects.Db4o.YapConst.INT_LENGTH);
					_slotChanges = new Db4objects.Db4o.TreeReader(bytes, new Db4objects.Db4o.Inside.Slots.SlotChange
						(0)).Read();
					if (WriteSlots())
					{
						FlushFile();
					}
					Stream().WriteTransactionPointer(0);
					FlushFile();
					FreeOnCommit();
				}
				else
				{
					Stream().WriteTransactionPointer(0);
					FlushFile();
				}
			}
		}

		public virtual void WritePointer(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			i_pointerIo.UseSlot(a_id);
			i_pointerIo.WriteInt(a_address);
			i_pointerIo.WriteInt(a_length);
			if (Db4objects.Db4o.Debug.xbytes && Db4objects.Db4o.Deploy.overwrite)
			{
				i_pointerIo.SetID(Db4objects.Db4o.YapConst.IGNORE_ID);
			}
			i_pointerIo.Write();
		}

		private bool WriteSlots()
		{
			CheckSynchronization();
			bool ret = false;
			if (i_parentTransaction != null)
			{
				if (i_parentTransaction.WriteSlots())
				{
					ret = true;
				}
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass684(this));
				ret = true;
			}
			return ret;
		}

		private sealed class _AnonymousInnerClass684 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass684(Transaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Inside.Slots.SlotChange)a_object).WritePointer(this._enclosing);
			}

			private readonly Transaction _enclosing;
		}

		public virtual void WriteUpdateDeleteMembers(int a_id, Db4objects.Db4o.YapClass a_yc
			, int a_type, int a_cascade)
		{
			CheckSynchronization();
			if (Db4objects.Db4o.Foundation.Tree.Find(i_writtenUpdateDeletedMembers, new Db4objects.Db4o.TreeInt
				(a_id)) != null)
			{
				return;
			}
			i_writtenUpdateDeletedMembers = Db4objects.Db4o.Foundation.Tree.Add(i_writtenUpdateDeletedMembers
				, new Db4objects.Db4o.TreeInt(a_id));
			Db4objects.Db4o.YapWriter objectBytes = Stream().ReadWriterByID(this, a_id);
			if (objectBytes == null)
			{
				if (a_yc.HasIndex())
				{
					DontRemoveFromClassIndex(a_yc.GetID(), a_id);
				}
				return;
			}
			Db4objects.Db4o.Inside.Marshall.ObjectHeader oh = new Db4objects.Db4o.Inside.Marshall.ObjectHeader
				(Stream(), a_yc, objectBytes);
			Db4objects.Db4o.DeleteInfo info = (Db4objects.Db4o.DeleteInfo)Db4objects.Db4o.TreeInt
				.Find(i_delete, a_id);
			if (info != null)
			{
				if (info._cascade > a_cascade)
				{
					a_cascade = info._cascade;
				}
			}
			objectBytes.SetCascadeDeletes(a_cascade);
			a_yc.DeleteMembers(oh._marshallerFamily, oh._headerAttributes, objectBytes, a_type
				, true);
			SlotFreeOnCommit(a_id, objectBytes.GetAddress(), objectBytes.GetLength());
		}

		public Db4objects.Db4o.YapStream Stream()
		{
			return i_stream;
		}

		public virtual void Enlist(Db4objects.Db4o.ITransactionParticipant participant)
		{
			if (null == participant)
			{
				throw new System.ArgumentNullException("participant");
			}
			CheckSynchronization();
			if (!_participants.ContainsByIdentity(participant))
			{
				_participants.Add(participant);
			}
		}

		public static Db4objects.Db4o.Transaction ReadInterruptedTransaction(Db4objects.Db4o.YapFile
			 file, Db4objects.Db4o.YapReader reader)
		{
			int transactionID1 = reader.ReadInt();
			int transactionID2 = reader.ReadInt();
			if ((transactionID1 > 0) && (transactionID1 == transactionID2))
			{
				Db4objects.Db4o.Transaction transaction = file.NewTransaction(null);
				transaction.SetAddress(transactionID1);
				return transaction;
			}
			return null;
		}

		public virtual Db4objects.Db4o.Transaction ParentTransaction()
		{
			return i_parentTransaction;
		}
	}
}
