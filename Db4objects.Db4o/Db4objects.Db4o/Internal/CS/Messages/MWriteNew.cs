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
				Db4objects.Db4o.Internal.ClassMetadata yc = yapClassId == 0 ? null : stream.GetYapClass
					(yapClassId);
				_payLoad.WriteEmbedded();
				stream.PrefetchedIDConsumed(_payLoad.GetID());
				_payLoad.Address(stream.GetSlot(_payLoad.GetLength()));
				if (yc != null)
				{
					yc.AddFieldIndices(_payLoad, null);
				}
				stream.WriteNew(yc, _payLoad);
				Transaction().WritePointer(_payLoad.GetID(), _payLoad.GetAddress(), _payLoad.GetLength
					());
			}
			return true;
		}
	}
}
