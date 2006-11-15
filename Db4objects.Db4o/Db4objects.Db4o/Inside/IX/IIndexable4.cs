namespace Db4objects.Db4o.Inside.IX
{
	/// <exclude></exclude>
	public interface IIndexable4 : Db4objects.Db4o.IYapComparable
	{
		object ComparableObject(Db4objects.Db4o.Transaction trans, object indexEntry);

		int LinkLength();

		object ReadIndexEntry(Db4objects.Db4o.YapReader a_reader);

		void WriteIndexEntry(Db4objects.Db4o.YapReader a_writer, object a_object);

		void DefragIndexEntry(Db4objects.Db4o.ReaderPair readers);
	}
}
