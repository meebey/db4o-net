namespace Db4objects.Db4o.Inside.Query
{
	/// <exclude></exclude>
	public class IdListQueryResult : Db4objects.Db4o.Inside.Query.AbstractQueryResult
		, Db4objects.Db4o.Foundation.IVisitor4
	{
		private Db4objects.Db4o.Foundation.Tree _candidates;

		private bool _checkDuplicates;

		public Db4objects.Db4o.Foundation.IntArrayList _ids;

		public IdListQueryResult(Db4objects.Db4o.Transaction trans, int initialSize) : base
			(trans)
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
				return ActivatedObject(GetId(index));
			}
		}

		public override int GetId(int index)
		{
			if (index < 0 || index >= Size())
			{
				throw new System.IndexOutOfRangeException();
			}
			return _ids.Get(index);
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
			Db4objects.Db4o.Foundation.Algorithms4.Qsort(new _AnonymousInnerClass72(this, cmp
				));
		}

		private sealed class _AnonymousInnerClass72 : Db4objects.Db4o.Foundation.IQuickSortable4
		{
			public _AnonymousInnerClass72(IdListQueryResult _enclosing, Db4objects.Db4o.Query.IQueryComparator
				 cmp)
			{
				this._enclosing = _enclosing;
				this.cmp = cmp;
			}

			public void Swap(int leftIndex, int rightIndex)
			{
				this._enclosing._ids.Swap(leftIndex, rightIndex);
			}

			public int Size()
			{
				return this._enclosing.Size();
			}

			public int Compare(int leftIndex, int rightIndex)
			{
				return cmp.Compare(this._enclosing.Get(leftIndex), this._enclosing.Get(rightIndex
					));
			}

			private readonly IdListQueryResult _enclosing;

			private readonly Db4objects.Db4o.Query.IQueryComparator cmp;
		}

		public override void LoadFromClassIndex(Db4objects.Db4o.YapClass clazz)
		{
			Db4objects.Db4o.Inside.Classindex.IClassIndexStrategy index = clazz.Index();
			if (index is Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy)
			{
				Db4objects.Db4o.Inside.Btree.BTree btree = ((Db4objects.Db4o.Inside.Classindex.BTreeClassIndexStrategy
					)index).Btree();
				_ids = new Db4objects.Db4o.Foundation.IntArrayList(btree.Size(Transaction()));
			}
			index.TraverseAll(_transaction, new _AnonymousInnerClass91(this));
		}

		private sealed class _AnonymousInnerClass91 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass91(IdListQueryResult _enclosing)
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
			Db4objects.Db4o.Foundation.Tree.ByRef duplicates = new Db4objects.Db4o.Foundation.Tree.ByRef
				();
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
						index.TraverseAll(_transaction, new _AnonymousInnerClass114(this, duplicates));
					}
				}
			}
		}

		private sealed class _AnonymousInnerClass114 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass114(IdListQueryResult _enclosing, Db4objects.Db4o.Foundation.Tree.ByRef
				 duplicates)
			{
				this._enclosing = _enclosing;
				this.duplicates = duplicates;
			}

			public void Visit(object obj)
			{
				int id = ((int)obj);
				Db4objects.Db4o.TreeInt newNode = new Db4objects.Db4o.TreeInt(id);
				duplicates.value = Db4objects.Db4o.Foundation.Tree.Add(duplicates.value, newNode);
				if (newNode.Size() != 0)
				{
					this._enclosing.Add(id);
				}
			}

			private readonly IdListQueryResult _enclosing;

			private readonly Db4objects.Db4o.Foundation.Tree.ByRef duplicates;
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
