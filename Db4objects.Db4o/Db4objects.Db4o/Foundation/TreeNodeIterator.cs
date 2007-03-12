namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class TreeNodeIterator : Db4objects.Db4o.Foundation.AbstractTreeIterator
	{
		public TreeNodeIterator(Db4objects.Db4o.Foundation.Tree tree) : base(tree)
		{
		}

		protected override object CurrentValue(Db4objects.Db4o.Foundation.Tree tree)
		{
			return tree.Root();
		}
	}
}
