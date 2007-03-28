namespace Db4objects.Db4o.Internal.CS.Messages
{
	public class MObjectSetSize : Db4objects.Db4o.Internal.CS.Messages.MObjectSet, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = QueryResult
				(ReadInt());
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_SIZE.GetWriterForInt(Transaction
				(), queryResult.Size()));
			return true;
		}
	}
}
