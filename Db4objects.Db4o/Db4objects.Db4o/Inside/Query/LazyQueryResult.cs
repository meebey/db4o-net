namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class LazyQueryResult : Db4objects.Db4o.Inside.Query.IQueryResult
	{
		private readonly Db4objects.Db4o.Transaction _transaction;

		private System.Collections.IEnumerable _iterable;

		public LazyQueryResult(Db4objects.Db4o.Transaction trans)
		{
			_transaction = trans;
		}

		public virtual object Get(int index)
		{
			throw new System.NotImplementedException();
		}

		public virtual int IndexOf(int id)
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			if (_iterable == null)
			{
				throw new System.InvalidOperationException();
			}
			return new Db4objects.Db4o.Foundation.IntIterator4Adaptor(_iterable.GetEnumerator
				());
		}

		public virtual void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy index = clazz.Index();
			if (!(index is Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy))
			{
				throw new System.InvalidOperationException();
			}
			Db4objects.Db4o.Inside.Btree.BTree btree = ((Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy
				)index).Btree();
			_iterable = new _AnonymousInnerClass47(this, btree);
		}

		private sealed class _AnonymousInnerClass47 : System.Collections.IEnumerable
		{
			public _AnonymousInnerClass47(LazyQueryResult _enclosing, Db4objects.Db4o.Inside.Btree.BTree
				 btree)
			{
				this._enclosing = _enclosing;
				this.btree = btree;
			}

			public System.Collections.IEnumerator GetEnumerator()
			{
				return new Db4objects.Db4o.Inside.Btree.BTreeRangeSingle(this._enclosing.Transaction
					(), btree, btree.FirstPointer(this._enclosing.Transaction()), null).Keys();
			}

			private readonly LazyQueryResult _enclosing;

			private readonly Db4objects.Db4o.Inside.Btree.BTree btree;
		}

		public virtual Db4objects.Db4o.Transaction Transaction()
		{
			return _transaction;
		}

		public virtual void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 iterator)
		{
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
		{
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			throw new System.NotImplementedException();
		}

		public virtual Db4objects.Db4o.YapStream Stream()
		{
			return _transaction.Stream();
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer()
		{
			return Stream();
		}

		public virtual int Size()
		{
			throw new System.NotImplementedException();
		}

		public virtual void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			throw new System.NotImplementedException();
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			throw new System.NotImplementedException();
		}
	}
}
