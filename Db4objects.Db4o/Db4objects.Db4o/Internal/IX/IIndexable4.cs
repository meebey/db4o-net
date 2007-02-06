namespace Db4objects.Db4o.Internal.IX
{
	/// <exclude></exclude>
	public interface IIndexable4 : Db4objects.Db4o.Internal.IComparable4
	{
		object ComparableObject(Db4objects.Db4o.Internal.Transaction trans, object indexEntry
			);

		int LinkLength();

		object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer a_reader);

		void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer a_writer, object a_object);

		void DefragIndexEntry(Db4objects.Db4o.Internal.ReaderPair readers);
	}
}
