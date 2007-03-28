namespace Db4objects.Db4o.Internal.CS.Messages
{
	public sealed class MSetSemaphore : Db4objects.Db4o.Internal.CS.Messages.MsgD, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			int timeout = ReadInt();
			string name = ReadString();
			Db4objects.Db4o.Internal.LocalObjectContainer stream = (Db4objects.Db4o.Internal.LocalObjectContainer
				)Stream();
			bool res = stream.SetSemaphore(Transaction(), name, timeout);
			if (res)
			{
				Write(Db4objects.Db4o.Internal.CS.Messages.Msg.SUCCESS);
			}
			else
			{
				Write(Db4objects.Db4o.Internal.CS.Messages.Msg.FAILED);
			}
			return true;
		}
	}
}
