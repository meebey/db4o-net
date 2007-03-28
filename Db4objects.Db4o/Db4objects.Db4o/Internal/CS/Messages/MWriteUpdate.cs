namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdate : Db4objects.Db4o.Internal.CS.Messages.MsgObject
		, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int yapClassId = _payLoad.ReadInt();
			Db4objects.Db4o.Internal.LocalObjectContainer stream = (Db4objects.Db4o.Internal.LocalObjectContainer
				)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				Db4objects.Db4o.Internal.ClassMetadata yc = stream.ClassMetadataForId(yapClassId);
				_payLoad.WriteEmbedded();
				int id = _payLoad.GetID();
				Transaction().DontDelete(id);
				Db4objects.Db4o.Internal.Slots.Slot oldSlot = ((Db4objects.Db4o.Internal.LocalTransaction
					)Transaction()).GetCommittedSlotOfID(id);
				stream.GetSlotForUpdate(_payLoad);
				yc.AddFieldIndices(_payLoad, oldSlot);
				_payLoad.WriteEncrypt();
			}
			return true;
		}
	}
}
