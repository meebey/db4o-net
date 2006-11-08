namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class IdTreeQueryResult : Db4objects.Db4o.Inside.Query.AbstractQueryResult
	{
		private Db4objects.Db4o.TreeInt _ids;

		public IdTreeQueryResult(Db4objects.Db4o.Transaction transaction, Db4objects.Db4o.Inside.Query.IQueryResult
			 queryResult) : base(transaction)
		{
			Db4objects.Db4o.Foundation.IIntIterator4 i = queryResult.IterateIDs();
			if (!i.MoveNext())
			{
				return;
			}
			_ids = new Db4objects.Db4o.TreeInt(i.CurrentInt());
			while (i.MoveNext())
			{
				_ids = (Db4objects.Db4o.TreeInt)_ids.Add(new Db4objects.Db4o.TreeInt(i.CurrentInt
					()));
			}
		}

		public override object Get(int index)
		{
			throw new System.NotImplementedException();
		}

		public override int GetId(int index)
		{
			throw new System.NotImplementedException();
		}

		public override int IndexOf(int id)
		{
			throw new System.NotImplementedException();
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return new Db4objects.Db4o.Foundation.IntIterator4Adaptor(new Db4objects.Db4o.Foundation.TreeKeyIterator
				(_ids));
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			throw new System.NotImplementedException();
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 iterator)
		{
			throw new System.NotImplementedException();
		}

		public override void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
		{
			throw new System.NotImplementedException();
		}

		public override void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			throw new System.NotImplementedException();
		}

		public override int Size()
		{
			if (_ids == null)
			{
				return 0;
			}
			return _ids.Size();
		}

		public override void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			throw new System.NotImplementedException();
		}

		public override Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportSort()
		{
			return ToIdList();
		}

		public override Db4objects.Db4o.Inside.Query.AbstractQueryResult SupportElementAccess
			()
		{
			return ToIdList();
		}
	}
}
