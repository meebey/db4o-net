namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class TreeKeyIterator : Db4objects.Db4o.Foundation.AbstractTreeIterator
	{
		public TreeKeyIterator(Db4objects.Db4o.Foundation.Tree tree) : base(tree)
		{
		}

		protected override object CurrentValue(Db4objects.Db4o.Foundation.Tree tree)
		{
			return tree.Key();
		}
	}
}
