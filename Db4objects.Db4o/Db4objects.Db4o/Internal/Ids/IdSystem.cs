/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Ids
{
	public class IdSystem
	{
		private readonly byte[] _pointerBuffer = new byte[Const4.PointerLength];

		protected readonly StatefulBuffer i_pointerIo;

		private readonly LockedTree _slotChanges = new LockedTree();

		protected readonly LocalObjectContainer _file;

		private Db4objects.Db4o.Internal.Ids.IdSystem _systemIdSystem;

		private TransactionLogHandler _transactionLogHandler = new EmbeddedTransactionLogHandler
			();

		public IdSystem(ObjectContainerBase container)
		{
			_file = (LocalObjectContainer)container;
			i_pointerIo = new StatefulBuffer(_file.Transaction, Const4.PointerLength);
			InitializeTransactionLogHandler();
		}

		private void InitializeTransactionLogHandler()
		{
			if (!IsSystemIdSystem())
			{
				_transactionLogHandler = _systemIdSystem._transactionLogHandler;
				return;
			}
			bool fileBased = Config().FileBasedTransactionLog() && File() is IoAdaptedObjectContainer;
			if (!fileBased)
			{
				_transactionLogHandler = new EmbeddedTransactionLogHandler();
				return;
			}
			string fileName = ((IoAdaptedObjectContainer)File()).FileName();
			_transactionLogHandler = new FileBasedTransactionLogHandler(this, fileName);
		}

		private bool IsSystemIdSystem()
		{
			return _systemIdSystem == null;
		}

		public virtual Config4Impl Config()
		{
			return File().Config();
		}

		public virtual LocalObjectContainer File()
		{
			return _file;
		}

		public virtual void Commit()
		{
			lock (File().Lock())
			{
				CommitImpl();
				CommitClear();
			}
		}

		private void CommitImpl()
		{
			Slot reservedSlot = _transactionLogHandler.AllocateSlot(this, false);
			FreeSlotChanges(false);
			FreespaceBeginCommit();
			CommitFreespace();
			FreeSlotChanges(true);
			_transactionLogHandler.ApplySlotChanges(this, reservedSlot);
			FreespaceEndCommit();
		}

		public void FreeSlotChanges(bool forFreespace)
		{
			IVisitor4 visitor = new _IVisitor4_82(this, forFreespace);
			if (IsSystemIdSystem())
			{
				_slotChanges.TraverseMutable(visitor);
				return;
			}
			_slotChanges.TraverseLocked(visitor);
			if (_systemIdSystem != null)
			{
				_systemIdSystem.FreeSlotChanges(forFreespace);
			}
		}

		private sealed class _IVisitor4_82 : IVisitor4
		{
			public _IVisitor4_82(IdSystem _enclosing, bool forFreespace)
			{
				this._enclosing = _enclosing;
				this.forFreespace = forFreespace;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).FreeDuringCommit(this._enclosing._file, forFreespace);
			}

			private readonly IdSystem _enclosing;

			private readonly bool forFreespace;
		}

		private void CommitClear()
		{
			if (_systemIdSystem != null)
			{
				_systemIdSystem.CommitClear();
			}
			Clear();
		}

		protected virtual void Clear()
		{
			_slotChanges.Clear();
		}

		public virtual void Rollback()
		{
			lock (File().Lock())
			{
				RollbackSlotChanges();
				Clear();
			}
		}

		protected virtual void RollbackSlotChanges()
		{
			_slotChanges.TraverseLocked(new _IVisitor4_119(this));
		}

		private sealed class _IVisitor4_119 : IVisitor4
		{
			public _IVisitor4_119(IdSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				((SlotChange)a_object).Rollback(this._enclosing._file);
			}

			private readonly IdSystem _enclosing;
		}

		public virtual bool IsDeleted(int id)
		{
			return SlotChangeIsFlaggedDeleted(id);
		}

		public virtual void WriteZeroPointer(int id)
		{
			WritePointer(id, Slot.Zero);
		}

		public virtual void WritePointer(Pointer4 pointer)
		{
			WritePointer(pointer._id, pointer._slot);
		}

		public virtual void WritePointer(int id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.WritePointer.Log(id);
				DTrace.WritePointer.LogLength(slot);
			}
			i_pointerIo.UseSlot(id);
			i_pointerIo.WriteInt(slot.Address());
			i_pointerIo.WriteInt(slot.Length());
			if (Debug4.xbytes && Deploy.overwrite)
			{
				i_pointerIo.SetID(Const4.IgnoreId);
			}
			i_pointerIo.Write();
		}

		public virtual bool WriteSlots()
		{
			BooleanByRef ret = new BooleanByRef();
			TraverseSlotChanges(new _IVisitor4_160(this, ret));
			return ret.value;
		}

		private sealed class _IVisitor4_160 : IVisitor4
		{
			public _IVisitor4_160(IdSystem _enclosing, BooleanByRef ret)
			{
				this._enclosing = _enclosing;
				this.ret = ret;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).WritePointer(this._enclosing.SystemTransaction());
				ret.value = true;
			}

			private readonly IdSystem _enclosing;

			private readonly BooleanByRef ret;
		}

		public virtual void FlushFile()
		{
			if (DTrace.enabled)
			{
				DTrace.TransFlush.Log();
			}
			_file.SyncFiles();
		}

		private SlotChange ProduceSlotChange(int id)
		{
			if (DTrace.enabled)
			{
				DTrace.ProduceSlotChange.Log(id);
			}
			SlotChange slot = new SlotChange(id);
			_slotChanges.Add(slot);
			return (SlotChange)slot.AddedOrExisting();
		}

		public SlotChange FindSlotChange(int a_id)
		{
			return (SlotChange)_slotChanges.Find(a_id);
		}

		public virtual Slot GetCurrentSlotOfID(int id)
		{
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
			if (_systemIdSystem != null)
			{
				Slot parentSlot = _systemIdSystem.GetCurrentSlotOfID(id);
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
			if (_systemIdSystem != null)
			{
				Slot parentSlot = _systemIdSystem.GetCommittedSlotOfID(id);
				if (parentSlot != null)
				{
					return parentSlot;
				}
			}
			return ReadPointer(id)._slot;
		}

		public virtual Pointer4 ReadPointer(int id)
		{
			if (!IsValidId(id))
			{
				throw new InvalidIDException(id);
			}
			_file.ReadBytes(_pointerBuffer, id, Const4.PointerLength);
			int address = (_pointerBuffer[3] & 255) | (_pointerBuffer[2] & 255) << 8 | (_pointerBuffer
				[1] & 255) << 16 | _pointerBuffer[0] << 24;
			int length = (_pointerBuffer[7] & 255) | (_pointerBuffer[6] & 255) << 8 | (_pointerBuffer
				[5] & 255) << 16 | _pointerBuffer[4] << 24;
			if (!IsValidSlot(address, length))
			{
				throw new InvalidSlotException(address, length, id);
			}
			return new Pointer4(id, new Slot(address, length));
		}

		private bool IsValidId(int id)
		{
			return _file.FileLength() >= id;
		}

		private bool IsValidSlot(int address, int length)
		{
			// just in case overflow 
			long fileLength = _file.FileLength();
			bool validAddress = fileLength >= address;
			bool validLength = fileLength >= length;
			bool validSlot = fileLength >= (address + length);
			return validAddress && validLength && validSlot;
		}

		private Pointer4 DebugReadPointer(int id)
		{
			return null;
		}

		public virtual void SetPointer(int a_id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.SlotSetPointer.Log(a_id);
				DTrace.SlotSetPointer.LogLength(slot);
			}
			ProduceSlotChange(a_id).SetPointer(slot);
		}

		private bool SlotChangeIsFlaggedDeleted(int id)
		{
			SlotChange slot = FindSlotChange(id);
			if (slot != null)
			{
				return slot.IsDeleted();
			}
			if (_systemIdSystem != null)
			{
				return _systemIdSystem.SlotChangeIsFlaggedDeleted(id);
			}
			return false;
		}

		internal void CompleteInterruptedTransaction()
		{
			lock (File().Lock())
			{
				_transactionLogHandler.CompleteInterruptedTransaction(this);
			}
		}

		public virtual void TraverseSlotChanges(IVisitor4 visitor)
		{
			if (_systemIdSystem != null)
			{
				_systemIdSystem.TraverseSlotChanges(visitor);
			}
			_slotChanges.TraverseLocked(visitor);
		}

		public virtual void SlotDelete(int id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.SlotDelete.Log(id);
				DTrace.SlotDelete.LogLength(slot);
			}
			if (id == 0)
			{
				return;
			}
			SlotChange slotChange = ProduceSlotChange(id);
			slotChange.FreeOnCommit(_file, slot);
			slotChange.SetPointer(Slot.Zero);
		}

		public virtual void SlotFreeOnCommit(int id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.SlotFreeOnCommit.Log(id);
				DTrace.SlotFreeOnCommit.LogLength(slot);
			}
			if (id == 0)
			{
				return;
			}
			ProduceSlotChange(id).FreeOnCommit(_file, slot);
		}

		public virtual void SlotFreeOnRollback(int id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.SlotFreeOnRollbackId.Log(id);
				DTrace.SlotFreeOnRollbackAddress.LogLength(slot);
			}
			ProduceSlotChange(id).FreeOnRollback(slot);
		}

		internal virtual void SlotFreeOnRollbackCommitSetPointer(int id, Slot newSlot, bool
			 forFreespace)
		{
			Slot oldSlot = GetCurrentSlotOfID(id);
			if (oldSlot == null)
			{
				return;
			}
			if (DTrace.enabled)
			{
				DTrace.FreeOnRollback.Log(id);
				DTrace.FreeOnRollback.LogLength(newSlot);
				DTrace.FreeOnCommit.Log(id);
				DTrace.FreeOnCommit.LogLength(oldSlot);
			}
			SlotChange change = ProduceSlotChange(id);
			change.FreeOnRollbackSetPointer(newSlot);
			change.FreeOnCommit(_file, oldSlot);
			change.ForFreespace(forFreespace);
		}

		internal virtual void ProduceUpdateSlotChange(int id, Slot slot)
		{
			if (DTrace.enabled)
			{
				DTrace.FreeOnRollback.Log(id);
				DTrace.FreeOnRollback.LogLength(slot);
			}
			SlotChange slotChange = ProduceSlotChange(id);
			slotChange.FreeOnRollbackSetPointer(slot);
		}

		public virtual void SlotFreePointerOnCommit(int a_id)
		{
			Slot slot = GetCurrentSlotOfID(a_id);
			if (slot == null)
			{
				return;
			}
			// FIXME: From looking at this it should call slotFreePointerOnCommit
			//        Write a test case and check.
			//        Looking at references, this method is only called from freed
			//        BTree nodes. Indeed it should be checked what happens here.
			SlotFreeOnCommit(a_id, slot);
		}

		internal virtual void SlotFreePointerOnCommit(int a_id, Slot slot)
		{
			SlotFreeOnCommit(slot.Address(), slot);
			// FIXME: This does not look nice
			SlotFreeOnCommit(a_id, slot);
		}

		// FIXME: It should rather work like this:
		// produceSlotChange(a_id).freePointerOnCommit();
		public virtual void SlotFreePointerOnRollback(int id)
		{
			ProduceSlotChange(id).FreePointerOnRollback();
		}

		public virtual CallbackObjectInfoCollections CollectCommittingCallbackInfo(LocalTransaction
			 trans)
		{
			if (null == _slotChanges)
			{
				return CallbackObjectInfoCollections.Emtpy;
			}
			Collection4 added = new Collection4();
			Collection4 deleted = new Collection4();
			Collection4 updated = new Collection4();
			CollectSlotChanges(new _ISlotChangeCollector_412(added, trans, updated, deleted));
			return NewCallbackObjectInfoCollections(added, updated, deleted);
		}

		private sealed class _ISlotChangeCollector_412 : ISlotChangeCollector
		{
			public _ISlotChangeCollector_412(Collection4 added, LocalTransaction trans, Collection4
				 updated, Collection4 deleted)
			{
				this.added = added;
				this.trans = trans;
				this.updated = updated;
				this.deleted = deleted;
			}

			public void Added(int id)
			{
				added.Add(trans.LazyReferenceFor(id));
			}

			public void Updated(int id)
			{
				updated.Add(trans.LazyReferenceFor(id));
			}

			public void Deleted(int id)
			{
				IObjectInfo @ref = trans.FrozenReferenceFor(id);
				if (@ref != null)
				{
					deleted.Add(@ref);
				}
			}

			private readonly Collection4 added;

			private readonly LocalTransaction trans;

			private readonly Collection4 updated;

			private readonly Collection4 deleted;
		}

		private CallbackObjectInfoCollections NewCallbackObjectInfoCollections(Collection4
			 added, Collection4 updated, Collection4 deleted)
		{
			return new CallbackObjectInfoCollections(new ObjectInfoCollectionImpl(added), new 
				ObjectInfoCollectionImpl(updated), new ObjectInfoCollectionImpl(deleted));
		}

		private void CollectSlotChanges(ISlotChangeCollector collector)
		{
			if (null == _slotChanges)
			{
				return;
			}
			_slotChanges.TraverseLocked(new _IVisitor4_445(collector));
		}

		private sealed class _IVisitor4_445 : IVisitor4
		{
			public _IVisitor4_445(ISlotChangeCollector collector)
			{
				this.collector = collector;
			}

			public void Visit(object obj)
			{
				SlotChange slotChange = ((SlotChange)obj);
				int id = slotChange._key;
				if (slotChange.IsDeleted())
				{
					if (!slotChange.IsNew())
					{
						collector.Deleted(id);
					}
				}
				else
				{
					if (slotChange.IsNew())
					{
						collector.Added(id);
					}
					else
					{
						collector.Updated(id);
					}
				}
			}

			private readonly ISlotChangeCollector collector;
		}

		public static Transaction ReadInterruptedTransaction(LocalObjectContainer file, ByteArrayBuffer
			 reader)
		{
			LocalTransaction transaction = (LocalTransaction)file.NewTransaction(null, null);
			if (transaction.WasInterrupted(reader))
			{
				return transaction;
			}
			return null;
		}

		public virtual bool WasInterrupted(ByteArrayBuffer reader)
		{
			return _transactionLogHandler.CheckForInterruptedTransaction(this, reader);
		}

		public virtual IFreespaceManager FreespaceManager()
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

		public virtual void ReadSlotChanges(ByteArrayBuffer buffer)
		{
			_slotChanges.Read(buffer, new SlotChange(0));
		}

		public virtual void Close()
		{
			_transactionLogHandler.Close();
		}

		public virtual LocalTransaction SystemTransaction()
		{
			return (LocalTransaction)File().SystemTransaction();
		}
	}
}
