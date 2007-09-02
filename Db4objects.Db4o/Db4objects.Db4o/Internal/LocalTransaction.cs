/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS;
using Db4objects.Db4o.Internal.Callbacks;
using Db4objects.Db4o.Internal.Freespace;
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

		private readonly LockedTree _slotChanges = new LockedTree();

		private Tree _writtenUpdateDeletedMembers;

		protected readonly LocalObjectContainer _file;

		public LocalTransaction(ObjectContainerBase container, Transaction parentTransaction
			, TransactionalReferenceSystem referenceSystem) : base(container, parentTransaction
			, referenceSystem)
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
			lock (Container()._lock)
			{
				if (DoCommittingCallbacks())
				{
					Callbacks().CommitOnStarted(this, CollectCallbackObjectInfos(dispatcher));
				}
				FreespaceBeginCommit();
				CommitImpl();
				CallbackObjectInfoCollections committedInfo = null;
				if (DoCommittedCallbacks(dispatcher))
				{
					committedInfo = CollectCallbackObjectInfos(dispatcher);
				}
				CommitClearAll();
				FreespaceEndCommit();
				if (DoCommittedCallbacks(dispatcher))
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

		private bool DoCommittedCallbacks(IServerMessageDispatcher dispatcher)
		{
			if (IsSystemTransaction())
			{
				return false;
			}
			if (dispatcher != null)
			{
				return dispatcher.Server().CaresAboutCommitted();
			}
			return Callbacks().CaresAboutCommitted();
		}

		private bool DoCommittingCallbacks()
		{
			return !IsSystemTransaction() && Callbacks().CaresAboutCommitting();
		}

		public virtual void Enlist(ITransactionParticipant participant)
		{
			if (null == participant)
			{
				throw new ArgumentNullException();
			}
			CheckSynchronization();
			if (!_participants.ContainsByIdentity(participant))
			{
				_participants.Add(participant);
			}
		}

		private void CommitImpl()
		{
			Commit2Listeners();
			Commit3Stream();
			Commit4FieldIndexes();
			CommitParticipants();
			Container().WriteDirty();
			Slot reservedSlot = AllocateTransactionLogSlot(false);
			FreeSlotChanges(false);
			CommitFreespace();
			FreeSlotChanges(true);
			Commit6WriteChanges(reservedSlot);
		}

		private void FreeSlotChanges(bool forFreespace)
		{
			IVisitor4 visitor = new _IVisitor4_121(this, forFreespace);
			if (IsSystemTransaction())
			{
				_slotChanges.TraverseMutable(visitor);
				return;
			}
			_slotChanges.TraverseLocked(visitor);
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().FreeSlotChanges(forFreespace);
			}
		}

		private sealed class _IVisitor4_121 : IVisitor4
		{
			public _IVisitor4_121(LocalTransaction _enclosing, bool forFreespace)
			{
				this._enclosing = _enclosing;
				this.forFreespace = forFreespace;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).FreeDuringCommit(this._enclosing._file, forFreespace);
			}

			private readonly LocalTransaction _enclosing;

			private readonly bool forFreespace;
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
			Container().ProcessPendingClassUpdates();
			Container().WriteDirty();
			Container().ClassCollection().Write(Container().SystemTransaction());
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
			_slotChanges.Clear();
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
			lock (Container()._lock)
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
			_slotChanges.TraverseLocked(new _IVisitor4_212(this));
		}

		private sealed class _IVisitor4_212 : IVisitor4
		{
			public _IVisitor4_212(LocalTransaction _enclosing)
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

		private Slot AllocateTransactionLogSlot(bool appendToFile)
		{
			int transactionLogByteCount = TransactionLogSlotLength();
			if (FreespaceManager() != null)
			{
				int blockedLength = _file.BytesToBlocks(transactionLogByteCount);
				Slot slot = FreespaceManager().AllocateTransactionLogSlot(blockedLength);
				if (slot != null)
				{
					return _file.ToNonBlockedLength(slot);
				}
			}
			if (!appendToFile)
			{
				return null;
			}
			return _file.AppendBytes(transactionLogByteCount);
		}

		private int TransactionLogSlotLength()
		{
			return ((CountSlotChanges() * 3) + 2) * Const4.INT_LENGTH;
		}

		private bool SlotLongEnoughForLog(Slot slot)
		{
			return slot != null && slot.Length() >= TransactionLogSlotLength();
		}

		protected void Commit6WriteChanges(Slot reservedSlot)
		{
			CheckSynchronization();
			int slotChangeCount = CountSlotChanges();
			if (slotChangeCount > 0)
			{
				Slot transactionLogSlot = SlotLongEnoughForLog(reservedSlot) ? reservedSlot : AllocateTransactionLogSlot
					(true);
				StatefulBuffer buffer = new StatefulBuffer(this, transactionLogSlot);
				buffer.WriteInt(transactionLogSlot.Length());
				buffer.WriteInt(slotChangeCount);
				AppendSlotChanges(buffer);
				buffer.Write();
				FlushFile();
				Container().WriteTransactionPointer(transactionLogSlot.Address());
				FlushFile();
				if (WriteSlots())
				{
					FlushFile();
				}
				Container().WriteTransactionPointer(0);
				FlushFile();
				if (transactionLogSlot != reservedSlot)
				{
					FreeTransactionLogSlot(transactionLogSlot);
				}
			}
			FreeTransactionLogSlot(reservedSlot);
		}

		private void FreeTransactionLogSlot(Slot slot)
		{
			if (slot == null)
			{
				return;
			}
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().FreeTransactionLogSlot(_file.ToBlockedLength(slot));
		}

		public virtual void WriteZeroPointer(int id)
		{
			WritePointer(id, Slot.ZERO);
		}

		public virtual void WritePointer(Pointer4 pointer)
		{
			WritePointer(pointer._id, pointer._slot);
		}

		public virtual void WritePointer(int id, Slot slot)
		{
			CheckSynchronization();
			i_pointerIo.UseSlot(id);
			i_pointerIo.WriteSlot(slot);
			if (Debug.xbytes && Deploy.overwrite)
			{
				i_pointerIo.SetID(Const4.IGNORE_ID);
			}
			i_pointerIo.Write();
		}

		private bool WriteSlots()
		{
			BooleanByRef ret = new BooleanByRef();
			TraverseSlotChanges(new _IVisitor4_325(this, ret));
			return ret.value;
		}

		private sealed class _IVisitor4_325 : IVisitor4
		{
			public _IVisitor4_325(LocalTransaction _enclosing, BooleanByRef ret)
			{
				this._enclosing = _enclosing;
				this.ret = ret;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).WritePointer(this._enclosing);
				ret.value = true;
			}

			private readonly LocalTransaction _enclosing;

			private readonly BooleanByRef ret;
		}

		public virtual void FlushFile()
		{
			if (_file.ConfigImpl().FlushFileBuffers())
			{
				_file.SyncFiles();
			}
		}

		private SlotChange ProduceSlotChange(int id)
		{
			SlotChange slot = new SlotChange(id);
			_slotChanges.Add(slot);
			return (SlotChange)slot.AddedOrExisting();
		}

		private SlotChange FindSlotChange(int a_id)
		{
			CheckSynchronization();
			return (SlotChange)_slotChanges.Find(a_id);
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
			return ReadPointer(id)._slot;
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
			return ReadPointer(id)._slot;
		}

		public virtual Pointer4 ReadPointer(int id)
		{
			_file.ReadBytes(_pointerBuffer, id, Const4.POINTER_LENGTH);
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			return new Pointer4(id, new Slot(address, length));
		}

		private Pointer4 DebugReadPointer(int id)
		{
			return null;
		}

		public override void SetPointer(int a_id, Slot slot)
		{
			CheckSynchronization();
			ProduceSlotChange(a_id).SetPointer(slot);
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
			IntByRef count = new IntByRef();
			TraverseSlotChanges(new _IVisitor4_449(this, count));
			return count.value;
		}

		private sealed class _IVisitor4_449 : IVisitor4
		{
			public _IVisitor4_449(LocalTransaction _enclosing, IntByRef count)
			{
				this._enclosing = _enclosing;
				this.count = count;
			}

			public void Visit(object obj)
			{
				SlotChange slot = (SlotChange)obj;
				if (slot.IsSetPointer())
				{
					count.value++;
				}
			}

			private readonly LocalTransaction _enclosing;

			private readonly IntByRef count;
		}

		internal void WriteOld()
		{
			lock (Container()._lock)
			{
				i_pointerIo.UseSlot(i_address);
				i_pointerIo.Read();
				int length = i_pointerIo.ReadInt();
				if (length > 0)
				{
					StatefulBuffer bytes = new StatefulBuffer(this, i_address, length);
					bytes.Read();
					bytes.IncrementOffset(Const4.INT_LENGTH);
					_slotChanges.Read(bytes, new SlotChange(0));
					if (WriteSlots())
					{
						FlushFile();
					}
					Container().WriteTransactionPointer(0);
					FlushFile();
					FreeSlotChanges(false);
				}
				else
				{
					Container().WriteTransactionPointer(0);
					FlushFile();
				}
			}
		}

		private void AppendSlotChanges(Db4objects.Db4o.Internal.Buffer writer)
		{
			TraverseSlotChanges(new _IVisitor4_484(this, writer));
		}

		private sealed class _IVisitor4_484 : IVisitor4
		{
			public _IVisitor4_484(LocalTransaction _enclosing, Db4objects.Db4o.Internal.Buffer
				 writer)
			{
				this._enclosing = _enclosing;
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).Write(writer);
			}

			private readonly LocalTransaction _enclosing;

			private readonly Db4objects.Db4o.Internal.Buffer writer;
		}

		private void TraverseSlotChanges(IVisitor4 visitor)
		{
			if (_systemTransaction != null)
			{
				ParentLocalTransaction().TraverseSlotChanges(visitor);
			}
			_slotChanges.TraverseLocked(visitor);
		}

		public override void SlotDelete(int id, Slot slot)
		{
			CheckSynchronization();
			if (id == 0)
			{
				return;
			}
			SlotChange slotChange = ProduceSlotChange(id);
			slotChange.FreeOnCommit(_file, slot);
			slotChange.SetPointer(Slot.ZERO);
		}

		public override void SlotFreeOnCommit(int id, Slot slot)
		{
			CheckSynchronization();
			if (id == 0)
			{
				return;
			}
			ProduceSlotChange(id).FreeOnCommit(_file, slot);
		}

		public override void SlotFreeOnRollback(int id, Slot slot)
		{
			CheckSynchronization();
			ProduceSlotChange(id).FreeOnRollback(slot);
		}

		internal override void SlotFreeOnRollbackCommitSetPointer(int id, Slot newSlot, bool
			 forFreespace)
		{
			Slot oldSlot = GetCurrentSlotOfID(id);
			if (oldSlot == null)
			{
				return;
			}
			CheckSynchronization();
			SlotChange change = ProduceSlotChange(id);
			change.FreeOnRollbackSetPointer(newSlot);
			change.FreeOnCommit(_file, oldSlot);
			change.ForFreespace(forFreespace);
		}

		internal override void ProduceUpdateSlotChange(int id, Slot slot)
		{
			CheckSynchronization();
			SlotChange slotChange = ProduceSlotChange(id);
			slotChange.FreeOnRollbackSetPointer(slot);
		}

		public override void SlotFreePointerOnCommit(int a_id)
		{
			CheckSynchronization();
			Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			SlotFreeOnCommit(a_id, slot);
		}

		internal override void SlotFreePointerOnCommit(int a_id, Slot slot)
		{
			CheckSynchronization();
			SlotFreeOnCommit(slot.Address(), slot);
			SlotFreeOnCommit(a_id, slot);
		}

		public override void SlotFreePointerOnRollback(int id)
		{
			ProduceSlotChange(id).FreePointerOnRollback();
		}

		public override void ProcessDeletes()
		{
			if (_delete == null)
			{
				_writtenUpdateDeletedMembers = null;
				return;
			}
			while (_delete != null)
			{
				Tree delete = _delete;
				_delete = null;
				delete.Traverse(new _IVisitor4_608(this));
			}
			_writtenUpdateDeletedMembers = null;
		}

		private sealed class _IVisitor4_608 : IVisitor4
		{
			public _IVisitor4_608(LocalTransaction _enclosing)
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
				if (obj == null || info._reference.GetID() < 0)
				{
					HardObjectReference hardRef = this._enclosing.Container().GetHardObjectReferenceById
						(this._enclosing, info._key);
					if (hardRef == HardObjectReference.INVALID)
					{
						return;
					}
					info._reference = hardRef._reference;
					info._reference.FlagForDelete(this._enclosing.Container().TopLevelCallId());
					obj = info._reference.GetObject();
				}
				this._enclosing.Container().Delete3(this._enclosing, info._reference, info._cascade
					, false);
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
			StatefulBuffer objectBytes = Container().ReadWriterByID(this, id);
			if (objectBytes == null)
			{
				if (clazz.HasClassIndex())
				{
					DontRemoveFromClassIndex(clazz.GetID(), id);
				}
				return;
			}
			ObjectHeader oh = new ObjectHeader(Container(), clazz, objectBytes);
			DeleteInfo info = (DeleteInfo)TreeInt.Find(_delete, id);
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
			SlotFreeOnCommit(id, new Slot(objectBytes.GetAddress(), objectBytes.Length()));
		}

		private ICallbacks Callbacks()
		{
			return Container().Callbacks();
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
			_slotChanges.TraverseLocked(new _IVisitor4_699(this, deleted, added, updated));
			return new CallbackObjectInfoCollections(serverMessageDispatcher, new ObjectInfoCollectionImpl
				(added), new ObjectInfoCollectionImpl(updated), new ObjectInfoCollectionImpl(deleted
				));
		}

		private sealed class _IVisitor4_699 : IVisitor4
		{
			public _IVisitor4_699(LocalTransaction _enclosing, Collection4 deleted, Collection4
				 added, Collection4 updated)
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
					)file.NewTransaction(null, null);
				transaction.SetAddress(transactionID1);
				return transaction;
			}
			return null;
		}

		private IFreespaceManager FreespaceManager()
		{
			return _file.FreespaceManager();
		}

		private void FreespaceBeginCommit()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().BeginCommit();
		}

		private void FreespaceEndCommit()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().EndCommit();
		}

		private void CommitFreespace()
		{
			if (FreespaceManager() == null)
			{
				return;
			}
			FreespaceManager().Commit();
		}
	}
}
