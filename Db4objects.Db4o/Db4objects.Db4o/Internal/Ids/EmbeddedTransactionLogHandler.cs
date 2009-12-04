/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Ids;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Ids
{
	/// <exclude></exclude>
	public class EmbeddedTransactionLogHandler : TransactionLogHandler
	{
		private int _addressOfIncompleteCommit;

		public override bool CheckForInterruptedTransaction(IdSystem idSystem, ByteArrayBuffer
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

		public override void CompleteInterruptedTransaction(IdSystem idSystem)
		{
			StatefulBuffer bytes = new StatefulBuffer(idSystem.SystemTransaction(), _addressOfIncompleteCommit
				, Const4.IntLength);
			bytes.Read();
			int length = bytes.ReadInt();
			if (length > 0)
			{
				bytes = new StatefulBuffer(idSystem.SystemTransaction(), _addressOfIncompleteCommit
					, length);
				bytes.Read();
				bytes.IncrementOffset(Const4.IntLength);
				idSystem.ReadSlotChanges(bytes);
				if (idSystem.WriteSlots())
				{
					FlushDatabaseFile(idSystem);
				}
				File(idSystem).WriteTransactionPointer(0);
				FlushDatabaseFile(idSystem);
				idSystem.FreeSlotChanges(false);
			}
			else
			{
				File(idSystem).WriteTransactionPointer(0);
				FlushDatabaseFile(idSystem);
			}
		}

		public override Slot AllocateSlot(IdSystem idSystem, bool appendToFile)
		{
			int transactionLogByteCount = TransactionLogSlotLength(idSystem);
			IFreespaceManager freespaceManager = idSystem.FreespaceManager();
			if (!appendToFile && freespaceManager != null)
			{
				int blockedLength = File(idSystem).BytesToBlocks(transactionLogByteCount);
				Slot slot = freespaceManager.AllocateTransactionLogSlot(blockedLength);
				if (slot != null)
				{
					return File(idSystem).ToNonBlockedLength(slot);
				}
			}
			return File(idSystem).AppendBytes(transactionLogByteCount);
		}

		private void FreeSlot(IdSystem idSystem, Slot slot)
		{
			if (slot == null)
			{
				return;
			}
			if (idSystem.FreespaceManager() == null)
			{
				return;
			}
			idSystem.FreespaceManager().FreeTransactionLogSlot(File(idSystem).ToBlockedLength
				(slot));
		}

		public override void ApplySlotChanges(IdSystem idSystem, Slot reservedSlot)
		{
			int slotChangeCount = CountSlotChanges(idSystem);
			if (slotChangeCount > 0)
			{
				Slot transactionLogSlot = SlotLongEnoughForLog(idSystem, reservedSlot) ? reservedSlot
					 : AllocateSlot(idSystem, true);
				StatefulBuffer buffer = new StatefulBuffer(idSystem.SystemTransaction(), transactionLogSlot
					);
				buffer.WriteInt(transactionLogSlot.Length());
				buffer.WriteInt(slotChangeCount);
				AppendSlotChanges(idSystem, buffer);
				buffer.Write();
				FlushDatabaseFile(idSystem);
				File(idSystem).WriteTransactionPointer(transactionLogSlot.Address());
				FlushDatabaseFile(idSystem);
				if (idSystem.WriteSlots())
				{
					FlushDatabaseFile(idSystem);
				}
				File(idSystem).WriteTransactionPointer(0);
				FlushDatabaseFile(idSystem);
				if (transactionLogSlot != reservedSlot)
				{
					FreeSlot(idSystem, transactionLogSlot);
				}
			}
			FreeSlot(idSystem, reservedSlot);
		}

		private bool SlotLongEnoughForLog(IdSystem idSystem, Slot slot)
		{
			return slot != null && slot.Length() >= TransactionLogSlotLength(idSystem);
		}

		public override void Close()
		{
		}
		// do nothing
	}
}
