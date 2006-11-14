namespace Db4objects.Db4o.Inside.IX
{
	/// <summary>Index root holder for a field and a transaction.</summary>
	/// <remarks>Index root holder for a field and a transaction.</remarks>
	/// <exclude></exclude>
	public class IndexTransaction : Db4objects.Db4o.Foundation.IVisitor4
	{
		internal readonly Db4objects.Db4o.Inside.IX.Index4 i_index;

		internal readonly Db4objects.Db4o.Transaction i_trans;

		internal int i_version;

		private Db4objects.Db4o.Foundation.Tree i_root;

		internal IndexTransaction(Db4objects.Db4o.Transaction a_trans, Db4objects.Db4o.Inside.IX.Index4
			 a_index)
		{
			i_trans = a_trans;
			i_index = a_index;
		}

		public override bool Equals(object obj)
		{
			return i_trans == ((Db4objects.Db4o.Inside.IX.IndexTransaction)obj).i_trans;
		}

		public virtual void Add(int id, object value)
		{
			Patch(new Db4objects.Db4o.Inside.IX.IxAdd(this, id, value));
		}

		public virtual void Remove(int id, object value)
		{
			Patch(new Db4objects.Db4o.Inside.IX.IxRemove(this, id, value));
		}

		private void Patch(Db4objects.Db4o.Inside.IX.IxPatch patch)
		{
			i_root = Db4objects.Db4o.Foundation.Tree.Add(i_root, patch);
		}

		public virtual Db4objects.Db4o.Foundation.Tree GetRoot()
		{
			return i_root;
		}

		public virtual void Commit()
		{
			i_index.Commit(this);
		}

		public virtual void Rollback()
		{
			i_index.Rollback(this);
		}

		internal virtual void Merge(Db4objects.Db4o.Inside.IX.IndexTransaction a_ft)
		{
			Db4objects.Db4o.Foundation.Tree otherRoot = a_ft.GetRoot();
			if (otherRoot != null)
			{
				otherRoot.TraverseFromLeaves(this);
			}
		}

		/// <summary>
		/// Visitor functionality for merge:<br />
		/// Add
		/// </summary>
		public virtual void Visit(object obj)
		{
			if (obj is Db4objects.Db4o.Inside.IX.IxPatch)
			{
				Db4objects.Db4o.Inside.IX.IxPatch tree = (Db4objects.Db4o.Inside.IX.IxPatch)obj;
				if (tree.HasQueue())
				{
					Db4objects.Db4o.Foundation.Queue4 queue = tree.DetachQueue();
					while ((tree = (Db4objects.Db4o.Inside.IX.IxPatch)queue.Next()) != null)
					{
						tree.DetachQueue();
						AddPatchToRoot(tree);
					}
				}
				else
				{
					AddPatchToRoot(tree);
				}
			}
		}

		private void AddPatchToRoot(Db4objects.Db4o.Inside.IX.IxPatch tree)
		{
			if (tree._version != i_version)
			{
				tree.BeginMerge();
				tree.Handler().PrepareComparison(tree.Handler().ComparableObject(i_trans, tree._value
					));
				if (i_root == null)
				{
					i_root = tree;
				}
				else
				{
					i_root = i_root.Add(tree);
				}
			}
		}

		internal virtual int CountLeaves()
		{
			if (i_root == null)
			{
				return 0;
			}
			int[] leaves = { 0 };
			i_root.Traverse(new _AnonymousInnerClass102(this, leaves));
			return leaves[0];
		}

		private sealed class _AnonymousInnerClass102 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass102(IndexTransaction _enclosing, int[] leaves)
			{
				this._enclosing = _enclosing;
				this.leaves = leaves;
			}

			public void Visit(object a_object)
			{
				leaves[0]++;
			}

			private readonly IndexTransaction _enclosing;

			private readonly int[] leaves;
		}

		public virtual void SetRoot(Db4objects.Db4o.Foundation.Tree a_tree)
		{
			i_root = a_tree;
		}

		public override string ToString()
		{
			return base.ToString();
			System.Text.StringBuilder sb = new System.Text.StringBuilder();
			sb.Append("IxFieldTransaction ");
			sb.Append(Sharpen.Runtime.IdentityHashCode(this));
			if (i_root == null)
			{
				sb.Append("\n    Empty");
			}
			else
			{
				i_root.Traverse(new _AnonymousInnerClass124(this, sb));
			}
			return sb.ToString();
		}

		private sealed class _AnonymousInnerClass124 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass124(IndexTransaction _enclosing, System.Text.StringBuilder
				 sb)
			{
				this._enclosing = _enclosing;
				this.sb = sb;
			}

			public void Visit(object a_object)
			{
				sb.Append("\n");
				sb.Append(a_object.ToString());
			}

			private readonly IndexTransaction _enclosing;

			private readonly System.Text.StringBuilder sb;
		}
	}
}
