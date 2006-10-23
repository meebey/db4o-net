namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class StatefulQueryResult
	{
		private readonly Db4objects.Db4o.Inside.Query.IQueryResult _delegate;

		private readonly Db4objects.Db4o.Foundation.Iterable4Adaptor _iterable;

		public StatefulQueryResult(Db4objects.Db4o.Inside.Query.IQueryResult queryResult)
		{
			_delegate = queryResult;
			_iterable = new Db4objects.Db4o.Foundation.Iterable4Adaptor(queryResult);
		}

		public virtual object Get(int index)
		{
			return _delegate.Get(ReverseIndex(index));
		}

		public virtual long[] GetIDs()
		{
			long[] ids = new long[Size()];
			int i = 0;
			Db4objects.Db4o.Foundation.IIntIterator4 iterator = _delegate.IterateIDs();
			while (iterator.MoveNext())
			{
				ids[i++] = iterator.CurrentInt();
			}
			return ids;
		}

		public virtual bool HasNext()
		{
			return _iterable.HasNext();
		}

		public virtual object Next()
		{
			return _iterable.Next();
		}

		public virtual void Reset()
		{
			_iterable.Reset();
		}

		public virtual int Size()
		{
			return _delegate.Size();
		}

		public virtual void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			_delegate.Sort(cmp);
		}

		internal virtual object StreamLock()
		{
			return _delegate.StreamLock();
		}

		internal virtual Db4objects.Db4o.IObjectContainer ObjectContainer()
		{
			return _delegate.ObjectContainer();
		}

		public virtual int IndexOf(object a_object)
		{
			lock (StreamLock())
			{
				int id = (int)ObjectContainer().Ext().GetID(a_object);
				if (id <= 0)
				{
					return -1;
				}
				return ReverseIndex(_delegate.IndexOf(id));
			}
		}

		private int ReverseIndex(int idx)
		{
			return Size() - idx - 1;
		}

		public virtual System.Collections.IEnumerator IterateIDs()
		{
			return _delegate.IterateIDs();
		}

		public virtual System.Collections.IEnumerator Iterator()
		{
			return _delegate.GetEnumerator();
		}
	}
}