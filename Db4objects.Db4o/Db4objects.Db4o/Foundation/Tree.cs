namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public abstract class Tree : Db4objects.Db4o.Foundation.IShallowClone
	{
		public Db4objects.Db4o.Foundation.Tree _preceding;

		public int _size = 1;

		public Db4objects.Db4o.Foundation.Tree _subsequent;

		public static Db4objects.Db4o.Foundation.Tree Add(Db4objects.Db4o.Foundation.Tree
			 a_old, Db4objects.Db4o.Foundation.Tree a_new)
		{
			if (a_old == null)
			{
				return a_new;
			}
			return a_old.Add(a_new);
		}

		public virtual Db4objects.Db4o.Foundation.Tree Add(Db4objects.Db4o.Foundation.Tree
			 a_new)
		{
			return Add(a_new, Compare(a_new));
		}

		/// <summary>
		/// On adding a node to a tree, if it already exists, and if
		/// Tree#duplicates() returns false, #isDuplicateOf() will be
		/// called.
		/// </summary>
		/// <remarks>
		/// On adding a node to a tree, if it already exists, and if
		/// Tree#duplicates() returns false, #isDuplicateOf() will be
		/// called. The added node can then be asked for the node that
		/// prevails in the tree using #duplicateOrThis(). This mechanism
		/// allows doing find() and add() in one run.
		/// </remarks>
		public virtual Db4objects.Db4o.Foundation.Tree Add(Db4objects.Db4o.Foundation.Tree
			 a_new, int a_cmp)
		{
			if (a_cmp < 0)
			{
				if (_subsequent == null)
				{
					_subsequent = a_new;
					_size++;
				}
				else
				{
					_subsequent = _subsequent.Add(a_new);
					if (_preceding == null)
					{
						return RotateLeft();
					}
					return Balance();
				}
			}
			else
			{
				if (a_cmp > 0 || a_new.Duplicates())
				{
					if (_preceding == null)
					{
						_preceding = a_new;
						_size++;
					}
					else
					{
						_preceding = _preceding.Add(a_new);
						if (_subsequent == null)
						{
							return RotateRight();
						}
						return Balance();
					}
				}
				else
				{
					a_new.IsDuplicateOf(this);
				}
			}
			return this;
		}

		/// <summary>
		/// On adding a node to a tree, if it already exists, and if
		/// Tree#duplicates() returns false, #isDuplicateOf() will be
		/// called.
		/// </summary>
		/// <remarks>
		/// On adding a node to a tree, if it already exists, and if
		/// Tree#duplicates() returns false, #isDuplicateOf() will be
		/// called. The added node can then be asked for the node that
		/// prevails in the tree using #duplicateOrThis(). This mechanism
		/// allows doing find() and add() in one run.
		/// </remarks>
		public virtual Db4objects.Db4o.Foundation.Tree DuplicateOrThis()
		{
			if (_size == 0)
			{
				return _preceding;
			}
			return this;
		}

		public Db4objects.Db4o.Foundation.Tree Balance()
		{
			int cmp = _subsequent.Nodes() - _preceding.Nodes();
			if (cmp < -2)
			{
				return RotateRight();
			}
			else
			{
				if (cmp > 2)
				{
					return RotateLeft();
				}
				else
				{
					SetSizeOwnPrecedingSubsequent();
					return this;
				}
			}
		}

		public virtual Db4objects.Db4o.Foundation.Tree BalanceCheckNulls()
		{
			if (_subsequent == null)
			{
				if (_preceding == null)
				{
					SetSizeOwn();
					return this;
				}
				return RotateRight();
			}
			else
			{
				if (_preceding == null)
				{
					return RotateLeft();
				}
			}
			return Balance();
		}

		public virtual void CalculateSize()
		{
			if (_preceding == null)
			{
				if (_subsequent == null)
				{
					SetSizeOwn();
				}
				else
				{
					SetSizeOwnSubsequent();
				}
			}
			else
			{
				if (_subsequent == null)
				{
					SetSizeOwnPreceding();
				}
				else
				{
					SetSizeOwnPrecedingSubsequent();
				}
			}
		}

		/// <summary>
		/// returns 0, if keys are equal
		/// uses this - other
		/// returns positive if this is greater than a_to
		/// returns negative if this is smaller than a_to
		/// </summary>
		public abstract int Compare(Db4objects.Db4o.Foundation.Tree a_to);

		public static Db4objects.Db4o.Foundation.Tree DeepClone(Db4objects.Db4o.Foundation.Tree
			 a_tree, object a_param)
		{
			if (a_tree == null)
			{
				return null;
			}
			Db4objects.Db4o.Foundation.Tree newNode = a_tree.DeepClone(a_param);
			newNode._size = a_tree._size;
			newNode._preceding = Db4objects.Db4o.Foundation.Tree.DeepClone(a_tree._preceding, 
				a_param);
			newNode._subsequent = Db4objects.Db4o.Foundation.Tree.DeepClone(a_tree._subsequent
				, a_param);
			return newNode;
		}

		public virtual Db4objects.Db4o.Foundation.Tree DeepClone(object a_param)
		{
			return (Db4objects.Db4o.Foundation.Tree)this.ShallowClone();
		}

		public virtual bool Duplicates()
		{
			return true;
		}

		public Db4objects.Db4o.Foundation.Tree Filter(Db4objects.Db4o.Foundation.IPredicate4
			 a_filter)
		{
			if (_preceding != null)
			{
				_preceding = _preceding.Filter(a_filter);
			}
			if (_subsequent != null)
			{
				_subsequent = _subsequent.Filter(a_filter);
			}
			if (!a_filter.Match(this))
			{
				return Remove();
			}
			return this;
		}

		public static Db4objects.Db4o.Foundation.Tree Find(Db4objects.Db4o.Foundation.Tree
			 a_in, Db4objects.Db4o.Foundation.Tree a_tree)
		{
			if (a_in == null)
			{
				return null;
			}
			return a_in.Find(a_tree);
		}

		public Db4objects.Db4o.Foundation.Tree Find(Db4objects.Db4o.Foundation.Tree a_tree
			)
		{
			int cmp = Compare(a_tree);
			if (cmp < 0)
			{
				if (_subsequent != null)
				{
					return _subsequent.Find(a_tree);
				}
			}
			else
			{
				if (cmp > 0)
				{
					if (_preceding != null)
					{
						return _preceding.Find(a_tree);
					}
				}
				else
				{
					return this;
				}
			}
			return null;
		}

		public static Db4objects.Db4o.Foundation.Tree FindGreaterOrEqual(Db4objects.Db4o.Foundation.Tree
			 a_in, Db4objects.Db4o.Foundation.Tree a_finder)
		{
			if (a_in == null)
			{
				return null;
			}
			int cmp = a_in.Compare(a_finder);
			if (cmp == 0)
			{
				return a_in;
			}
			if (cmp > 0)
			{
				Db4objects.Db4o.Foundation.Tree node = FindGreaterOrEqual(a_in._preceding, a_finder
					);
				if (node != null)
				{
					return node;
				}
				return a_in;
			}
			return FindGreaterOrEqual(a_in._subsequent, a_finder);
		}

		public static Db4objects.Db4o.Foundation.Tree FindSmaller(Db4objects.Db4o.Foundation.Tree
			 a_in, Db4objects.Db4o.Foundation.Tree a_node)
		{
			if (a_in == null)
			{
				return null;
			}
			int cmp = a_in.Compare(a_node);
			if (cmp < 0)
			{
				Db4objects.Db4o.Foundation.Tree node = FindSmaller(a_in._subsequent, a_node);
				if (node != null)
				{
					return node;
				}
				return a_in;
			}
			return FindSmaller(a_in._preceding, a_node);
		}

		public Db4objects.Db4o.Foundation.Tree First()
		{
			if (_preceding == null)
			{
				return this;
			}
			return _preceding.First();
		}

		public virtual void IsDuplicateOf(Db4objects.Db4o.Foundation.Tree a_tree)
		{
			_size = 0;
			_preceding = a_tree;
		}

		/// <returns>the number of nodes in this tree for balancing</returns>
		public virtual int Nodes()
		{
			return _size;
		}

		public virtual int OwnSize()
		{
			return 1;
		}

		public virtual Db4objects.Db4o.Foundation.Tree Remove()
		{
			if (_subsequent != null && _preceding != null)
			{
				_subsequent = _subsequent.RotateSmallestUp();
				_subsequent._preceding = _preceding;
				_subsequent.CalculateSize();
				return _subsequent;
			}
			if (_subsequent != null)
			{
				return _subsequent;
			}
			return _preceding;
		}

		public virtual void RemoveChildren()
		{
			_preceding = null;
			_subsequent = null;
			SetSizeOwn();
		}

		public virtual Db4objects.Db4o.Foundation.Tree RemoveFirst()
		{
			if (_preceding == null)
			{
				return _subsequent;
			}
			_preceding = _preceding.RemoveFirst();
			CalculateSize();
			return this;
		}

		public static Db4objects.Db4o.Foundation.Tree RemoveLike(Db4objects.Db4o.Foundation.Tree
			 from, Db4objects.Db4o.Foundation.Tree a_find)
		{
			if (from == null)
			{
				return null;
			}
			return from.RemoveLike(a_find);
		}

		public Db4objects.Db4o.Foundation.Tree RemoveLike(Db4objects.Db4o.Foundation.Tree
			 a_find)
		{
			int cmp = Compare(a_find);
			if (cmp == 0)
			{
				return Remove();
			}
			if (cmp > 0)
			{
				if (_preceding != null)
				{
					_preceding = _preceding.RemoveLike(a_find);
				}
			}
			else
			{
				if (_subsequent != null)
				{
					_subsequent = _subsequent.RemoveLike(a_find);
				}
			}
			CalculateSize();
			return this;
		}

		public Db4objects.Db4o.Foundation.Tree RemoveNode(Db4objects.Db4o.Foundation.Tree
			 a_tree)
		{
			if (this == a_tree)
			{
				return Remove();
			}
			int cmp = Compare(a_tree);
			if (cmp >= 0)
			{
				if (_preceding != null)
				{
					_preceding = _preceding.RemoveNode(a_tree);
				}
			}
			if (cmp <= 0)
			{
				if (_subsequent != null)
				{
					_subsequent = _subsequent.RemoveNode(a_tree);
				}
			}
			CalculateSize();
			return this;
		}

		public Db4objects.Db4o.Foundation.Tree RotateLeft()
		{
			Db4objects.Db4o.Foundation.Tree tree = _subsequent;
			_subsequent = tree._preceding;
			CalculateSize();
			tree._preceding = this;
			if (tree._subsequent == null)
			{
				tree.SetSizeOwnPlus(this);
			}
			else
			{
				tree.SetSizeOwnPlus(this, tree._subsequent);
			}
			return tree;
		}

		public Db4objects.Db4o.Foundation.Tree RotateRight()
		{
			Db4objects.Db4o.Foundation.Tree tree = _preceding;
			_preceding = tree._subsequent;
			CalculateSize();
			tree._subsequent = this;
			if (tree._preceding == null)
			{
				tree.SetSizeOwnPlus(this);
			}
			else
			{
				tree.SetSizeOwnPlus(this, tree._preceding);
			}
			return tree;
		}

		private Db4objects.Db4o.Foundation.Tree RotateSmallestUp()
		{
			if (_preceding != null)
			{
				_preceding = _preceding.RotateSmallestUp();
				return RotateRight();
			}
			return this;
		}

		public virtual void SetSizeOwn()
		{
			_size = OwnSize();
		}

		public virtual void SetSizeOwnPrecedingSubsequent()
		{
			_size = OwnSize() + _preceding._size + _subsequent._size;
		}

		public virtual void SetSizeOwnPreceding()
		{
			_size = OwnSize() + _preceding._size;
		}

		public virtual void SetSizeOwnSubsequent()
		{
			_size = OwnSize() + _subsequent._size;
		}

		public virtual void SetSizeOwnPlus(Db4objects.Db4o.Foundation.Tree tree)
		{
			_size = OwnSize() + tree._size;
		}

		public virtual void SetSizeOwnPlus(Db4objects.Db4o.Foundation.Tree tree1, Db4objects.Db4o.Foundation.Tree
			 tree2)
		{
			_size = OwnSize() + tree1._size + tree2._size;
		}

		public static int Size(Db4objects.Db4o.Foundation.Tree a_tree)
		{
			if (a_tree == null)
			{
				return 0;
			}
			return a_tree.Size();
		}

		/// <returns>the number of objects represented.</returns>
		public virtual int Size()
		{
			return _size;
		}

		public static void Traverse(Db4objects.Db4o.Foundation.Tree tree, Db4objects.Db4o.Foundation.IVisitor4
			 visitor)
		{
			if (tree == null)
			{
				return;
			}
			tree.Traverse(visitor);
		}

		public void Traverse(Db4objects.Db4o.Foundation.IVisitor4 a_visitor)
		{
			if (_preceding != null)
			{
				_preceding.Traverse(a_visitor);
			}
			a_visitor.Visit(this);
			if (_subsequent != null)
			{
				_subsequent.Traverse(a_visitor);
			}
		}

		public void TraverseFromLeaves(Db4objects.Db4o.Foundation.IVisitor4 a_visitor)
		{
			if (_preceding != null)
			{
				_preceding.TraverseFromLeaves(a_visitor);
			}
			if (_subsequent != null)
			{
				_subsequent.TraverseFromLeaves(a_visitor);
			}
			a_visitor.Visit(this);
		}

		protected virtual Db4objects.Db4o.Foundation.Tree ShallowCloneInternal(Db4objects.Db4o.Foundation.Tree
			 tree)
		{
			tree._preceding = _preceding;
			tree._size = _size;
			tree._subsequent = _subsequent;
			return tree;
		}

		public virtual object ShallowClone()
		{
			throw new System.NotImplementedException();
		}

		public abstract object Key();
	}
}
