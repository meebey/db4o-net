/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Lang;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class InMemoryIdSystem : IIdSystem
	{
		private readonly LocalObjectContainer _container;

		private IdSlotTree _ids;

		private Slot _slot;

		private readonly SequentialIdGenerator _idGenerator;

		/// <summary>for testing purposes only.</summary>
		/// <remarks>for testing purposes only.</remarks>
		public InMemoryIdSystem(LocalObjectContainer container, int maxValidId)
		{
			_container = container;
			_idGenerator = new SequentialIdGenerator(new _IFunction4_30(this, maxValidId), _container
				.Handlers.LowestValidId(), maxValidId);
		}

		private sealed class _IFunction4_30 : IFunction4
		{
			public _IFunction4_30(InMemoryIdSystem _enclosing, int maxValidId)
			{
				this._enclosing = _enclosing;
				this.maxValidId = maxValidId;
			}

			public object Apply(object start)
			{
				return this._enclosing.FindFreeId((((int)start)), maxValidId);
			}

			private readonly InMemoryIdSystem _enclosing;

			private readonly int maxValidId;
		}

		public InMemoryIdSystem(LocalObjectContainer container) : this(container, int.MaxValue
			)
		{
			ReadThis();
		}

		private void ReadThis()
		{
			SystemData systemData = _container.SystemData();
			_slot = new Slot(systemData.TransactionPointer1(), systemData.TransactionPointer2
				());
			if (!_slot.IsNull())
			{
				ByteArrayBuffer buffer = _container.ReadBufferBySlot(_slot);
				_idGenerator.Read(buffer);
				_ids = (IdSlotTree)new TreeReader(buffer, new IdSlotTree(0, null)).Read();
			}
		}

		public virtual void Close()
		{
		}

		// do nothing
		public virtual void Commit(IVisitable slotChanges, FreespaceCommitter freespaceCommitter
			)
		{
			Slot oldSlot = _slot;
			Slot reservedSlot = AllocateSlot(false, EstimatedSlotLength(EstimateMappingCount(
				slotChanges)));
			// No more operations against the FreespaceManager.
			// Time to free old slots.
			freespaceCommitter.Commit();
			slotChanges.Accept(new _IVisitor4_66(this));
			WriteThis(reservedSlot);
			FreeSlot(oldSlot);
		}

		private sealed class _IVisitor4_66 : IVisitor4
		{
			public _IVisitor4_66(InMemoryIdSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object slotChange)
			{
				if (!((SlotChange)slotChange).SlotModified())
				{
					return;
				}
				if (((SlotChange)slotChange).RemoveId())
				{
					this._enclosing._ids = (IdSlotTree)Tree.RemoveLike(this._enclosing._ids, new TreeInt
						(((TreeInt)slotChange)._key));
					return;
				}
				if (DTrace.enabled)
				{
					DTrace.SlotCommitted.LogLength(((TreeInt)slotChange)._key, ((SlotChange)slotChange
						).NewSlot());
				}
				this._enclosing._ids = ((IdSlotTree)Tree.Add(this._enclosing._ids, new IdSlotTree
					(((TreeInt)slotChange)._key, ((SlotChange)slotChange).NewSlot())));
			}

			private readonly InMemoryIdSystem _enclosing;
		}

		private Slot AllocateSlot(bool appendToFile, int slotLength)
		{
			if (!appendToFile)
			{
				Slot slot = _container.FreespaceManager().AllocateTransactionLogSlot(slotLength);
				if (slot != null)
				{
					return slot;
				}
			}
			return _container.AppendBytes(slotLength);
		}

		private int EstimateMappingCount(IVisitable slotChanges)
		{
			IntByRef count = new IntByRef();
			count.value = _ids == null ? 0 : _ids.Size();
			slotChanges.Accept(new _IVisitor4_100(count));
			return count.value;
		}

		private sealed class _IVisitor4_100 : IVisitor4
		{
			public _IVisitor4_100(IntByRef count)
			{
				this.count = count;
			}

			public void Visit(object slotChange)
			{
				if (!((SlotChange)slotChange).SlotModified() || ((SlotChange)slotChange).RemoveId
					())
				{
					return;
				}
				count.value++;
			}

			private readonly IntByRef count;
		}

		private void WriteThis(Slot reservedSlot)
		{
			// We need a little dance here to keep filling free slots
			// with X bytes. The FreespaceManager would do it immediately
			// upon the free call, but then our CrashSimulatingTestCase
			// fails because we have the Xses in the file before flushing.
			Slot xByteSlot = null;
			int slotLength = SlotLength();
			if (reservedSlot.Length() >= slotLength)
			{
				_slot = reservedSlot;
				reservedSlot = null;
			}
			else
			{
				_slot = AllocateSlot(true, slotLength);
			}
			ByteArrayBuffer buffer = new ByteArrayBuffer(_slot.Length());
			_idGenerator.Write(buffer);
			TreeInt.Write(buffer, _ids);
			_container.WriteBytes(buffer, _slot.Address(), 0);
			IRunnable commitHook = _container.CommitHook();
			_container.SyncFiles();
			_container.WriteTransactionPointer(_slot.Address(), _slot.Length());
			commitHook.Run();
			_container.SyncFiles();
			_container.SystemData().TransactionPointer1(_slot.Address());
			_container.SystemData().TransactionPointer2(_slot.Length());
			FreeSlot(reservedSlot);
		}

		private void FreeSlot(Slot slot)
		{
			if (slot == null || slot.IsNull())
			{
				return;
			}
			IFreespaceManager freespaceManager = _container.FreespaceManager();
			if (freespaceManager == null)
			{
				return;
			}
			freespaceManager.FreeTransactionLogSlot(slot);
		}

		private int SlotLength()
		{
			return TreeInt.MarshalledLength(_ids) + _idGenerator.MarshalledLength();
		}

		private int EstimatedSlotLength(int estimatedCount)
		{
			IdSlotTree template = _ids;
			if (template == null)
			{
				template = new IdSlotTree(0, new Slot(0, 0));
			}
			return template.MarshalledLength(estimatedCount) + _idGenerator.MarshalledLength(
				);
		}

		public virtual Slot CommittedSlot(int id)
		{
			IdSlotTree idSlotMapping = (IdSlotTree)Tree.Find(_ids, new TreeInt(id));
			if (idSlotMapping == null)
			{
				throw new InvalidIDException(id);
			}
			return idSlotMapping.Slot();
		}

		public virtual void CompleteInterruptedTransaction(int address, int length)
		{
		}

		// do nothing
		public virtual int NewId()
		{
			int id = _idGenerator.NewId();
			_ids = ((IdSlotTree)Tree.Add(_ids, new IdSlotTree(id, Slot.Zero)));
			return id;
		}

		private int FindFreeId(int start, int end)
		{
			if (_ids == null)
			{
				return start;
			}
			IntByRef lastId = new IntByRef();
			IntByRef freeId = new IntByRef();
			Tree.Traverse(_ids, new TreeInt(start), new _ICancellableVisitor4_203(lastId, start
				, freeId));
			if (freeId.value > 0)
			{
				return freeId.value;
			}
			if (lastId.value < end)
			{
				return Math.Max(start, lastId.value + 1);
			}
			return 0;
		}

		private sealed class _ICancellableVisitor4_203 : ICancellableVisitor4
		{
			public _ICancellableVisitor4_203(IntByRef lastId, int start, IntByRef freeId)
			{
				this.lastId = lastId;
				this.start = start;
				this.freeId = freeId;
			}

			public bool Visit(object node)
			{
				int id = ((TreeInt)node)._key;
				if (lastId.value == 0)
				{
					if (id > start)
					{
						freeId.value = start;
						return false;
					}
					lastId.value = id;
					return true;
				}
				if (id > lastId.value + 1)
				{
					freeId.value = lastId.value + 1;
					return false;
				}
				lastId.value = id;
				return true;
			}

			private readonly IntByRef lastId;

			private readonly int start;

			private readonly IntByRef freeId;
		}

		public virtual void ReturnUnusedIds(IVisitable visitable)
		{
			visitable.Accept(new _IVisitor4_232(this));
		}

		private sealed class _IVisitor4_232 : IVisitor4
		{
			public _IVisitor4_232(InMemoryIdSystem _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object obj)
			{
				this._enclosing._ids = (IdSlotTree)Tree.RemoveLike(this._enclosing._ids, new TreeInt
					((((int)obj))));
			}

			private readonly InMemoryIdSystem _enclosing;
		}

		public virtual ITransactionalIdSystem FreespaceIdSystem()
		{
			return null;
		}
	}
}
