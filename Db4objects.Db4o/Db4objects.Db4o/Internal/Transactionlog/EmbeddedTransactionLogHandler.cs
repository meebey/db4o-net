/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal.Transactionlog
{
	/// <exclude></exclude>
	public class EmbeddedTransactionLogHandler : TransactionLogHandler
	{
		public EmbeddedTransactionLogHandler(StandardIdSystem idSystem) : base(idSystem)
		{
		}

		public override IInterruptedTransactionHandler InterruptedTransactionHandler(ByteArrayBuffer
			 reader)
		{
			int transactionID1 = reader.ReadInt();
			int transactionID2 = reader.ReadInt();
			if ((transactionID1 > 0) && (transactionID1 == transactionID2))
			{
				return new _IInterruptedTransactionHandler_23(this, transactionID1);
			}
			return null;
		}

		private sealed class _IInterruptedTransactionHandler_23 : IInterruptedTransactionHandler
		{
			public _IInterruptedTransactionHandler_23(EmbeddedTransactionLogHandler _enclosing
				, int transactionID1)
			{
				this._enclosing = _enclosing;
				this.transactionID1 = transactionID1;
				this._addressOfIncompleteCommit = transactionID1;
			}

			private int _addressOfIncompleteCommit;

			public void CompleteInterruptedTransaction()
			{
				StatefulBuffer bytes = new StatefulBuffer(this._enclosing._idSystem.SystemTransaction
					(), this._addressOfIncompleteCommit, Const4.IntLength);
				bytes.Read();
				int length = bytes.ReadInt();
				if (length > 0)
				{
					bytes = new StatefulBuffer(this._enclosing._idSystem.SystemTransaction(), this._addressOfIncompleteCommit
						, length);
					bytes.Read();
					bytes.IncrementOffset(Const4.IntLength);
					this._enclosing._idSystem.ReadWriteSlotChanges(bytes);
					this._enclosing.LocalContainer().WriteTransactionPointer(0);
					this._enclosing.FlushDatabaseFile();
					this._enclosing._idSystem.FreeAndClearSystemSlotChanges();
				}
				else
				{
					this._enclosing.LocalContainer().WriteTransactionPointer(0);
					this._enclosing.FlushDatabaseFile();
				}
			}

			private readonly EmbeddedTransactionLogHandler _enclosing;

			private readonly int transactionID1;
		}

		public override Slot AllocateSlot(LocalTransaction transaction, bool appendToFile
			)
		{
			int transactionLogByteCount = TransactionLogSlotLength(transaction);
			IFreespaceManager freespaceManager = transaction.FreespaceManager();
			if (!appendToFile && freespaceManager != null)
			{
				int blockedLength = transaction.LocalContainer().BytesToBlocks(transactionLogByteCount
					);
				Slot slot = freespaceManager.AllocateTransactionLogSlot(blockedLength);
				if (slot != null)
				{
					return transaction.LocalContainer().ToNonBlockedLength(slot);
				}
			}
			return transaction.LocalContainer().AppendBytes(transactionLogByteCount);
		}

		private void FreeSlot(Slot slot)
		{
			if (slot == null)
			{
				return;
			}
			if (_idSystem.FreespaceManager() == null)
			{
				return;
			}
			_idSystem.FreespaceManager().FreeTransactionLogSlot(LocalContainer().ToBlockedLength
				(slot));
		}

		public override void ApplySlotChanges(LocalTransaction transaction, Slot reservedSlot
			)
		{
			int slotChangeCount = CountSlotChanges(transaction);
			if (slotChangeCount > 0)
			{
				Slot transactionLogSlot = SlotLongEnoughForLog(transaction, reservedSlot) ? reservedSlot
					 : AllocateSlot(transaction, true);
				StatefulBuffer buffer = new StatefulBuffer(transaction.SystemTransaction(), transactionLogSlot
					);
				buffer.WriteInt(transactionLogSlot.Length());
				buffer.WriteInt(slotChangeCount);
				AppendSlotChanges(transaction, buffer);
				buffer.Write();
				FlushDatabaseFile();
				LocalContainer().WriteTransactionPointer(transactionLogSlot.Address());
				FlushDatabaseFile();
				if (_idSystem.WriteSlots(transaction))
				{
					FlushDatabaseFile();
				}
				LocalContainer().WriteTransactionPointer(0);
				FlushDatabaseFile();
				if (transactionLogSlot != reservedSlot)
				{
					FreeSlot(transactionLogSlot);
				}
			}
			FreeSlot(reservedSlot);
		}

		private bool SlotLongEnoughForLog(LocalTransaction transaction, Slot slot)
		{
			return slot != null && slot.Length() >= TransactionLogSlotLength(transaction);
		}

		public override void Close()
		{
		}
		// do nothing
	}
}
