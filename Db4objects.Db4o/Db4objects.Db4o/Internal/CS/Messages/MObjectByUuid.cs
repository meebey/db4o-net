using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.CS.Messages;

namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MObjectByUuid : MsgD, IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			long uuid = ReadLong();
			byte[] signature = ReadBytes();
			int id = 0;
			Transaction trans = Transaction();
			lock (StreamLock())
			{
				try
				{
					HardObjectReference hardRef = trans.GetHardReferenceBySignature(uuid, signature);
					if (hardRef._reference != null)
					{
						id = hardRef._reference.GetID();
					}
				}
				catch (Exception e)
				{
				}
			}
			Write(Msg.OBJECT_BY_UUID.GetWriterForInt(trans, id));
			return true;
		}
	}
}
