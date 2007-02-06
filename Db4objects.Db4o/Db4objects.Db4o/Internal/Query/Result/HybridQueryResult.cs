namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public class HybridQueryResult : Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult
	{
		private Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult _delegate;

		public HybridQueryResult(Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Config.QueryEvaluationMode
			 mode) : base(transaction)
		{
			_delegate = ForMode(transaction, mode);
		}

		private static Db4objects.Db4o.Internal.Query.Result.AbstractQueryResult ForMode(
			Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Config.QueryEvaluationMode
			 mode)
		{
			if (mode == Db4objects.Db4o.Config.QueryEvaluationMode.LAZY)
			{
				return new Db4objects.Db4o.Internal.Query.Result.LazyQueryResult(transaction);
			}
			if (mode == Db4objects.Db4o.Config.QueryEvaluationMode.SNAPSHOT)
			{
				return new Db4objects.Db4o.Internal.Query.Result.SnapShotQueryResult(transaction);
			}
			return new Db4objects.Db4o.Internal.Query.Result.IdListQueryResult(transaction);
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

		public override void LoadFromClassIndex(Db4objects.Db4o.Internal.ClassMetadata clazz
			)
		{
			_delegate.LoadFromClassIndex(clazz);
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.Internal.ClassMetadataIterator
			 iterator)
		{
			_delegate.LoadFromClassIndexes(iterator);
		}

		public override void LoadFromIdReader(Db4objects.Db4o.Internal.Buffer reader)
		{
			_delegate.LoadFromIdReader(reader);
		}

		public override void LoadFromQuery(Db4objects.Db4o.Internal.Query.Processor.QQuery
			 query)
		{
			if (query.RequiresSort())
			{
				_delegate = new Db4objects.Db4o.Internal.Query.Result.IdListQueryResult(Transaction
					());
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
