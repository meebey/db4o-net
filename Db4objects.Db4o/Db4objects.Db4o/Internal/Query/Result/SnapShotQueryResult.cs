namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public class SnapShotQueryResult : Db4objects.Db4o.Internal.Query.Result.AbstractLateQueryResult
	{
		public SnapShotQueryResult(Db4objects.Db4o.Internal.Transaction transaction) : base
			(transaction)
		{
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.Internal.ClassMetadata clazz
			)
		{
			CreateSnapshot(ClassIndexIterable(clazz));
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.Internal.ClassMetadataIterator
			 classCollectionIterator)
		{
			CreateSnapshot(ClassIndexesIterable(classCollectionIterator));
		}

		public override void LoadFromQuery(Db4objects.Db4o.Internal.Query.Processor.QQuery
			 query)
		{
			System.Collections.IEnumerator _iterator = query.ExecuteSnapshot();
			_iterable = new _AnonymousInnerClass29(this, _iterator);
		}

		private sealed class _AnonymousInnerClass29 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass29(SnapShotQueryResult _enclosing, System.Collections.IEnumerator
				 _iterator)
			{
				this._enclosing = _enclosing;
				this._iterator = _iterator;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				_iterator.Reset();
				return _iterator;
			}

			private readonly SnapShotQueryResult _enclosing;

			private readonly System.Collections.IEnumerator _iterator;
		}

		private void CreateSnapshot(System.Collections.IEnumerable iterable)
		{
			Db4objects.Db4o.Foundation.Tree ids = Db4objects.Db4o.Internal.TreeInt.AddAll(null
				, new Db4objects.Db4o.Foundation.IntIterator4Adaptor(iterable));
			_iterable = new _AnonymousInnerClass39(this, ids);
		}

		private sealed class _AnonymousInnerClass39 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass39(SnapShotQueryResult _enclosing, Db4objects.Db4o.Foundation.Tree
				 ids)
			{
				this._enclosing = _enclosing;
				this.ids = ids;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new Db4objects.Db4o.Foundation.TreeKeyIterator(ids);
			}

			private readonly SnapShotQueryResult _enclosing;

			private readonly Db4objects.Db4o.Foundation.Tree ids;
		}
	}
}
