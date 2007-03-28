namespace Db4objects.Db4o.Internal.CS.Messages
{
	internal sealed class MCommit : Db4objects.Db4o.Internal.CS.Messages.Msg, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public bool ProcessAtServer()
		{
			try
			{
				Transaction().Commit();
			}
			catch (Db4objects.Db4o.Ext.Db4oException db4oException)
			{
				Write(Db4objects.Db4o.Internal.CS.Messages.MCommitResponse.CreateWithException(Transaction
					(), db4oException));
				return true;
			}
			Write(Db4objects.Db4o.Internal.CS.Messages.MCommitResponse.CreateWithoutException
				(Transaction()));
			return true;
		}
	}
}
