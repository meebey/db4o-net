namespace Db4objects.Db4o.Internal.CS.Messages
{
	/// <exclude></exclude>
	public class MObjectSetIndexOf : Db4objects.Db4o.Internal.CS.Messages.MObjectSet, 
		Db4objects.Db4o.Internal.CS.Messages.IServerSideMessage
	{
		public virtual bool ProcessAtServer()
		{
			Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult queryResult = QueryResult
				(ReadInt());
			int id = queryResult.IndexOf(ReadInt());
			Write(Db4objects.Db4o.Internal.CS.Messages.Msg.OBJECTSET_INDEXOF.GetWriterForInt(
				Transaction(), id));
			return true;
		}
	}
}
