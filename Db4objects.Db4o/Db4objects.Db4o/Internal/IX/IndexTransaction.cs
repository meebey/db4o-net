using System.Text;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.IX;
using Sharpen;

namespace Db4objects.Db4o.Internal.IX
{
	/// <summary>Index root holder for a field and a transaction.</summary>
	/// <remarks>Index root holder for a field and a transaction.</remarks>
	/// <exclude></exclude>
	public class IndexTransaction : IVisitor4
	{
		internal readonly Index4 i_index;

		internal readonly LocalTransaction i_trans;

		internal int i_version;

		private Tree i_root;

		internal IndexTransaction(LocalTransaction a_trans, Index4 a_index)
		{
			i_trans = a_trans;
			i_index = a_index;
		}

		/// <summary>Will raise an exception if argument class doesn't match this class - violates equals() contract in favor of failing fast.
		/// 	</summary>
		/// <remarks>Will raise an exception if argument class doesn't match this class - violates equals() contract in favor of failing fast.
		/// 	</remarks>
		public override bool Equals(object obj)
		{
			if (this == obj)
			{
				return true;
			}
			if (null == obj)
			{
				return false;
			}
			if (GetType() != obj.GetType())
			{
				Exceptions4.ShouldNeverHappen();
			}
			return i_trans == ((Db4objects.Db4o.Internal.IX.IndexTransaction)obj).i_trans;
		}

		public override int GetHashCode()
		{
			return i_trans.GetHashCode();
		}

		public virtual void Add(int id, object value)
		{
			Patch(new IxAdd(this, id, value));
		}

		public virtual void Remove(int id, object value)
		{
			Patch(new IxRemove(this, id, value));
		}

		private void Patch(IxPatch patch)
		{
			i_root = Tree.Add(i_root, patch);
		}

		public virtual Tree GetRoot()
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

		internal virtual void Merge(Db4objects.Db4o.Internal.IX.IndexTransaction a_ft)
		{
			Tree otherRoot = a_ft.GetRoot();
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
			if (obj is IxPatch)
			{
				IxPatch tree = (IxPatch)obj;
				if (tree.HasQueue())
				{
					IQueue4 queue = tree.DetachQueue();
					while ((tree = (IxPatch)queue.Next()) != null)
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

		private void AddPatchToRoot(IxPatch tree)
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
			int[] leaves = new int[] { 0 };
			i_root.Traverse(new _AnonymousInnerClass118(this, leaves));
			return leaves[0];
		}

		private sealed class _AnonymousInnerClass118 : IVisitor4
		{
			public _AnonymousInnerClass118(IndexTransaction _enclosing, int[] leaves)
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

		public virtual void SetRoot(Tree a_tree)
		{
			i_root = a_tree;
		}

		public override string ToString()
		{
			return base.ToString();
			StringBuilder sb = new StringBuilder();
			sb.Append("IxFieldTransaction ");
			sb.Append(Runtime.IdentityHashCode(this));
			if (i_root == null)
			{
				sb.Append("\n    Empty");
			}
			else
			{
				i_root.Traverse(new _AnonymousInnerClass140(this, sb));
			}
			return sb.ToString();
		}

		private sealed class _AnonymousInnerClass140 : IVisitor4
		{
			public _AnonymousInnerClass140(IndexTransaction _enclosing, StringBuilder sb)
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

			private readonly StringBuilder sb;
		}
	}
}
