namespace Db4objects.Db4o.Internal.CS.Messages
{
	internal sealed class MCommit : Db4objects.Db4o.Internal.CS.Messages.Msg
	{
		public sealed override bool ProcessAtServer(Db4objects.Db4o.Internal.CS.ServerMessageDispatcher
			 serverThread)
		{
			try
			{
				Transaction().Commit();
			}
			catch (Db4objects.Db4o.Ext.Db4oException db4oException)
			{
				serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.MCommitResponse.CreateWithException
					(Transaction(), db4oException));
				return true;
			}
			serverThread.Write(Db4objects.Db4o.Internal.CS.Messages.MCommitResponse.CreateWithoutException
				(Transaction()));
			return true;
		}
	}
}
