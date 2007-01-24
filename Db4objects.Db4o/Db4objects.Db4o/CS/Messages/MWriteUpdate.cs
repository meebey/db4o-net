namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MWriteUpdate : Db4objects.Db4o.CS.Messages.MsgObject
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			int yapClassId = _payLoad.ReadInt();
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)Stream();
			Unmarshall(_payLoad._offset);
			lock (StreamLock())
			{
				Db4objects.Db4o.YapClass yc = stream.GetYapClass(yapClassId);
				_payLoad.WriteEmbedded();
				int id = _payLoad.GetID();
				Transaction().DontDelete(id);
				Db4objects.Db4o.Inside.Slots.Slot oldSlot = ((Db4objects.Db4o.YapFileTransaction)
					_trans).GetCommittedSlotOfID(id);
				stream.GetSlotForUpdate(_payLoad);
				yc.AddFieldIndices(_payLoad, oldSlot);
				_payLoad.WriteEncrypt();
			}
			return true;
		}
	}
}
