namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LocalTransaction : Db4objects.Db4o.Internal.Transaction
	{
		private readonly byte[] _pointerBuffer = new byte[Db4objects.Db4o.Internal.Const4
			.POINTER_LENGTH];

		private int i_address;

		private Db4objects.Db4o.Foundation.Tree _slotChanges;

		public LocalTransaction(Db4objects.Db4o.Internal.ObjectContainerBase a_stream, Db4objects.Db4o.Internal.Transaction
			 a_parent) : base(a_stream, a_parent)
		{
		}

		public override void Commit()
		{
			lock (Stream().i_lock)
			{
				TriggerCommitOnStarted();
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
			CommitParticipants();
			Stream().WriteDirty();
			Commit6WriteChanges();
			FreeOnCommit();
			Commit7ClearAll();
		}

		private void Commit2Listeners()
		{
			CommitParentListeners();
			CommitTransactionListeners();
		}

		private void CommitParentListeners()
		{
			if (i_parentTransaction != null)
			{
				ParentLocalTransaction().Commit2Listeners();
			}
		}

		private void Commit3Stream()
		{
			Stream().CheckNeededUpdates();
			Stream().WriteDirty();
			Stream().ClassCollection().Write(Stream().GetSystemTransaction());
		}

		private Db4objects.Db4o.Internal.LocalTransaction ParentLocalTransaction()
		{
			return (Db4objects.Db4o.Internal.LocalTransaction)i_parentTransaction;
		}

		private void Commit7ClearAll()
		{
			Commit7ParentClearAll();
			ClearAll();
		}

		private void Commit7ParentClearAll()
		{
			if (i_parentTransaction != null)
			{
				ParentLocalTransaction().Commit7ClearAll();
			}
		}

		protected override void ClearAll()
		{
			_slotChanges = null;
			base.ClearAll();
		}

		protected override void RollbackSlotChanges()
		{
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass99(this));
			}
		}

		private sealed class _AnonymousInnerClass99 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass99(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Internal.Slots.SlotChange)a_object).Rollback(this._enclosing.i_file
					);
			}

			private readonly LocalTransaction _enclosing;
		}

		public override bool IsDeleted(int id)
		{
			return SlotChangeIsFlaggedDeleted(id);
		}

		protected virtual void Commit6WriteChanges()
		{
			CheckSynchronization();
			int slotSetPointerCount = CountSlotChanges();
			if (slotSetPointerCount > 0)
			{
				int length = (((slotSetPointerCount * 3) + 2) * Db4objects.Db4o.Internal.Const4.INT_LENGTH
					);
				int address = i_file.GetSlot(length);
				Db4objects.Db4o.Internal.StatefulBuffer bytes = new Db4objects.Db4o.Internal.StatefulBuffer
					(this, address, length);
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

		private bool WriteSlots()
		{
			CheckSynchronization();
			bool ret = false;
			if (i_parentTransaction != null)
			{
				if (ParentFileTransaction().WriteSlots())
				{
					ret = true;
				}
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass155(this));
				ret = true;
			}
			return ret;
		}

		private sealed class _AnonymousInnerClass155 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass155(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Internal.Slots.SlotChange)a_object).WritePointer(this._enclosing
					);
			}

			private readonly LocalTransaction _enclosing;
		}

		protected virtual void FlushFile()
		{
			if (i_file.ConfigImpl().FlushFileBuffers())
			{
				i_file.SyncFiles();
			}
		}

		private Db4objects.Db4o.Internal.Slots.SlotChange ProduceSlotChange(int id)
		{
			Db4objects.Db4o.Internal.Slots.SlotChange slot = new Db4objects.Db4o.Internal.Slots.SlotChange
				(id);
			_slotChanges = Db4objects.Db4o.Foundation.Tree.Add(_slotChanges, slot);
			return (Db4objects.Db4o.Internal.Slots.SlotChange)slot.AddedOrExisting();
		}

		private Db4objects.Db4o.Internal.Slots.SlotChange FindSlotChange(int a_id)
		{
			CheckSynchronization();
			return (Db4objects.Db4o.Internal.Slots.SlotChange)Db4objects.Db4o.Internal.TreeInt
				.Find(_slotChanges, a_id);
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot GetCurrentSlotOfID(int id)
		{
			CheckSynchronization();
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Internal.Slots.SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				if (change.IsSetPointer())
				{
					return change.NewSlot();
				}
			}
			if (i_parentTransaction != null)
			{
				Db4objects.Db4o.Internal.Slots.Slot parentSlot = ParentFileTransaction().GetCurrentSlotOfID
					(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		public virtual Db4objects.Db4o.Internal.Slots.Slot GetCommittedSlotOfID(int id)
		{
			if (id == 0)
			{
				return null;
			}
			Db4objects.Db4o.Internal.Slots.SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				Db4objects.Db4o.Internal.Slots.Slot slot = change.OldSlot();
				if (slot != null)
				{
					return slot;
				}
			}
			if (i_parentTransaction != null)
			{
				Db4objects.Db4o.Internal.Slots.Slot parentSlot = ParentFileTransaction().GetCommittedSlotOfID
					(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		private Db4objects.Db4o.Internal.Slots.Slot ReadCommittedSlotOfID(int id)
		{
			try
			{
				i_file.ReadBytes(_pointerBuffer, id, Db4objects.Db4o.Internal.Const4.POINTER_LENGTH
					);
			}
			catch
			{
				return null;
			}
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			return new Db4objects.Db4o.Internal.Slots.Slot(address, length);
		}

		public override void SetPointer(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).SetPointer(a_address, a_length);
		}

		private bool SlotChangeIsFlaggedDeleted(int id)
		{
			Db4objects.Db4o.Internal.Slots.SlotChange slot = FindSlotChange(id);
			if (slot != null)
			{
				return slot.IsDeleted();
			}
			if (i_parentTransaction != null)
			{
				return ParentFileTransaction().SlotChangeIsFlaggedDeleted(id);
			}
			return false;
		}

		private int CountSlotChanges()
		{
			int count = 0;
			if (i_parentTransaction != null)
			{
				count += ParentFileTransaction().CountSlotChanges();
			}
			int[] slotSetPointerCount = { count };
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass292(this, slotSetPointerCount));
			}
			return slotSetPointerCount[0];
		}

		private sealed class _AnonymousInnerClass292 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass292(LocalTransaction _enclosing, int[] slotSetPointerCount
				)
			{
				this._enclosing = _enclosing;
				this.slotSetPointerCount = slotSetPointerCount;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Slots.SlotChange slot = (Db4objects.Db4o.Internal.Slots.SlotChange
					)obj;
				if (slot.IsSetPointer())
				{
					slotSetPointerCount[0]++;
				}
			}

			private readonly LocalTransaction _enclosing;

			private readonly int[] slotSetPointerCount;
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
					Db4objects.Db4o.Internal.StatefulBuffer bytes = new Db4objects.Db4o.Internal.StatefulBuffer
						(this, i_address, length);
					bytes.Read();
					bytes.IncrementOffset(Db4objects.Db4o.Internal.Const4.INT_LENGTH);
					_slotChanges = new Db4objects.Db4o.Internal.TreeReader(bytes, new Db4objects.Db4o.Internal.Slots.SlotChange
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

		protected sealed override void FreeOnCommit()
		{
			CheckSynchronization();
			if (i_parentTransaction != null)
			{
				ParentFileTransaction().FreeOnCommit();
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass334(this));
			}
		}

		private sealed class _AnonymousInnerClass334 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass334(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.Internal.Slots.SlotChange)obj).FreeDuringCommit(this._enclosing
					.i_file);
			}

			private readonly LocalTransaction _enclosing;
		}

		private void AppendSlotChanges(Db4objects.Db4o.Internal.Buffer writer)
		{
			if (i_parentTransaction != null)
			{
				ParentFileTransaction().AppendSlotChanges(writer);
			}
			Db4objects.Db4o.Foundation.Tree.Traverse(_slotChanges, new _AnonymousInnerClass348
				(this, writer));
		}

		private sealed class _AnonymousInnerClass348 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass348(LocalTransaction _enclosing, Db4objects.Db4o.Internal.Buffer
				 writer)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.Internal.TreeInt)obj).Write(writer);
			}

			private readonly LocalTransaction _enclosing;

			private readonly Db4objects.Db4o.Internal.Buffer writer;
		}

		public override void SlotDelete(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			if (a_id == 0)
			{
				return;
			}
			Db4objects.Db4o.Internal.Slots.SlotChange slot = ProduceSlotChange(a_id);
			slot.FreeOnCommit(i_file, new Db4objects.Db4o.Internal.Slots.Slot(a_address, a_length
				));
			slot.SetPointer(0, 0);
		}

		private void SlotFreeOnCommit(int id, Db4objects.Db4o.Internal.Slots.Slot slot)
		{
			if (slot == null)
			{
				return;
			}
			SlotFreeOnCommit(id, slot.GetAddress(), slot.GetLength());
		}

		public override void SlotFreeOnCommit(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			if (a_id == 0)
			{
				return;
			}
			ProduceSlotChange(a_id).FreeOnCommit(i_file, new Db4objects.Db4o.Internal.Slots.Slot
				(a_address, a_length));
		}

		public override void SlotFreeOnRollback(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).FreeOnRollback(a_address, a_length);
		}

		internal override void SlotFreeOnRollbackCommitSetPointer(int a_id, int newAddress
			, int newLength)
		{
			Db4objects.Db4o.Internal.Slots.Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			CheckSynchronization();
			Db4objects.Db4o.Internal.Slots.SlotChange change = ProduceSlotChange(a_id);
			change.FreeOnRollbackSetPointer(newAddress, newLength);
			change.FreeOnCommit(i_file, slot);
		}

		internal override void SlotFreeOnRollbackSetPointer(int a_id, int a_address, int 
			a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).FreeOnRollbackSetPointer(a_address, a_length);
		}

		public override void SlotFreePointerOnCommit(int a_id)
		{
			CheckSynchronization();
			Db4objects.Db4o.Internal.Slots.Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			SlotFreeOnCommit(a_id, slot._address, slot._length);
		}

		internal override void SlotFreePointerOnCommit(int a_id, int a_address, int a_length
			)
		{
			CheckSynchronization();
			SlotFreeOnCommit(a_address, a_address, a_length);
			SlotFreeOnCommit(a_id, a_id, Db4objects.Db4o.Internal.Const4.POINTER_LENGTH);
		}

		public override void SlotFreePointerOnRollback(int id)
		{
			ProduceSlotChange(id).FreePointerOnRollback();
		}

		private Db4objects.Db4o.Internal.LocalTransaction ParentFileTransaction()
		{
			return (Db4objects.Db4o.Internal.LocalTransaction)i_parentTransaction;
		}

		public override void ProcessDeletes()
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
				delete.Traverse(new _AnonymousInnerClass474(this));
			}
			i_writtenUpdateDeletedMembers = null;
		}

		private sealed class _AnonymousInnerClass474 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass474(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				Db4objects.Db4o.Internal.DeleteInfo info = (Db4objects.Db4o.Internal.DeleteInfo)a_object;
				if (this._enclosing.IsDeleted(info._key))
				{
					return;
				}
				object obj = null;
				if (info._reference != null)
				{
					obj = info._reference.GetObject();
				}
				if (obj == null)
				{
					Db4objects.Db4o.Internal.HardObjectReference hardRef = this._enclosing.Stream().GetHardObjectReferenceById
						(this._enclosing, info._key);
					info._reference = hardRef._reference;
					info._reference.FlagForDelete(this._enclosing.Stream().TopLevelCallId());
				}
				this._enclosing.Stream().Delete3(this._enclosing, info._reference, info._cascade, 
					false);
			}

			private readonly LocalTransaction _enclosing;
		}

		public override void WriteUpdateDeleteMembers(int id, Db4objects.Db4o.Internal.ClassMetadata
			 clazz, int typeInfo, int cascade)
		{
			CheckSynchronization();
			Db4objects.Db4o.Internal.TreeInt newNode = new Db4objects.Db4o.Internal.TreeInt(id
				);
			i_writtenUpdateDeletedMembers = Db4objects.Db4o.Foundation.Tree.Add(i_writtenUpdateDeletedMembers
				, newNode);
			if (!newNode.WasAddedToTree())
			{
				return;
			}
			if (clazz.CanUpdateFast())
			{
				SlotFreeOnCommit(id, GetCurrentSlotOfID(id));
				return;
			}
			Db4objects.Db4o.Internal.StatefulBuffer objectBytes = Stream().ReadWriterByID(this
				, id);
			if (objectBytes == null)
			{
				if (clazz.HasIndex())
				{
					DontRemoveFromClassIndex(clazz.GetID(), id);
				}
				return;
			}
			Db4objects.Db4o.Internal.Marshall.ObjectHeader oh = new Db4objects.Db4o.Internal.Marshall.ObjectHeader
				(Stream(), clazz, objectBytes);
			Db4objects.Db4o.Internal.DeleteInfo info = (Db4objects.Db4o.Internal.DeleteInfo)Db4objects.Db4o.Internal.TreeInt
				.Find(i_delete, id);
			if (info != null)
			{
				if (info._cascade > cascade)
				{
					cascade = info._cascade;
				}
			}
			objectBytes.SetCascadeDeletes(cascade);
			clazz.DeleteMembers(oh._marshallerFamily, oh._headerAttributes, objectBytes, typeInfo
				, true);
			SlotFreeOnCommit(id, new Db4objects.Db4o.Internal.Slots.Slot(objectBytes.GetAddress
				(), objectBytes.GetLength()));
		}

		private void TriggerCommitOnStarted()
		{
			Db4objects.Db4o.Ext.IObjectInfoCollection[] collections = PartitionSlotChangesInAddedDeletedUpdated
				();
			Stream().Callbacks().CommitOnStarted(collections[0], collections[1], collections[
				2]);
		}

		private sealed class ObjectInfoCollectionImpl : Db4objects.Db4o.Ext.IObjectInfoCollection
		{
			public static readonly Db4objects.Db4o.Ext.IObjectInfoCollection EMPTY = new Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl
				(Db4objects.Db4o.Foundation.Iterators.EMPTY_ITERABLE);

			private readonly System.Collections.IEnumerable _collection;

			public ObjectInfoCollectionImpl(System.Collections.IEnumerable collection)
			{
				_collection = collection;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return _collection.GetEnumerator();
			}
		}

		private Db4objects.Db4o.Ext.IObjectInfoCollection[] PartitionSlotChangesInAddedDeletedUpdated
			()
		{
			if (null == _slotChanges)
			{
				return new Db4objects.Db4o.Ext.IObjectInfoCollection[] { Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl
					.EMPTY, Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl.EMPTY
					, Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl.EMPTY };
			}
			Db4objects.Db4o.Foundation.Collection4 added = new Db4objects.Db4o.Foundation.Collection4
				();
			Db4objects.Db4o.Foundation.Collection4 deleted = new Db4objects.Db4o.Foundation.Collection4
				();
			_slotChanges.Traverse(new _AnonymousInnerClass580(this, deleted, added));
			return new Db4objects.Db4o.Ext.IObjectInfoCollection[] { new Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl
				(added), new Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl(
				deleted), Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl.EMPTY
				 };
		}

		private sealed class _AnonymousInnerClass580 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass580(LocalTransaction _enclosing, Db4objects.Db4o.Foundation.Collection4
				 deleted, Db4objects.Db4o.Foundation.Collection4 added)
			{
				this._enclosing = _enclosing;
				this.deleted = deleted;
				this.added = added;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Slots.SlotChange slotChange = ((Db4objects.Db4o.Internal.Slots.SlotChange
					)obj);
				Db4objects.Db4o.Internal.ObjectReference reference = this._enclosing.Stream().ReferenceForId
					(slotChange._key);
				if (slotChange.IsDeleted())
				{
					deleted.Add(reference);
				}
				else
				{
					added.Add(reference);
				}
			}

			private readonly LocalTransaction _enclosing;

			private readonly Db4objects.Db4o.Foundation.Collection4 deleted;

			private readonly Db4objects.Db4o.Foundation.Collection4 added;
		}

		private void SetAddress(int a_address)
		{
			i_address = a_address;
		}

		public static Db4objects.Db4o.Internal.Transaction ReadInterruptedTransaction(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, Db4objects.Db4o.Internal.Buffer reader)
		{
			int transactionID1 = reader.ReadInt();
			int transactionID2 = reader.ReadInt();
			if ((transactionID1 > 0) && (transactionID1 == transactionID2))
			{
				Db4objects.Db4o.Internal.LocalTransaction transaction = (Db4objects.Db4o.Internal.LocalTransaction
					)file.NewTransaction(null);
				transaction.SetAddress(transactionID1);
				return transaction;
			}
			return null;
		}
	}
}
