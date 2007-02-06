namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public class IdTreeQueryResult : Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult
	{
		private Db4objects.Db4o.Foundation.Tree _ids;

		public IdTreeQueryResult(Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Foundation.IIntIterator4
			 ids) : base(transaction)
		{
			_ids = Db4objects.Db4o.Internal.TreeInt.AddAll(null, ids);
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return new Db4objects.Db4o.Foundation.IntIterator4Adaptor(new Db4objects.Db4o.Foundation.TreeKeyIterator
				(_ids));
		}

		public override int Size()
		{
			if (_ids == null)
			{
				return 0;
			}
			return _ids.Size();
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportSort
			()
		{
			return ToIdList();
		}

		public override Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult SupportElementAccess
			()
		{
			return ToIdList();
		}
	}
}
