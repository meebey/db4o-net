using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Internal.IX
{
	/// <exclude></exclude>
	public interface IIndexable4 : IComparable4
	{
		object ComparableObject(Transaction trans, object indexEntry);

		int LinkLength();

		object ReadIndexEntry(Db4objects.Db4o.Internal.Buffer reader);

		void WriteIndexEntry(Db4objects.Db4o.Internal.Buffer writer, object obj);

		void DefragIndexEntry(ReaderPair readers);
	}
}
