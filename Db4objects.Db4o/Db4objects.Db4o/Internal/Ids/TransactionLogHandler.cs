/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public abstract class TransactionLogHandler
	{
		protected virtual LocalObjectContainer File(IdSystem idSystem)
		{
			return idSystem.File();
		}

		protected virtual void FlushDatabaseFile(IdSystem idSystem)
		{
			idSystem.FlushFile();
		}

		protected virtual void AppendSlotChanges(IdSystem idSystem, ByteArrayBuffer writer
			)
		{
			idSystem.TraverseSlotChanges(new _IVisitor4_24(writer));
		}

		private sealed class _IVisitor4_24 : IVisitor4
		{
			public _IVisitor4_24(ByteArrayBuffer writer)
			{
				this.writer = writer;
			}

			public void Visit(object obj)
			{
				((SlotChange)obj).Write(writer);
			}

			private readonly ByteArrayBuffer writer;
		}

		protected virtual int TransactionLogSlotLength(IdSystem idSystem)
		{
			// slotchanges * 3 for ID, address, length
			// 2 ints for slotlength and count
			return ((CountSlotChanges(idSystem) * 3) + 2) * Const4.IntLength;
		}

		protected virtual int CountSlotChanges(IdSystem idSystem)
		{
			IntByRef count = new IntByRef();
			idSystem.TraverseSlotChanges(new _IVisitor4_39(count));
			return count.value;
		}

		private sealed class _IVisitor4_39 : IVisitor4
		{
			public _IVisitor4_39(IntByRef count)
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

		public abstract Slot AllocateSlot(IdSystem idSystem, bool append);

		public abstract void ApplySlotChanges(IdSystem idSystem, Slot reservedSlot);

		public abstract bool CheckForInterruptedTransaction(IdSystem idSystem, ByteArrayBuffer
			 reader);

		public abstract void CompleteInterruptedTransaction(IdSystem idSystem);

		public abstract void Close();
	}
}
