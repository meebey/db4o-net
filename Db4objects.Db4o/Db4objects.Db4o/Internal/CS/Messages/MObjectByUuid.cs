namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MObjectByUuid : Db4objects.Db4o.Internal.CS.Messages.MsgD
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			long uuid = ReadLong();
			byte[] signature = ReadBytes();
			int id = 0;
			Db4objects.Db4o.Internal.Transaction trans = Transaction();
			lock (StreamLock())
			{
				try
				{
					Db4objects.Db4o.Internal.HardObjectReference hardRef = trans.GetHardReferenceBySignature
						(uuid, signature);
					if (hardRef._reference != null)
					{
						id = hardRef._reference.GetID();
					}
				}
				catch (System.Exception e)
				{
				}
			}
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECT_BY_UUID.GetWriterForInt
				(trans, id));
			return true;
		}
	}
}
