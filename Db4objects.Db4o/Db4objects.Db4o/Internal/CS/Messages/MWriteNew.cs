/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteNew : MsgObject, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int yapClassId = _payLoad.ReadInt();
			LocalObjectContainer stream = (LocalObjectContainer)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				ClassMetadata classMetadata = yapClassId == 0 ? null : stream.ClassMetadataForId(
					yapClassId);
				int id = _payLoad.GetID();
				stream.PrefetchedIDConsumed(id);
				Transaction().SlotFreePointerOnRollback(id);
				Slot slot = stream.GetSlot(_payLoad.Length());
				_payLoad.Address(slot.Address());
				Transaction().SlotFreeOnRollback(id, slot);
				if (classMetadata != null)
				{
					classMetadata.AddFieldIndices(_payLoad, null);
				}
				stream.WriteNew(Transaction(), _payLoad.Pointer(), classMetadata, _payLoad);
				ServerTransaction().WritePointer(id, slot);
			}
			return true;
		}
	}
}
