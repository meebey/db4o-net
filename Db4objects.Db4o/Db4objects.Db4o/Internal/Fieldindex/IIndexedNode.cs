namespace Db4objects.Db4o.Internal.Fieldindex
{
	public interface IIndexedNode : System.Collections.IEnumerable
	{
		bool IsResolved();

		Db4objects.Db4o.Internal.Fieldindex.IIndexedNode Resolve();

		Db4objects.Db4o.Internal.Btree.BTree GetIndex();

		int ResultSize();

		Db4objects.Db4o.Internal.TreeInt ToTreeInt();
	}
}
