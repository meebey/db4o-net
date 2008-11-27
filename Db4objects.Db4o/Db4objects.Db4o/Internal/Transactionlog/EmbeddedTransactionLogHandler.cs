/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Internal.Transactionlog;

namespace Db4objects.Db4o.Internal.Transactionlog
{
	/// <exclude></exclude>
	public class EmbeddedTransactionLogHandler : TransactionLogHandler
	{
		private int _addressOfIncompleteCommit;

		public override bool CheckForInterruptedTransaction(LocalTransaction trans, ByteArrayBuffer
			 reader)
		{
			int transactionID1 = reader.ReadInt();
			int transactionID2 = reader.ReadInt();
			if ((transactionID1 > 0) && (transactionID1 == transactionID2))
			{
				_addressOfIncompleteCommit = transactionID1;
				return true;
			}
			return false;
		}

		public override void CompleteInterruptedTransaction(LocalTransaction trans)
		{
			StatefulBuffer bytes = new StatefulBuffer(trans, _addressOfIncompleteCommit, Const4
				.IntLength);
			bytes.Read();
			int length = bytes.ReadInt();
			if (length > 0)
			{
				bytes = new StatefulBuffer(trans, _addressOfIncompleteCommit, length);
				bytes.Read();
				bytes.IncrementOffset(Const4.IntLength);
				trans.ReadSlotChanges(bytes);
				if (trans.WriteSlots())
				{
					FlushDatabaseFile(trans);
				}
				File(trans).WriteTransactionPointer(0);
				FlushDatabaseFile(trans);
				trans.FreeSlotChanges(false);
			}
			else
			{
				File(trans).WriteTransactionPointer(0);
				FlushDatabaseFile(trans);
			}
		}

		public override Slot AllocateSlot(LocalTransaction trans, bool appendToFile)
		{
			int transactionLogByteCount = TransactionLogSlotLength(trans);
			IFreespaceManager freespaceManager = trans.FreespaceManager();
			if (!appendToFile && freespaceManager != null)
			{
				int blockedLength = File(trans).BytesToBlocks(transactionLogByteCount);
				Slot slot = freespaceManager.AllocateTransactionLogSlot(blockedLength);
				if (slot != null)
				{
					return File(trans).ToNonBlockedLength(slot);
				}
			}
			return File(trans).AppendBytes(transactionLogByteCount);
		}

		private void FreeSlot(LocalTransaction trans, Slot slot)
		{
			if (slot == null)
			{
				return;
			}
			if (trans.FreespaceManager() == null)
			{
				return;
			}
			trans.FreespaceManager().FreeTransactionLogSlot(File(trans).ToBlockedLength(slot)
				);
		}

		public override void ApplySlotChanges(LocalTransaction trans, Slot reservedSlot)
		{
			int slotChangeCount = CountSlotChanges(trans);
			if (slotChangeCount > 0)
			{
				Slot transactionLogSlot = SlotLongEnoughForLog(trans, reservedSlot) ? reservedSlot
					 : AllocateSlot(trans, true);
				StatefulBuffer buffer = new StatefulBuffer(trans, transactionLogSlot);
				buffer.WriteInt(transactionLogSlot.Length());
				buffer.WriteInt(slotChangeCount);
				AppendSlotChanges(trans, buffer);
				buffer.Write();
				FlushDatabaseFile(trans);
				File(trans).WriteTransactionPointer(transactionLogSlot.Address());
				FlushDatabaseFile(trans);
				if (trans.WriteSlots())
				{
					FlushDatabaseFile(trans);
				}
				File(trans).WriteTransactionPointer(0);
				FlushDatabaseFile(trans);
				if (transactionLogSlot != reservedSlot)
				{
					FreeSlot(trans, transactionLogSlot);
				}
			}
			FreeSlot(trans, reservedSlot);
		}

		private bool SlotLongEnoughForLog(LocalTransaction trans, Slot slot)
		{
			return slot != null && slot.Length() >= TransactionLogSlotLength(trans);
		}

		public override void Close()
		{
		}
		// do nothing
	}
}
