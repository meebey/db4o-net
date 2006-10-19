namespace Db4objects.Db4o
{
	internal sealed class MRollback : Db4objects.Db4o.Msg
	{
		internal sealed override bool ProcessMessageAtServer(Db4objects.Db4o.Foundation.Network.IYapSocket
			 sock)
		{
			this.GetTransaction().Rollback();
			return true;
		}
	}
}
