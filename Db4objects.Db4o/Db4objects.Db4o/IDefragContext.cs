namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public interface IDefragContext : Db4objects.Db4o.IIDMapping
	{
		Db4objects.Db4o.YapReader SourceReaderByID(int sourceID);

		int AllocateTargetSlot(int targetLength);

		void TargetWriteBytes(Db4objects.Db4o.YapReader targetPointerReader, int targetID
			);

		Db4objects.Db4o.Transaction SystemTrans();

		void TargetWriteBytes(Db4objects.Db4o.ReaderPair readers, int targetAddress);

		void TraverseAllIndexSlots(Db4objects.Db4o.Inside.Btree.BTree tree, Db4objects.Db4o.Foundation.IVisitor4
			 visitor4);
	}
}
