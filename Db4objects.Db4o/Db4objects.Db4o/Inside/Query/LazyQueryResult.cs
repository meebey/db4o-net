namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class LazyQueryResult : Db4objects.Db4o.Inside.Query.IQueryResult
	{
		private readonly Db4objects.Db4o.Transaction _transaction;

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
			throw new System.NotImplementedException();
		}

		public virtual void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			throw new System.NotImplementedException();
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
