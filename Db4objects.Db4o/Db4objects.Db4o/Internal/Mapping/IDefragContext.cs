namespace Db4objects.Db4o.Internal.Mapping
{
	/// <summary>Encapsulates services involving source and target database files during defragmenting.
	/// 	</summary>
	/// <remarks>Encapsulates services involving source and target database files during defragmenting.
	/// 	</remarks>
	/// <exclude></exclude>
	public interface IDefragContext : Db4objects.Db4o.Internal.Mapping.IIDMapping
	{
		Db4objects.Db4o.Internal.Buffer SourceReaderByAddress(int address, int length);

		Db4objects.Db4o.Internal.Buffer TargetReaderByAddress(int address, int length);

		Db4objects.Db4o.Internal.Buffer SourceReaderByID(int sourceID);

		int AllocateTargetSlot(int targetLength);

		void TargetWriteBytes(Db4objects.Db4o.Internal.Buffer targetPointerReader, int targetID
			);

		Db4objects.Db4o.Internal.Transaction SystemTrans();

		void TargetWriteBytes(Db4objects.Db4o.Internal.ReaderPair readers, int targetAddress
			);

		void TraverseAllIndexSlots(Db4objects.Db4o.Internal.Btree.BTree tree, Db4objects.Db4o.Foundation.IVisitor4
			 visitor4);

		Db4objects.Db4o.Internal.ClassMetadata YapClass(int id);

		Db4objects.Db4o.Internal.StatefulBuffer SourceWriterByID(int sourceID);

		int MappedID(int id, bool lenient);

		void RegisterUnindexed(int id);

		System.Collections.IEnumerator UnindexedIDs();
	}
}
