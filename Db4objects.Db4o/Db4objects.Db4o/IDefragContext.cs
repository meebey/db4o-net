namespace Db4objects.Db4o
{
	/// <summary>Encapsulates services involving source and target database files during defragmenting.
	/// 	</summary>
	/// <remarks>Encapsulates services involving source and target database files during defragmenting.
	/// 	</remarks>
	/// <exclude></exclude>
	public interface IDefragContext : Db4objects.Db4o.IIDMapping
	{
		Db4objects.Db4o.YapReader SourceReaderByAddress(int address, int length);

		Db4objects.Db4o.YapReader TargetReaderByAddress(int address, int length);

		Db4objects.Db4o.YapReader SourceReaderByID(int sourceID);

		int AllocateTargetSlot(int targetLength);

		void TargetWriteBytes(Db4objects.Db4o.YapReader targetPointerReader, int targetID
			);

		Db4objects.Db4o.Transaction SystemTrans();

		void TargetWriteBytes(Db4objects.Db4o.ReaderPair readers, int targetAddress);

		void TraverseAllIndexSlots(Db4objects.Db4o.Inside.Btree.BTree tree, Db4objects.Db4o.Foundation.IVisitor4
			 visitor4);

		Db4objects.Db4o.YapClass YapClass(int id);

		Db4objects.Db4o.YapWriter SourceWriterByID(int sourceID);

		int MappedID(int id, bool lenient);

		void RegisterSeen(int id);
	}
}
