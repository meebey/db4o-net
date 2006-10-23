namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class IdListQueryResult : Db4objects.Db4o.Foundation.IntArrayList, Db4objects.Db4o.Foundation.IVisitor4
		, Db4objects.Db4o.Inside.Query.IQueryResult
	{
		private readonly Db4objects.Db4o.Transaction _transaction;

		private Db4objects.Db4o.Foundation.Tree _candidates;

		private bool _checkDuplicates;

		public IdListQueryResult(Db4objects.Db4o.Transaction a_trans)
		{
			_transaction = a_trans;
		}

		protected IdListQueryResult(Db4objects.Db4o.Transaction trans, int initialSize) : 
			base(initialSize)
		{
			_transaction = trans;
		}

		public virtual Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return IntIterator();
		}

		private class QueryResultImplIterator : Db4objects.Db4o.Foundation.MappingIterator
		{
			public QueryResultImplIterator(IdListQueryResult _enclosing, System.Collections.IEnumerator
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

			private readonly IdListQueryResult _enclosing;
		}

		public override System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Db4o.Inside.Query.IdListQueryResult.QueryResultImplIterator
				(this, base.GetEnumerator());
		}

		public object Activate(object obj)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			stream.Activate1(_transaction, obj, stream.ConfigImpl().ActivationDepth());
			return obj;
		}

		private object ActivatedObject(int id)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			object ret = stream.GetActivatedObjectFromCache(_transaction, id);
			if (ret != null)
			{
				return ret;
			}
			return stream.ReadActivatedObjectNotInCache(_transaction, id);
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

		public void CheckDuplicates()
		{
			_checkDuplicates = true;
		}

		public virtual void Visit(object a_tree)
		{
			Db4objects.Db4o.QCandidate candidate = (Db4objects.Db4o.QCandidate)a_tree;
			if (candidate.Include())
			{
				AddKeyCheckDuplicates(candidate._key);
			}
		}

		public virtual void AddKeyCheckDuplicates(int a_key)
		{
			if (_checkDuplicates)
			{
				Db4objects.Db4o.TreeInt newNode = new Db4objects.Db4o.TreeInt(a_key);
				_candidates = Db4objects.Db4o.Foundation.Tree.Add(_candidates, newNode);
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
			return stream.Lock();
		}

		public virtual Db4objects.Db4o.YapStream Stream()
		{
			return _transaction.Stream();
		}

		public virtual Db4objects.Db4o.Ext.IExtObjectContainer ObjectContainer()
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

		public virtual void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy index = clazz.Index();
			index.TraverseAll(_transaction, new _AnonymousInnerClass170(this));
		}

		private sealed class _AnonymousInnerClass170 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass170(IdListQueryResult _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.Add(((int)a_object));
			}

			private readonly IdListQueryResult _enclosing;
		}

		public virtual void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			query.ExecuteLocal(this);
		}

		public virtual void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
			 iter)
		{
			Db4objects.Db4o.Foundation.Tree[] duplicates = new Db4objects.Db4o.Foundation.Tree
				[1];
			while (iter.MoveNext())
			{
				Db4objects.Db4o.YapClass yapClass = iter.CurrentClass();
				if (yapClass.GetName() != null)
				{
					Db4objects.Db4o.Reflect.IReflectClass claxx = yapClass.ClassReflector();
					if (claxx == null || !(Stream().i_handlers.ICLASS_INTERNAL.IsAssignableFrom(claxx
						)))
					{
						Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy index = yapClass.Index();
						index.TraverseAll(_transaction, new _AnonymousInnerClass193(this, duplicates));
					}
				}
			}
		}

		private sealed class _AnonymousInnerClass193 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass193(IdListQueryResult _enclosing, Db4objects.Db4o.Foundation.Tree[]
				 duplicates)
			{
				this._enclosing = _enclosing;
				this.duplicates = duplicates;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				Db4objects.Db4o.TreeInt newNode = new Db4objects.Db4o.TreeInt(id);
				duplicates[0] = Db4objects.Db4o.Foundation.Tree.Add(duplicates[0], newNode);
				if (newNode.Size() != 0)
				{
					this._enclosing.Add(id);
				}
			}

			private readonly IdListQueryResult _enclosing;

			private readonly Db4objects.Db4o.Foundation.Tree[] duplicates;
		}

		public virtual void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
		{
			int size = reader.ReadInt();
			for (int i = 0; i < size; i++)
			{
				Add(reader.ReadInt());
			}
		}
	}
}
