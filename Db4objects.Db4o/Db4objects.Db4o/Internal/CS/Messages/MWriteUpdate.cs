namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MWriteUpdate : Db4objects.Db4o.Internal.CS.Messages.MsgObject
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
				Db4objects.Db4o.Internal.ClassMetadata yc = stream.GetYapClass(yapClassId);
				_payLoad.WriteEmbedded();
				int id = _payLoad.GetID();
				Transaction().DontDelete(id);
				Db4objects.Db4o.Internal.Slots.Slot oldSlot = ((Db4objects.Db4o.Internal.LocalTransaction
					)_trans).GetCommittedSlotOfID(id);
				stream.GetSlotForUpdate(_payLoad);
				yc.AddFieldIndices(_payLoad, oldSlot);
				_payLoad.WriteEncrypt();
			}
			return true;
		}
	}
}
