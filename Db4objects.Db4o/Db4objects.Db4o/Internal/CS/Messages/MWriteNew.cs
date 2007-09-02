/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
				ClassMetadata yc = yapClassId == 0 ? null : stream.ClassMetadataForId(yapClassId);
				_payLoad.WriteEmbedded();
				int id = _payLoad.GetID();
				stream.PrefetchedIDConsumed(id);
				Transaction().SlotFreePointerOnRollback(id);
				Slot slot = stream.GetSlot(_payLoad.Length());
				_payLoad.Address(slot.Address());
				Transaction().SlotFreeOnRollback(id, slot);
				if (yc != null)
				{
					yc.AddFieldIndices(_payLoad, null);
				}
				stream.WriteNew(yc, _payLoad);
				ServerTransaction().WritePointer(id, slot);
			}
			return true;
		}
	}
}
