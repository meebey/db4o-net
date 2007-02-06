namespace Db4objects.Db4o.Internal.Fieldindex
{
	public interface IIndexedNodeWithRange : Db4objects.Db4o.Internal.Fieldindex.IIndexedNode
	{
		Db4objects.Db4o.Internal.Btree.IBTreeRange GetRange();
	}
}
