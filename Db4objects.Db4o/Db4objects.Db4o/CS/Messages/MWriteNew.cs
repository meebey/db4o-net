namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MWriteNew : Db4objects.Db4o.CS.Messages.MsgObject
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			int yapClassId = _payLoad.ReadInt();
			Db4objects.Db4o.YapFile stream = (Db4objects.Db4o.YapFile)GetStream();
			Unmarshall(Db4objects.Db4o.YapConst.INT_LENGTH);
			lock (stream.i_lock)
			{
				Db4objects.Db4o.YapClass yc = yapClassId == 0 ? null : stream.GetYapClass(yapClassId
					);
				_payLoad.WriteEmbedded();
				stream.PrefetchedIDConsumed(_payLoad.GetID());
				_payLoad.Address(stream.GetSlot(_payLoad.GetLength()));
				if (yc != null)
				{
					yc.AddFieldIndices(_payLoad, null);
				}
				stream.WriteNew(yc, _payLoad);
				GetTransaction().WritePointer(_payLoad.GetID(), _payLoad.GetAddress(), _payLoad.GetLength
					());
			}
			return true;
		}
	}
}
