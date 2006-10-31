namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class IdListQueryResult : Db4objects.Db4o.Inside.Query.AbstractQueryResult
		, Db4objects.Db4o.Foundation.IVisitor4
	{
		private Db4objects.Db4o.Foundation.Tree _candidates;

		private bool _checkDuplicates;

		private readonly Db4objects.Db4o.Foundation.IntArrayList _ids;

		public IdListQueryResult(Db4objects.Db4o.Transaction trans, int initialSize) : 
			base(trans)
		{
			_ids = new Db4objects.Db4o.Foundation.IntArrayList(initialSize);
		}

		public IdListQueryResult(Db4objects.Db4o.Transaction trans) : this(trans, 0)
		{
		}

		public override Db4objects.Db4o.Foundation.IIntIterator4 IterateIDs()
		{
			return _ids.IntIterator();
		}

		public override object Get(int index)
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

		public override void Sort(Db4objects.Db4o.Query.IQueryComparator cmp)
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

		public override void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy index = clazz.Index();
			index.TraverseAll(_transaction, new _AnonymousInnerClass105(this));
		}

		private sealed class _AnonymousInnerClass105 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass105(IdListQueryResult _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Visit(object a_object)
			{
				this._enclosing.Add(((int)a_object));
			}

			private readonly IdListQueryResult _enclosing;
		}

		public override void LoadFromQuery(Db4objects.Db4o.QQuery query)
		{
			query.ExecuteLocal(this);
		}

		public override void LoadFromClassIndexes(Db4objects.Db4o.YapClassCollectionIterator
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
						index.TraverseAll(_transaction, new _AnonymousInnerClass128(this, duplicates));
					}
				}
			}
		}

		private sealed class _AnonymousInnerClass128 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass128(IdListQueryResult _enclosing, Db4objects.Db4o.Foundation.Tree[]
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

		public override void LoadFromIdReader(Db4objects.Db4o.YapReader reader)
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

		public override int IndexOf(int id)
		{
			return _ids.IndexOf(id);
		}

		public override int Size()
		{
			return _ids.Size();
		}
	}
}
