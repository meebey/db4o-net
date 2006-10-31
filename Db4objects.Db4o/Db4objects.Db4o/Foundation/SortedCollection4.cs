namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class SortedCollection4
	{
		private readonly Db4objects.Db4o.Foundation.IComparison4 _comparison;

		private Db4objects.Db4o.Foundation.Tree _tree;

		public SortedCollection4(Db4objects.Db4o.Foundation.IComparison4 comparison)
		{
			if (null == comparison)
			{
				throw new System.ArgumentNullException();
			}
			_comparison = comparison;
			_tree = null;
		}

		public virtual object SingleElement()
		{
			if (1 != Size())
			{
				throw new System.InvalidOperationException();
			}
			return _tree.Key();
		}

		public virtual void AddAll(System.Collections.IEnumerator iterator)
		{
			while (iterator.MoveNext())
			{
				Add(iterator.Current);
			}
		}

		public virtual void Add(object element)
		{
			_tree = Db4objects.Db4o.Foundation.Tree.Add(_tree, new Db4objects.Db4o.Foundation.TreeObject
				(element, _comparison));
		}

		public virtual void Remove(object element)
		{
			_tree = Db4objects.Db4o.Foundation.Tree.RemoveLike(_tree, new Db4objects.Db4o.Foundation.TreeObject
				(element, _comparison));
		}

		public virtual object[] ToArray(object[] array)
		{
			Db4objects.Db4o.Foundation.Tree.Traverse(_tree, new _AnonymousInnerClass43(this, 
				array));
			return array;
		}

		private sealed class _AnonymousInnerClass43 : Db4objects.Db4o.Foundation.IVisitor4
		{
			public _AnonymousInnerClass43(SortedCollection4 _enclosing, object[] array)
			{
				this._enclosing = _enclosing;
				this.array = array;
			}

			internal int i = 0;

			public void Visit(object obj)
			{
				array[this.i++] = ((Db4objects.Db4o.Foundation.TreeObject)obj).Key();
			}

			private readonly SortedCollection4 _enclosing;

			private readonly object[] array;
		}

		public virtual int Size()
		{
			return Db4objects.Db4o.Foundation.Tree.Size(_tree);
		}
	}
}
