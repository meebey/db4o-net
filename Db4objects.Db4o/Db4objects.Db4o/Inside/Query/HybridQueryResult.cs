namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class HybridQueryResult : Db4objects.Db4o.Inside.Query.AbstractQueryResult
	{
		private Db4objects.Db4o.Inside.Query.AbstractQueryResult _delegate;

		public HybridQueryResult(Db4objects.Db4o.Transaction transaction, Db4objects.Db4o.Inside.Query.AbstractQueryResult
			 delegate_) : base(transaction)
		{
			_delegate = delegate_;
		}

		public HybridQueryResult(Db4objects.Db4o.Transaction transaction) : this(transaction
			, new Db4objects.Db4o.Inside.Query.LazyQueryResult(transaction))
		{
		}

		public override object Get(int index)
		{
			_delegate = _delegate.SupportElementAccess();
			return _delegate.Get(index);
		}

		public override int GetId(int index)
		{
			_delegate = _delegate.SupportElementAccess();
			return _delegate.GetId(index);
		}

		public override int IndexOf(int id)
		{
			_delegate = _delegate.SupportElementAccess();
			return _delegate.IndexOf(id);
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return _delegate.IterateIDs();
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return _delegate.GetEnumerator();
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			_delegate.LoadFromClassIndex(clazz);
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 iterator)
		{
			_delegate.LoadFromClassIndexes(iterator);
		}

		public override void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
		{
			_delegate.LoadFromIdReader(reader);
		}

		public override void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			if (query.RequiresSort())
			{
				_delegate = new Db4objects.Db4o.Inside.Query.IdListQueryResult(Transaction());
			}
			_delegate.LoadFromQuery(query);
		}

		public override int Size()
		{
			_delegate = _delegate.SupportSize();
			return _delegate.Size();
		}

		public override void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			_delegate = _delegate.SupportSort();
			_delegate.Sort(cmp);
		}
	}
}
