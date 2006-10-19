namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	internal class QueryResultImpl : Db4objects.Db4o.Foundation.IntArrayList, Db4objects.Db4o.Foundation.IVisitor4
		, Db4objects.Db4o.Inside.Query.IQueryResult
	{
		internal Db4objects.Db4o.Foundation.Tree i_candidates;

		internal bool i_checkDuplicates;

		internal readonly Db4objects.Db4o.Transaction i_trans;

		internal QueryResultImpl(Db4objects.Db4o.Transaction a_trans)
		{
			i_trans = a_trans;
		}

		internal QueryResultImpl(Db4objects.Db4o.Transaction trans, int initialSize) : base
			(initialSize)
		{
			i_trans = trans;
		}

		public virtual Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return IntIterator();
		}

		private class QueryResultImplIterator : Db4objects.Db4o.Foundation.MappingIterator
		{
			public QueryResultImplIterator(QueryResultImpl _enclosing, System.Collections.IEnumerator
				 iterator) : base(iterator)
			{
				this._enclosing = _enclosing;
			}

			public override bool MoveNext()
			{
				if (!base.MoveNext())
				{
					return false;
				}
				if (null == this.Current)
				{
					return this.MoveNext();
				}
				return true;
			}

			protected override object Map(object current)
			{
				lock (this._enclosing.StreamLock())
				{
					return this._enclosing.ActivatedObject(((int)current));
				}
			}

			private readonly QueryResultImpl _enclosing;
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.QueryResultImpl.QueryResultImplIterator(this, base.GetEnumerator
				());
		}

		internal object Activate(object obj)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			stream.Activate1(i_trans, obj, stream.ConfigImpl().ActivationDepth());
			return obj;
		}

		private object ActivatedObject(int id)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			object ret = stream.GetActivatedObjectFromCache(i_trans, id);
			if (ret != null)
			{
				return ret;
			}
			return stream.ReadActivatedObjectNotInCache(i_trans, id);
		}

		public virtual object Get(int index)
		{
			lock (StreamLock())
			{
				if (index < 0 || index >= Size())
				{
					throw new System.IndexOutOfRangeException();
				}
				return ActivatedObject(i_content[index]);
			}
		}

		internal void CheckDuplicates()
		{
			i_checkDuplicates = true;
		}

		public virtual void Visit(object a_tree)
		{
			Db4objects.Db4o.QCandidate candidate = (Db4objects.Db4o.QCandidate)a_tree;
			if (candidate.Include())
			{
				AddKeyCheckDuplicates(candidate._key);
			}
		}

		internal virtual void AddKeyCheckDuplicates(int a_key)
		{
			if (i_checkDuplicates)
			{
				Db4objects.Db4o.TreeInt newNode = new Db4objects.Db4o.TreeInt(a_key);
				i_candidates = Db4objects.Db4o.Foundation.Tree.Add(i_candidates, newNode);
				if (newNode._size == 0)
				{
					return;
				}
			}
			Add(a_key);
		}

		public virtual object StreamLock()
		{
			Db4objects.Db4o.YapStream stream = Stream();
			stream.CheckClosed();
			return stream.i_lock;
		}

		private Db4objects.Db4o.YapStream Stream()
		{
			return i_trans.Stream();
		}

		public virtual Db4objects.Db4o.IObjectContainer ObjectContainer()
		{
			return Stream();
		}

		public virtual void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
		{
			Sort(cmp, 0, Size() - 1);
		}

		private void Sort(Db4objects.Db4o.Query.IQueryComparator cmp, int from, int to)
		{
			if (to - from < 1)
			{
				return;
			}
			object pivot = Get(to);
			int left = from;
			int right = to;
			while (left < right)
			{
				while (left < right && cmp.Compare(pivot, Get(left)) < 0)
				{
					left++;
				}
				while (left < right && cmp.Compare(pivot, Get(right)) >= 0)
				{
					right--;
				}
				Swap(left, right);
			}
			Swap(to, right);
			Sort(cmp, from, right - 1);
			Sort(cmp, right + 1, to);
		}

		private void Swap(int left, int right)
		{
			if (left != right)
			{
				int swap = i_content[left];
				i_content[left] = i_content[right];
				i_content[right] = swap;
			}
		}
	}
}
