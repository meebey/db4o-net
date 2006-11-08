namespace Db4objects.Db4o.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetIndexOf : Db4objects.Db4o.CS.Messages.MObjectSet
	{
		public override bool ProcessAtServer(Db4objects.Db4o.CS.YapServerThread serverThread
			)
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = QueryResult(serverThread
				, ReadInt());
			int id = queryResult.IndexOf(ReadInt());
			serverThread.Write(Db4objects.Db4o.CS.Messages.Msg.OBJECTSET_INDEXOF.GetWriterForInt
				(Transaction(), id));
			return true;
		}
	}
}
