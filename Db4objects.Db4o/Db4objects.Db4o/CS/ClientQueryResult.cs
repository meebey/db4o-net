namespace Db4objects.Db4o.CS
{
	/// <exclude></exclude>
	public class ClientQueryResult : Db4objects.Db4o.Inside.Query.IdListQueryResult
	{
		public ClientQueryResult(Db4objects.Db4o.Transaction ta) : base(ta)
		{
		}

		public ClientQueryResult(Db4objects.Db4o.Transaction ta, int initialSize) : base(
			ta, initialSize)
		{
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.CS.ClientQueryResultIterator(this);
		}
	}
}
