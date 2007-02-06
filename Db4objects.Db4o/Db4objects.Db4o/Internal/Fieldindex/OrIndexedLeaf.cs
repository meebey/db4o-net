namespace Db4objects.Db4o.Internal.Fieldindex
{
	public class OrIndexedLeaf : Db4objects.Db4o.Internal.Fieldindex.JoinedLeaf
	{
		public OrIndexedLeaf(Db4objects.Db4o.Internal.Query.Processor.QCon constraint, Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange
			 leaf1, Db4objects.Db4o.Internal.Fieldindex.IIndexedNodeWithRange leaf2) : base(
			constraint, leaf1, leaf1.GetRange().Union(leaf2.GetRange()))
		{
		}
	}
}
