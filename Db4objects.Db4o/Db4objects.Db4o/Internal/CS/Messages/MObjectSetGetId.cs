namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetGetId : Db4objects.Db4o.Internal.CS.Messages.MObjectSet, Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = QueryResult
				(ReadInt());
			int id = queryResult.GetId(ReadInt());
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_GET_ID.GetWriterForInt(Transaction
				(), id));
			return true;
		}
	}
}
