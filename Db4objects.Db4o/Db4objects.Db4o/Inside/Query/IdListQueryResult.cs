namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class IdListQueryResult : Db4objects.Db4o.Foundation.IVisitor4, Db4objects.Db4o.Inside.Query.IQueryResult
	{
		private readonly Db4objects.Db4o.Transaction _transaction;

		private Db4objects.Db4o.Foundation.Tree _candidates;

		private bool _checkDuplicates;

		private readonly Db4objects.Db4o.Foundation.IntArrayList _ids;

		protected IdListQueryResult(Db4objects.Db4o.Transaction trans, int initialSize)
		{
			_ids = new Db4objects.Db4o.Foundation.IntArrayList(initialSize);
			_transaction = trans;
		}

		public IdListQueryResult(Db4objects.Db4o.Transaction trans) : this(trans, 0)
		{
		}

		public virtual Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return _ids.IntIterator();
		}

		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return new _AnonymousInnerClass39(this, _ids.GetEnumerator());
		}

		private sealed class _AnonymousInnerClass39 : Db4objects.Db4o.Foundation.MappingIterator
		{
			public _AnonymousInnerClass39(IdListQueryResult _enclosing, System.Collections.IEnumerator
				 baseArg1) : base(baseArg1)
			{
				this._enclosing = _enclosing;
			}

			protected override object Map(object current)
			{
				lock (this._enclosing.StreamLock())
				{
					object obj = this._enclosing.ActivatedObject(((int)current));
					if (obj == null)
					{
						return Db4objects.Db4o.Foundation.MappingIterator.SKIP;
					}
					return obj;
				}
			}

			private readonly IdListQueryResult _enclosing;
		}

		public object Activate(object obj)
		{
			Db4objects.Db4o.YapStream stream = Stream();
			stream.Activate1(_transaction, obj, stream.ConfigImpl().ActivationDepth());
			return obj;
		}

		public object ActivatedObject(int id)
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
				return ActivatedObject(_ids.Get(index));
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
			_ids.Swap(left, right);
		}

		public virtual void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy index = clazz.Index();
			index.TraverseAll(_transaction, new _AnonymousInnerClass154(this));
		}

		private sealed class _AnonymousInnerClass154 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass154(IdListQueryResult _enclosing)
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
						index.TraverseAll(_transaction, new _AnonymousInnerClass177(this, duplicates));
					}
				}
			}
		}

		private sealed class _AnonymousInnerClass177 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass177(IdListQueryResult _enclosing, Db4objects.Db4o.Foundation.Tree[]
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

		public virtual void Add(int id)
		{
			_ids.Add(id);
		}

		public virtual int IndexOf(int id)
		{
			return _ids.IndexOf(id);
		}

		public virtual int Size()
		{
			return _ids.Size();
		}
	}
}
