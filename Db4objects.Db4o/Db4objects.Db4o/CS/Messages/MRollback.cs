namespace Db4objects.Db4o.CS.Messages
{
	public sealed class MRollback : Db4objects.Db4o.CS.Messages.Msg
	{
		public sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			this.GetTransaction().Rollback();
			return true;
		}
	}
}
