/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4objects.Db4o.CS.Internal.Messages;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.CS.Internal.Messages
{
	public sealed class MWriteNew : MsgObject, IServerSideMessage
	{
		public void ProcessAtServer()
		{
			int classMetadataId = _payLoad.ReadInt();
			LocalObjectContainer container = (LocalObjectContainer)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				ClassMetadata classMetadata = classMetadataId == 0 ? null : container.ClassMetadataForID
					(classMetadataId);
				int id = _payLoad.GetID();
				container.PrefetchedIDConsumed(id);
				Transaction().SlotFreePointerOnRollback(id);
				Slot slot = container.GetSlot(_payLoad.Length());
				_payLoad.Address(slot.Address());
				Transaction().SlotFreeOnRollback(id, slot);
				if (classMetadata != null)
				{
					classMetadata.AddFieldIndices(_payLoad, null);
				}
				container.WriteNew(Transaction(), _payLoad.Pointer(), classMetadata, _payLoad);
				ServerTransaction().WritePointer(id, slot);
			}
		}
	}
}
