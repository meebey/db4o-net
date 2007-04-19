using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

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
				int length = _payLoad.GetLength();
				stream.PrefetchedIDConsumed(id);
				Transaction().SlotFreePointerOnRollback(id);
				int address = stream.GetSlot(length);
				_payLoad.Address(address);
				Transaction().SlotFreeOnRollback(id, address, length);
				if (yc != null)
				{
					yc.AddFieldIndices(_payLoad, null);
				}
				stream.WriteNew(yc, _payLoad);
				ServerTransaction().WritePointer(id, address, length);
			}
			return true;
		}
	}
}
