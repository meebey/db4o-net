namespace Db4objects.Db4o.Internal.Classindex
{
	/// <exclude></exclude>
	public interface IClassIndexStrategy
	{
		void Initialize(Db4objects.Db4o.Internal.ObjectContainerBase stream);

		void Read(Db4objects.Db4o.Internal.ObjectContainerBase stream, int indexID);

		int Write(Db4objects.Db4o.Internal.Transaction transaction);

		void Add(Db4objects.Db4o.Internal.Transaction transaction, int id);

		void Remove(Db4objects.Db4o.Internal.Transaction transaction, int id);

		int EntryCount(Db4objects.Db4o.Internal.Transaction transaction);

		int OwnLength();

		void Purge();

		/// <summary>Traverses all index entries (java.lang.Integer references).</summary>
		/// <remarks>Traverses all index entries (java.lang.Integer references).</remarks>
		void TraverseAll(Db4objects.Db4o.Internal.Transaction transaction, Db4objects.Db4o.Foundation.IVisitor4
			 command);

		void DontDelete(Db4objects.Db4o.Internal.Transaction transaction, int id);

		System.Collections.IEnumerator AllSlotIDs(Db4objects.Db4o.Internal.Transaction trans
			);

		void DefragReference(Db4objects.Db4o.Internal.ClassMetadata yapClass, Db4objects.Db4o.Internal.ReaderPair
			 readers, int classIndexID);

		int Id();

		void DefragIndex(Db4objects.Db4o.Internal.ReaderPair readers);
	}
}
