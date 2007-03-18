namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteNew : Db4objects.Db4o.Internal.CS.Messages.MsgObject
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			int yapClassId = _payLoad.ReadInt();
			Db4objects.Db4o.Internal.LocalObjectContainer stream = (Db4objects.Db4o.Internal.LocalObjectContainer
				)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.ClassMetadata yc = yapClassId == 0 ? null : stream.ClassMetadataForId
					(yapClassId);
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
				Transaction().WritePointer(id, address, length);
			}
			return true;
		}
	}
}
