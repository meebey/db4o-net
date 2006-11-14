namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MWriteUpdate : Db4objects.Db4o.CS.Messages.MsgObject
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int yapClassId = _payLoad.ReadInt();
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)Stream();
			Unmarshall(Db4objects.Db4o.YapConst.INT_LENGTH);
			lock (StreamLock())
			{
				Db4objects.Db4o.YapClass yc = stream.GetYapClass(yapClassId);
				_payLoad.WriteEmbedded();
				Db4objects.Db4o.Inside.Slots.Slot oldSlot = _trans.GetCommittedSlotOfID(_payLoad.
					GetID());
				stream.GetSlotForUpdate(_payLoad);
				yc.AddFieldIndices(_payLoad, oldSlot);
				_payLoad.WriteEncrypt();
			}
			return true;
		}
	}
}
