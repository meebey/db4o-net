namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class TreeKeyIterator : System.Collections.IEnumerator
	{
		private readonly Db4objects.Db4o.Foundation.Tree _tree;

		private Db4objects.Db4o.Foundation.Stack4 _stack;

		public TreeKeyIterator(Db4objects.Db4o.Foundation.Tree tree)
		{
			_tree = tree;
		}

		public virtual object Current
		{
			get
			{
				if (_stack == null)
				{
					throw new System.InvalidOperationException();
				}
				Db4objects.Db4o.Foundation.Tree tree = Peek();
				if (tree == null)
				{
					return null;
				}
				return tree.Key();
			}
		}

		private Db4objects.Db4o.Foundation.Tree Peek()
		{
			return (Db4objects.Db4o.Foundation.Tree)_stack.Peek();
		}

		public virtual void Reset()
		{
			_stack = null;
		}

		public virtual bool MoveNext()
		{
			if (_stack == null)
			{
				InitStack();
				return _stack != null;
			}
			Db4objects.Db4o.Foundation.Tree current = Peek();
			if (current == null)
			{
				return false;
			}
			if (PushPreceding(current._subsequent))
			{
				return true;
			}
			while (true)
			{
				_stack.Pop();
				Db4objects.Db4o.Foundation.Tree parent = Peek();
				if (parent == null)
				{
					return false;
				}
				if (current == parent._preceding)
				{
					return true;
				}
				current = parent;
			}
		}

		private void InitStack()
		{
			if (_tree == null)
			{
				return;
			}
			_stack = new Db4objects.Db4o.Foundation.Stack4();
			PushPreceding(_tree);
		}

		private bool PushPreceding(Db4objects.Db4o.Foundation.Tree node)
		{
			if (node == null)
			{
				return false;
			}
			while (node != null)
			{
				_stack.Push(node);
				node = node._preceding;
			}
			return true;
		}
	}
}
