namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MWriteUpdateDeleteMembers : Db4objects.Db4o.CS.Messages.MsgD
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			Db4objects.Db4o.YapStream stream = GetStream();
			lock (stream.i_lock)
			{
				this.GetTransaction().WriteUpdateDeleteMembers(ReadInt(), stream.GetYapClass(ReadInt
					()), ReadInt(), ReadInt());
			}
			return true;
		}
	}
}
