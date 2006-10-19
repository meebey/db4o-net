namespace Db4objects.Db4o.Inside.Fieldindex
{
	public class AndIndexedLeaf : Db4objects.Db4o.Inside.Fieldindex.JoinedLeaf
	{
		public AndIndexedLeaf(Db4objects.Db4o.QCon constraint, Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange
			 leaf1, Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange leaf2) : base(constraint
			, leaf1, leaf1.GetRange().Intersect(leaf2.GetRange()))
		{
		}
	}
}
