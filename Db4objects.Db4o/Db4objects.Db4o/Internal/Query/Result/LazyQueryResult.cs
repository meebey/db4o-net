namespace Db4objects.Db4o.Internal.Query.Result
{
	/// <exclude></exclude>
	public class LazyQueryResult : Db4objects.Db4o.Internal.Query.Result.AbstractLateQueryResult
	{
		public LazyQueryResult(Db4objects.Db4o.Internal.Transaction trans) : base(trans)
		{
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.Internal.ClassMetadata clazz
			)
		{
			_iterable = ClassIndexIterable(clazz);
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.Internal.ClassMetadataIterator
			 classCollectionIterator)
		{
			_iterable = ClassIndexesIterable(classCollectionIterator);
		}

		public override void LoadFromQuery(Db4objects.Db4o.Internal.Query.Processor.QQuery
			 query)
		{
			_iterable = new _AnonymousInnerClass28(this, query);
		}

		private sealed class _AnonymousInnerClass28 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass28(LazyQueryResult _enclosing, Db4objects.Db4o.Internal.Query.Processor.QQuery
				 query)
			{
				this._enclosing = _enclosing;
				this.query = query;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return query.ExecuteLazy();
			}

			private readonly LazyQueryResult _enclosing;

			private readonly Db4objects.Db4o.Internal.Query.Processor.QQuery query;
		}
	}
}
