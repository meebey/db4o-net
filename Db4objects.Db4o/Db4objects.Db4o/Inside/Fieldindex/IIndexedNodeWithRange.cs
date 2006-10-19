namespace Db4objects.Db4o.Inside.Fieldindex
{
	public interface IIndexedNodeWithRange : Db4objects.Db4o.Inside.Fieldindex.IIndexedNode
	{
		Db4objects.Db4o.Inside.Btree.IBTreeRange GetRange();
	}
}
