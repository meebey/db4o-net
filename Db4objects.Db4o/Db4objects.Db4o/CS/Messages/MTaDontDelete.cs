namespace Db4objects.Db4o.CS.Messages
{
	public class MTaDontDelete : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 @in)
		{
			int classID = _payLoad.ReadInt();
			int id = _payLoad.ReadInt();
			Db4objects.Db4o.Transaction trans = GetTransaction();
			Db4objects.Db4o.YapStream stream = trans.Stream();
			lock (stream.i_lock)
			{
				trans.DontDelete(classID, id);
				return true;
			}
		}
	}
}
