namespace Db4objects.Db4o
{
	internal class MTaDelete : Db4objects.Db4o.MsgD
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 @in)
		{
			int id = _payLoad.ReadInt();
			int cascade = _payLoad.ReadInt();
			Db4objects.Db4o.Transaction trans = GetTransaction();
			Db4objects.Db4o.YapStream stream = trans.Stream();
			lock (stream.i_lock)
			{
				object[] arr = stream.GetObjectAndYapObjectByID(trans, id);
				trans.Delete((Db4objects.Db4o.YapObject)arr[1], cascade);
				return true;
			}
		}
	}
}
