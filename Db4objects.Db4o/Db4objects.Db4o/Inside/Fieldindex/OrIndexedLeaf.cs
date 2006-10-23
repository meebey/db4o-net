namespace Db4objects.Db4o.Inside.Fieldindex
{
	public class OrIndexedLeaf : Db4objects.Db4o.Inside.Fieldindex.JoinedLeaf
	{
		public OrIndexedLeaf(Db4objects.Db4o.QCon constraint, Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange
			 leaf1, Db4objects.Db4o.Inside.Fieldindex.IIndexedNodeWithRange leaf2) : base(constraint
			, leaf1, leaf1.GetRange().Union(leaf2.GetRange()))
		{
		}
	}
}
