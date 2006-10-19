namespace Db4objects.Db4o.Inside.Fieldindex
{
	public interface IIndexedNode : System.Collections.IEnumerable
	{
		bool IsResolved();

		Db4objects.Db4o.Inside.Fieldindex.IIndexedNode Resolve();

		Db4objects.Db4o.Inside.Btree.BTree GetIndex();

		int ResultSize();

		Db4objects.Db4o.TreeInt ToTreeInt();
	}
}
