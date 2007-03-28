namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LocalTransaction : Db4objects.Db4o.Internal.Transaction
	{
		private readonly byte[] _pointerBuffer = new byte[Db4objects.Db4o.Internal.Const4
			.POINTER_LENGTH];

		protected readonly Db4objects.Db4o.Internal.StatefulBuffer i_pointerIo;

		private int i_address;

		private readonly Db4objects.Db4o.Foundation.Collection4 _participants = new Db4objects.Db4o.Foundation.Collection4
			();

		private Db4objects.Db4o.Foundation.Tree _slotChanges;

		private Db4objects.Db4o.Foundation.Tree _writtenUpdateDeletedMembers;

		private readonly Db4objects.Db4o.Internal.LocalObjectContainer _file;

		public LocalTransaction(Db4objects.Db4o.Internal.ObjectContainerBase container, Db4objects.Db4o.Internal.Transaction
			 parent) : base(container, parent)
		{
			_file = (Db4objects.Db4o.Internal.LocalObjectContainer)container;
			i_pointerIo = new Db4objects.Db4o.Internal.StatefulBuffer(this, Db4objects.Db4o.Internal.Const4
				.POINTER_LENGTH);
		}

		public virtual Db4objects.Db4o.Internal.LocalObjectContainer File()
		{
			return _file;
		}

		public override void Commit()
		{
			lock (Stream().i_lock)
			{
				if (!IsSystemTransaction())
				{
					TriggerCommitOnStarted();
				}
				_file.FreeSpaceBeginCommit();
				CommitExceptForFreespace();
				if (!IsSystemTransaction())
				{
					TriggerCommitOnCompleted();
				}
				CommitClearAll();
				_file.FreeSpaceEndCommit();
			}
		}

		public virtual void Enlist(Db4objects.Db4o.Internal.ITransactionParticipant participant
			)
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

		private void TriggerCommitOnCompleted()
		{
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
		}

		private void Commit2Listeners()
		{
			CommitParentListeners();
			CommitTransactionListeners();
		}

		private void CommitParentListeners()
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().Commit2Listeners();
			}
		}

		private void CommitParticipants()
		{
			if (ParentLocalTransaction() != null)
			{
				ParentLocalTransaction().CommitParticipants();
			}
			System.Collections.IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((Db4objects.Db4o.Internal.ITransactionParticipant)iterator.Current).Commit(this);
			}
		}

		private void Commit3Stream()
		{
			Stream().ProcessPendingClassUpdates();
			Stream().WriteDirty();
			Stream().ClassCollection().Write(Stream().SystemTransaction());
		}

		private Db4objects.Db4o.Internal.LocalTransaction ParentLocalTransaction()
		{
			return (Db4objects.Db4o.Internal.LocalTransaction)_systemTransaction;
		}

		private void CommitClearAll()
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().CommitClearAll();
			}
			ClearAll();
		}

		protected override void Clear()
		{
			_slotChanges = null;
			DisposeParticipants();
			_participants.Clear();
		}

		private void DisposeParticipants()
		{
			System.Collections.IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((Db4objects.Db4o.Internal.ITransactionParticipant)iterator.Current).Dispose(this
					);
			}
		}

		public override void Rollback()
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

		private void RollbackParticipants()
		{
			System.Collections.IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((Db4objects.Db4o.Internal.ITransactionParticipant)iterator.Current).Rollback(this
					);
			}
		}

		protected virtual void RollbackSlotChanges()
		{
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass173(this));
			}
		}

		private sealed class _AnonymousInnerClass173 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass173(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((Db4objects.Db4o.Internal.Slots.SlotChange)a_object).Rollback(this._enclosing._file
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
				int address = _file.GetSlot(length);
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
				_file.Free(address, length);
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
				i_pointerIo.SetID(Db4objects.Db4o.Internal.Const4.IGNORE_ID);
			}
			i_pointerIo.Write();
		}

		private bool WriteSlots()
		{
			CheckSynchronization();
			bool ret = false;
			if (_systemTransaction != null)
			{
				if (ParentLocalTransaction().WriteSlots())
				{
					ret = true;
				}
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass252(this));
				ret = true;
			}
			return ret;
		}

		private sealed class _AnonymousInnerClass252 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass252(LocalTransaction _enclosing)
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
			if (_file.ConfigImpl().FlushFileBuffers())
			{
				_file.SyncFiles();
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
			if (_systemTransaction != null)
			{
				Db4objects.Db4o.Internal.Slots.Slot parentSlot = ParentLocalTransaction().GetCurrentSlotOfID
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
			if (_systemTransaction != null)
			{
				Db4objects.Db4o.Internal.Slots.Slot parentSlot = ParentLocalTransaction().GetCommittedSlotOfID
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
				_file.ReadBytes(_pointerBuffer, id, Db4objects.Db4o.Internal.Const4.POINTER_LENGTH
					);
			}
			catch (System.IO.IOException exc)
			{
				throw new Db4objects.Db4o.Internal.SlotRetrievalException(exc, id);
			}
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			return new Db4objects.Db4o.Internal.Slots.Slot(address, length);
		}

		private Db4objects.Db4o.Internal.Slots.Slot DebugReadCommittedSlotOfID(int id)
		{
			try
			{
				i_pointerIo.UseSlot(id);
				i_pointerIo.Read();
				i_pointerIo.ReadBegin(Db4objects.Db4o.Internal.Const4.YAPPOINTER);
				int debugAddress = i_pointerIo.ReadInt();
				int debugLength = i_pointerIo.ReadInt();
				i_pointerIo.ReadEnd();
				return new Db4objects.Db4o.Internal.Slots.Slot(debugAddress, debugLength);
			}
			catch (System.IO.IOException exc)
			{
				throw new Db4objects.Db4o.Internal.SlotRetrievalException(exc, id);
			}
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
			if (_systemTransaction != null)
			{
				return ParentLocalTransaction().SlotChangeIsFlaggedDeleted(id);
			}
			return false;
		}

		private int CountSlotChanges()
		{
			int count = 0;
			if (_systemTransaction != null)
			{
				count += ParentLocalTransaction().CountSlotChanges();
			}
			int[] slotSetPointerCount = new int[] { count };
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass395(this, slotSetPointerCount));
			}
			return slotSetPointerCount[0];
		}

		private sealed class _AnonymousInnerClass395 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass395(LocalTransaction _enclosing, int[] slotSetPointerCount
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
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().FreeOnCommit();
			}
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass437(this));
			}
		}

		private sealed class _AnonymousInnerClass437 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass437(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				((Db4objects.Db4o.Internal.Slots.SlotChange)obj).FreeDuringCommit(this._enclosing
					._file);
			}

			private readonly LocalTransaction _enclosing;
		}

		private void AppendSlotChanges(Db4objects.Db4o.Internal.Buffer writer)
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().AppendSlotChanges(writer);
			}
			Db4objects.Db4o.Foundation.Tree.Traverse(_slotChanges, new _AnonymousInnerClass451
				(this, writer));
		}

		private sealed class _AnonymousInnerClass451 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass451(LocalTransaction _enclosing, Db4objects.Db4o.Internal.Buffer
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
			slot.FreeOnCommit(_file, new Db4objects.Db4o.Internal.Slots.Slot(a_address, a_length
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
			ProduceSlotChange(a_id).FreeOnCommit(_file, new Db4objects.Db4o.Internal.Slots.Slot
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
			change.FreeOnCommit(_file, slot);
		}

		internal override void ProduceUpdateSlotChange(int a_id, int a_address, int a_length
			)
		{
			CheckSynchronization();
			Db4objects.Db4o.Internal.Slots.SlotChange slotChange = ProduceSlotChange(a_id);
			slotChange.FreeOnRollbackSetPointer(a_address, a_length);
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

		public override void ProcessDeletes()
		{
			if (i_delete == null)
			{
				_writtenUpdateDeletedMembers = null;
				return;
			}
			while (i_delete != null)
			{
				Db4objects.Db4o.Foundation.Tree delete = i_delete;
				i_delete = null;
				delete.Traverse(new _AnonymousInnerClass575(this));
			}
			_writtenUpdateDeletedMembers = null;
		}

		private sealed class _AnonymousInnerClass575 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass575(LocalTransaction _enclosing)
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
			_writtenUpdateDeletedMembers = Db4objects.Db4o.Foundation.Tree.Add(_writtenUpdateDeletedMembers
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
			Db4objects.Db4o.Internal.Callbacks.ICallbacks callbacks = Stream().Callbacks();
			if (!callbacks.CaresAboutCommit())
			{
				return;
			}
			Db4objects.Db4o.Ext.IObjectInfoCollection[] collections = PartitionSlotChangesInAddedDeletedUpdated
				();
			callbacks.CommitOnStarted(this, collections[0], collections[1], collections[2]);
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
			Db4objects.Db4o.Foundation.Collection4 updated = new Db4objects.Db4o.Foundation.Collection4
				();
			_slotChanges.Traverse(new _AnonymousInnerClass686(this, deleted, added, updated));
			return new Db4objects.Db4o.Ext.IObjectInfoCollection[] { new Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl
				(added), new Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl(
				deleted), new Db4objects.Db4o.Internal.LocalTransaction.ObjectInfoCollectionImpl
				(updated) };
		}

		private sealed class _AnonymousInnerClass686 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass686(LocalTransaction _enclosing, Db4objects.Db4o.Foundation.Collection4
				 deleted, Db4objects.Db4o.Foundation.Collection4 added, Db4objects.Db4o.Foundation.Collection4
				 updated)
			{
				this._enclosing = _enclosing;
				this.deleted = deleted;
				this.added = added;
				this.updated = updated;
			}

			public void Visit(object obj)
			{
				Db4objects.Db4o.Internal.Slots.SlotChange slotChange = ((Db4objects.Db4o.Internal.Slots.SlotChange
					)obj);
				Db4objects.Db4o.Internal.LazyObjectReference lazyRef = new Db4objects.Db4o.Internal.LazyObjectReference
					(this._enclosing, slotChange._key);
				if (slotChange.IsDeleted())
				{
					deleted.Add(lazyRef);
				}
				else
				{
					if (slotChange.IsNew())
					{
						added.Add(lazyRef);
					}
					else
					{
						updated.Add(lazyRef);
					}
				}
			}

			private readonly LocalTransaction _enclosing;

			private readonly Db4objects.Db4o.Foundation.Collection4 deleted;

			private readonly Db4objects.Db4o.Foundation.Collection4 added;

			private readonly Db4objects.Db4o.Foundation.Collection4 updated;
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
