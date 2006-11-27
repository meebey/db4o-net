namespace Db4objects.Db4o.Inside.Cluster
{
	/// <exclude></exclude>
	public class ClusterQueryResult : Db4objects.Db4o.Inside.Query.IQueryResult
	{
		private readonly Db4objects.Db4o.Cluster.Cluster _cluster;

		private readonly Db4objects.Db4o.IObjectSet[] _objectSets;

		private readonly int[] _sizes;

		private readonly int _size;

		public ClusterQueryResult(Db4objects.Db4o.Cluster.Cluster cluster, Db4objects.Db4o.Query.IQuery[]
			 queries)
		{
			_cluster = cluster;
			_objectSets = new Db4objects.Db4o.IObjectSet[queries.Length];
			_sizes = new int[queries.Length];
			int size = 0;
			for (int i = 0; i < queries.Length; i++)
			{
				_objectSets[i] = queries[i].Execute();
				_sizes[i] = _objectSets[i].Size();
				size += _sizes[i];
			}
			_size = size;
		}

		private sealed class ClusterQueryResultIntIterator : Db4objects.Db4o.Foundation.IIntIterator4
		{
			private readonly Db4objects.Db4o.Foundation.CompositeIterator4 _delegate;

			public ClusterQueryResultIntIterator(System.Collections.IEnumerator[] iterators)
			{
				_delegate = new Db4objects.Db4o.Foundation.CompositeIterator4(iterators);
			}

			public bool MoveNext()
			{
				return _delegate.MoveNext();
			}

			public object Current
			{
				get
				{
					return _delegate.Current;
				}
			}

			public void Reset()
			{
				_delegate.Reset();
			}

			public int CurrentInt()
			{
				return ((Db4objects.Db4o.Foundation.IIntIterator4)_delegate.CurrentIterator()).CurrentInt
					();
			}
		}

		public virtual Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			lock (_cluster)
			{
				System.Collections.IEnumerator[] iterators = new System.Collections.IEnumerator[_objectSets
					.Length];
				for (int i = 0; i < _objectSets.Length; i++)
				{
					iterators[i] = ((Db4objects.Db4o.Inside.Query.ObjectSetFacade)_objectSets[i])._delegate
						.IterateIDs();
				}
				return new Db4objects.Db4o.Inside.Cluster.ClusterQueryResult.ClusterQueryResultIntIterator
					(iterators);
			}
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			lock (_cluster)
			{
				System.Collections.IEnumerator[] iterators = new System.Collections.IEnumerator[_objectSets
					.Length];
				for (int i = 0; i < _objectSets.Length; i++)
				{
					iterators[i] = ((Db4objects.Db4o.Inside.Query.ObjectSetFacade)_objectSets[i])._delegate
						.Iterator();
				}
				return new Db4objects.Db4o.Foundation.CompositeIterator4(iterators);
			}
		}

		public virtual int Size()
		{
			return _size;
		}

		public virtual object Get(int index)
		{
			lock (_cluster)
			{
				if (index < 0 || index >= Size())
				{
					throw new System.IndexOutOfRangeException();
				}
				int i = 0;
				while (index >= _sizes[i])
				{
					index -= _sizes[i];
					i++;
				}
				return ((Db4objects.Db4o.Inside.Query.ObjectSetFacade)_objectSets[i]).Get(index);
			}
		}

		public virtual object StreamLock()
		{
			return _cluster;
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer()
		{
			throw new System.NotSupportedException();
		}

		public virtual int IndexOf(int id)
		{
			throw new System.NotSupportedException();
		}

		public virtual void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			throw new System.NotSupportedException();
		}

		public virtual void LoadFromClassIndex(Db4objects.Db4o.YapClass c)
		{
			throw new System.NotSupportedException();
		}

		public virtual void LoadFromQuery(Db4objects.Db4o.QQuery q)
		{
			throw new System.NotSupportedException();
		}

		public virtual void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 i)
		{
			throw new System.NotSupportedException();
		}

		public virtual void LoadFromIdReader(Db4objects.Db4o.YapReader r)
		{
			throw new System.NotSupportedException();
		}
	}
}
