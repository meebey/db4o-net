namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class LazyQueryResult : Db4objects.Db4o.Inside.Query.AbstractLateQueryResult
	{
		public LazyQueryResult(Db4objects.Db4o.Transaction trans) : base(trans)
		{
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			_iterable = ClassIndexIterable(clazz);
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 classCollectionIterator)
		{
			_iterable = ClassIndexesIterable(classCollectionIterator);
		}

		public override void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			_iterable = new _AnonymousInnerClass27(this, query);
		}

		private sealed class _AnonymousInnerClass27 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass27(LazyQueryResult _enclosing, Db4objects.Db4o.QQuery 
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

			private readonly Db4objects.Db4o.QQuery query;
		}
	}
}
