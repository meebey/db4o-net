/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal.Transactionlog
{
	/// <exclude></exclude>
	public abstract class TransactionLogHandler
	{
		protected readonly StandardIdSystem _idSystem;

		protected TransactionLogHandler(StandardIdSystem idSystem)
		{
			_idSystem = idSystem;
		}

		protected virtual LocalObjectContainer LocalContainer()
		{
			return _idSystem.LocalContainer();
		}

		protected void FlushDatabaseFile()
		{
			_idSystem.FlushFile();
		}

		protected void AppendSlotChanges(LocalTransaction transaction, ByteArrayBuffer writer
			)
		{
			_idSystem.TraverseSlotChanges(transaction, new _IVisitor4_31(writer));
		}

		private sealed class _IVisitor4_31 : IVisitor4
		{
			public _IVisitor4_31(ByteArrayBuffer writer)
			{
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).Write(writer);
			}

			private readonly ByteArrayBuffer writer;
		}

		protected int TransactionLogSlotLength(LocalTransaction transaction)
		{
			// slotchanges * 3 for ID, address, length
			// 2 ints for slotlength and count
			return ((CountSlotChanges(transaction) * 3) + 2) * Const4.IntLength;
		}

		protected int CountSlotChanges(LocalTransaction transaction)
		{
			IntByRef count = new IntByRef();
			_idSystem.TraverseSlotChanges(transaction, new _IVisitor4_46(count));
			return count.value;
		}

		private sealed class _IVisitor4_46 : IVisitor4
		{
			public _IVisitor4_46(IntByRef count)
			{
				this.count = count;
			}

			public void Visit(object obj)
			{
				SlotChange slot = (SlotChange)obj;
				if (slot.SlotModified())
				{
					count.value++;
				}
			}

			private readonly IntByRef count;
		}

		public abstract Slot AllocateSlot(LocalTransaction transaction, bool append);

		public abstract void ApplySlotChanges(LocalTransaction transaction, Slot reservedSlot
			);

		public abstract IInterruptedTransactionHandler InterruptedTransactionHandler(ByteArrayBuffer
			 reader);

		public abstract void Close();
	}
}
