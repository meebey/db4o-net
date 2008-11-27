/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Transactionlog
{
	/// <exclude></exclude>
	public abstract class TransactionLogHandler
	{
		protected virtual LocalObjectContainer File(LocalTransaction trans)
		{
			return trans.File();
		}

		protected virtual void FlushDatabaseFile(LocalTransaction trans)
		{
			trans.FlushFile();
		}

		protected virtual void AppendSlotChanges(LocalTransaction trans, ByteArrayBuffer 
			writer)
		{
			trans.TraverseSlotChanges(new _IVisitor4_25(writer));
		}

		private sealed class _IVisitor4_25 : IVisitor4
		{
			public _IVisitor4_25(ByteArrayBuffer writer)
			{
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).Write(writer);
			}

			private readonly ByteArrayBuffer writer;
		}

		protected virtual int TransactionLogSlotLength(LocalTransaction trans)
		{
			// slotchanges * 3 for ID, address, length
			// 2 ints for slotlength and count
			return ((CountSlotChanges(trans) * 3) + 2) * Const4.IntLength;
		}

		protected virtual int CountSlotChanges(LocalTransaction trans)
		{
			IntByRef count = new IntByRef();
			trans.TraverseSlotChanges(new _IVisitor4_40(count));
			return count.value;
		}

		private sealed class _IVisitor4_40 : IVisitor4
		{
			public _IVisitor4_40(IntByRef count)
			{
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

			private readonly IntByRef count;
		}

		public abstract Slot AllocateSlot(LocalTransaction trans, bool append);

		public abstract void ApplySlotChanges(LocalTransaction trans, Slot reservedSlot);

		public abstract bool CheckForInterruptedTransaction(LocalTransaction trans, ByteArrayBuffer
			 reader);

		public abstract void CompleteInterruptedTransaction(LocalTransaction trans);

		public abstract void Close();
	}
}
