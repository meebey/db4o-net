using System;
using System.Collections;
using System.IO;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LocalTransaction : Transaction
	{
		private readonly byte[] _pointerBuffer = new byte[Const4.POINTER_LENGTH];

		protected readonly StatefulBuffer i_pointerIo;

		private int i_address;

		private readonly Collection4 _participants = new Collection4();

		private Tree _slotChanges;

		private Tree _writtenUpdateDeletedMembers;

		private readonly LocalObjectContainer _file;

		public LocalTransaction(ObjectContainerBase container, Transaction parent) : base
			(container, parent)
		{
			_file = (LocalObjectContainer)container;
			i_pointerIo = new StatefulBuffer(this, Const4.POINTER_LENGTH);
		}

		public virtual LocalObjectContainer File()
		{
			return _file;
		}

		public override void Commit()
		{
			Commit(null);
		}

		public virtual void Commit(IServerMessageDispatcher dispatcher)
		{
			lock (Stream().i_lock)
			{
				if (DoCommittingCallbacks())
				{
					Callbacks().CommitOnStarted(this, CollectCallbackObjectInfos(dispatcher));
				}
				_file.FreeSpaceBeginCommit();
				CommitExceptForFreespace();
				CallbackObjectInfoCollections committedInfo = null;
				if (DoCommittedCallbacks())
				{
					committedInfo = CollectCallbackObjectInfos(dispatcher);
				}
				CommitClearAll();
				_file.FreeSpaceEndCommit();
				if (DoCommittedCallbacks())
				{
					if (dispatcher == null)
					{
						Callbacks().CommitOnCompleted(this, committedInfo);
					}
					else
					{
						dispatcher.CommittedInfo(committedInfo);
					}
				}
			}
		}

		private bool DoCommittedCallbacks()
		{
			return !IsSystemTransaction();
		}

		private bool DoCommittingCallbacks()
		{
			return !IsSystemTransaction() && Callbacks().CaresAboutCommitting();
		}

		public virtual void Enlist(ITransactionParticipant participant)
		{
			if (null == participant)
			{
				throw new ArgumentNullException("participant");
			}
			CheckSynchronization();
			if (!_participants.ContainsByIdentity(participant))
			{
				_participants.Add(participant);
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
			IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((ITransactionParticipant)iterator.Current).Commit(this);
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
			IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((ITransactionParticipant)iterator.Current).Dispose(this);
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
			IEnumerator iterator = _participants.GetEnumerator();
			while (iterator.MoveNext())
			{
				((ITransactionParticipant)iterator.Current).Rollback(this);
			}
		}

		protected virtual void RollbackSlotChanges()
		{
			if (_slotChanges != null)
			{
				_slotChanges.Traverse(new _AnonymousInnerClass189(this));
			}
		}

		private sealed class _AnonymousInnerClass189 : IVisitor4
		{
			public _AnonymousInnerClass189(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((SlotChange)a_object).Rollback(this._enclosing._file);
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
				int length = (((slotSetPointerCount * 3) + 2) * Const4.INT_LENGTH);
				int address = _file.GetSlot(length);
				StatefulBuffer bytes = new StatefulBuffer(this, address, length);
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
			if (Debug.xbytes && Deploy.overwrite)
			{
				i_pointerIo.SetID(Const4.IGNORE_ID);
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
				_slotChanges.Traverse(new _AnonymousInnerClass268(this));
				ret = true;
			}
			return ret;
		}

		private sealed class _AnonymousInnerClass268 : IVisitor4
		{
			public _AnonymousInnerClass268(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((SlotChange)a_object).WritePointer(this._enclosing);
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

		private SlotChange ProduceSlotChange(int id)
		{
			SlotChange slot = new SlotChange(id);
			_slotChanges = Tree.Add(_slotChanges, slot);
			return (SlotChange)slot.AddedOrExisting();
		}

		private SlotChange FindSlotChange(int a_id)
		{
			CheckSynchronization();
			return (SlotChange)TreeInt.Find(_slotChanges, a_id);
		}

		public virtual Slot GetCurrentSlotOfID(int id)
		{
			CheckSynchronization();
			if (id == 0)
			{
				return null;
			}
			SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				if (change.IsSetPointer())
				{
					return change.NewSlot();
				}
			}
			if (_systemTransaction != null)
			{
				Slot parentSlot = ParentLocalTransaction().GetCurrentSlotOfID(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		public virtual Slot GetCommittedSlotOfID(int id)
		{
			if (id == 0)
			{
				return null;
			}
			SlotChange change = FindSlotChange(id);
			if (change != null)
			{
				Slot slot = change.OldSlot();
				if (slot != null)
				{
					return slot;
				}
			}
			if (_systemTransaction != null)
			{
				Slot parentSlot = ParentLocalTransaction().GetCommittedSlotOfID(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadCommittedSlotOfID(id);
		}

		private Slot ReadCommittedSlotOfID(int id)
		{
			try
			{
				_file.ReadBytes(_pointerBuffer, id, Const4.POINTER_LENGTH);
			}
			catch (IOException exc)
			{
				throw new SlotRetrievalException(exc, id);
			}
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			return new Slot(address, length);
		}

		private Slot DebugReadCommittedSlotOfID(int id)
		{
			try
			{
				i_pointerIo.UseSlot(id);
				i_pointerIo.Read();
				i_pointerIo.ReadBegin(Const4.YAPPOINTER);
				int debugAddress = i_pointerIo.ReadInt();
				int debugLength = i_pointerIo.ReadInt();
				i_pointerIo.ReadEnd();
				return new Slot(debugAddress, debugLength);
			}
			catch (IOException exc)
			{
				throw new SlotRetrievalException(exc, id);
			}
		}

		public override void SetPointer(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).SetPointer(a_address, a_length);
		}

		private bool SlotChangeIsFlaggedDeleted(int id)
		{
			SlotChange slot = FindSlotChange(id);
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
				_slotChanges.Traverse(new _AnonymousInnerClass411(this, slotSetPointerCount));
			}
			return slotSetPointerCount[0];
		}

		private sealed class _AnonymousInnerClass411 : IVisitor4
		{
			public _AnonymousInnerClass411(LocalTransaction _enclosing, int[] slotSetPointerCount
				)
			{
				this._enclosing = _enclosing;
				this.slotSetPointerCount = slotSetPointerCount;
			}

			public void Visit(object obj)
			{
				SlotChange slot = (SlotChange)obj;
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
					StatefulBuffer bytes = new StatefulBuffer(this, i_address, length);
					bytes.Read();
					bytes.IncrementOffset(Const4.INT_LENGTH);
					_slotChanges = new TreeReader(bytes, new SlotChange(0)).Read();
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
				_slotChanges.Traverse(new _AnonymousInnerClass453(this));
			}
		}

		private sealed class _AnonymousInnerClass453 : IVisitor4
		{
			public _AnonymousInnerClass453(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).FreeDuringCommit(this._enclosing._file);
			}

			private readonly LocalTransaction _enclosing;
		}

		private void AppendSlotChanges(Db4objects.Db4o.Internal.Buffer writer)
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().AppendSlotChanges(writer);
			}
			Tree.Traverse(_slotChanges, new _AnonymousInnerClass467(this, writer));
		}

		private sealed class _AnonymousInnerClass467 : IVisitor4
		{
			public _AnonymousInnerClass467(LocalTransaction _enclosing, Db4objects.Db4o.Internal.Buffer
				 writer)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((TreeInt)obj).Write(writer);
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
			SlotChange slot = ProduceSlotChange(a_id);
			slot.FreeOnCommit(_file, new Slot(a_address, a_length));
			slot.SetPointer(0, 0);
		}

		private void SlotFreeOnCommit(int id, Slot slot)
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
			ProduceSlotChange(a_id).FreeOnCommit(_file, new Slot(a_address, a_length));
		}

		public override void SlotFreeOnRollback(int a_id, int a_address, int a_length)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).FreeOnRollback(a_address, a_length);
		}

		internal override void SlotFreeOnRollbackCommitSetPointer(int a_id, int newAddress
			, int newLength)
		{
			Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			CheckSynchronization();
			SlotChange change = ProduceSlotChange(a_id);
			change.FreeOnRollbackSetPointer(newAddress, newLength);
			change.FreeOnCommit(_file, slot);
		}

		internal override void ProduceUpdateSlotChange(int a_id, int a_address, int a_length
			)
		{
			CheckSynchronization();
			SlotChange slotChange = ProduceSlotChange(a_id);
			slotChange.FreeOnRollbackSetPointer(a_address, a_length);
		}

		public override void SlotFreePointerOnCommit(int a_id)
		{
			CheckSynchronization();
			Slot slot = GetCurrentSlotOfID(a_id);
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
			SlotFreeOnCommit(a_id, a_id, Const4.POINTER_LENGTH);
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
				Tree delete = i_delete;
				i_delete = null;
				delete.Traverse(new _AnonymousInnerClass591(this));
			}
			_writtenUpdateDeletedMembers = null;
		}

		private sealed class _AnonymousInnerClass591 : IVisitor4
		{
			public _AnonymousInnerClass591(LocalTransaction _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				DeleteInfo info = (DeleteInfo)a_object;
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
					HardObjectReference hardRef = this._enclosing.Stream().GetHardObjectReferenceById
						(this._enclosing, info._key);
					info._reference = hardRef._reference;
					info._reference.FlagForDelete(this._enclosing.Stream().TopLevelCallId());
				}
				this._enclosing.Stream().Delete3(this._enclosing, info._reference, info._cascade, 
					false);
			}

			private readonly LocalTransaction _enclosing;
		}

		public override void WriteUpdateDeleteMembers(int id, ClassMetadata clazz, int typeInfo
			, int cascade)
		{
			CheckSynchronization();
			TreeInt newNode = new TreeInt(id);
			_writtenUpdateDeletedMembers = Tree.Add(_writtenUpdateDeletedMembers, newNode);
			if (!newNode.WasAddedToTree())
			{
				return;
			}
			if (clazz.CanUpdateFast())
			{
				SlotFreeOnCommit(id, GetCurrentSlotOfID(id));
				return;
			}
			StatefulBuffer objectBytes = Stream().ReadWriterByID(this, id);
			if (objectBytes == null)
			{
				if (clazz.HasIndex())
				{
					DontRemoveFromClassIndex(clazz.GetID(), id);
				}
				return;
			}
			ObjectHeader oh = new ObjectHeader(Stream(), clazz, objectBytes);
			DeleteInfo info = (DeleteInfo)TreeInt.Find(i_delete, id);
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
			SlotFreeOnCommit(id, new Slot(objectBytes.GetAddress(), objectBytes.GetLength()));
		}

		private ICallbacks Callbacks()
		{
			return Stream().Callbacks();
		}

		private CallbackObjectInfoCollections CollectCallbackObjectInfos(IServerMessageDispatcher
			 serverMessageDispatcher)
		{
			if (null == _slotChanges)
			{
				return CallbackObjectInfoCollections.EMTPY;
			}
			Collection4 added = new Collection4();
			Collection4 deleted = new Collection4();
			Collection4 updated = new Collection4();
			_slotChanges.Traverse(new _AnonymousInnerClass678(this, deleted, added, updated));
			return new CallbackObjectInfoCollections(serverMessageDispatcher, new ObjectInfoCollectionImpl
				(added), new ObjectInfoCollectionImpl(updated), new ObjectInfoCollectionImpl(deleted
				));
		}

		private sealed class _AnonymousInnerClass678 : IVisitor4
		{
			public _AnonymousInnerClass678(LocalTransaction _enclosing, Collection4 deleted, 
				Collection4 added, Collection4 updated)
			{
				this._enclosing = _enclosing;
				this.deleted = deleted;
				this.added = added;
				this.updated = updated;
			}

			public void Visit(object obj)
			{
				SlotChange slotChange = ((SlotChange)obj);
				LazyObjectReference lazyRef = new LazyObjectReference(this._enclosing, slotChange
					._key);
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

			private readonly Collection4 deleted;

			private readonly Collection4 added;

			private readonly Collection4 updated;
		}

		private void SetAddress(int a_address)
		{
			i_address = a_address;
		}

		public static Transaction ReadInterruptedTransaction(LocalObjectContainer file, Db4objects.Db4o.Internal.Buffer
			 reader)
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
