namespace Db4objects.Db4o.Internal.CS
{
	/// <exclude></exclude>
	public class ClientQueryResult : Db4objects.Db4o.Internal.Query.Result.IdListQueryResult
	{
		public ClientQueryResult(Db4objects.Db4o.Internal.Transaction ta) : base(ta)
		{
		}

		public ClientQueryResult(Db4objects.Db4o.Internal.Transaction ta, int initialSize
			) : base(ta, initialSize)
		{
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.Internal.CS.ClientQueryResultIterator(this);
		}
	}
}
