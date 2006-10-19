namespace Db4objects.Db4o
{
	internal sealed class MWriteUpdate : Db4objects.Db4o.MsgObject
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			int yapClassId = _payLoad.ReadInt();
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)GetStream();
			Unmarshall(Db4objects.Db4o.YapConst.INT_LENGTH);
			lock (stream.i_lock)
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
