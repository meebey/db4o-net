using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Btree;
using Db4objects.Db4o.Internal.Mapping;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Mapping
{
	/// <summary>Encapsulates services involving source and target database files during defragmenting.
	/// 	</summary>
	/// <remarks>Encapsulates services involving source and target database files during defragmenting.
	/// 	</remarks>
	/// <exclude></exclude>
	public interface IDefragContext : IIDMapping
	{
		Db4objects.Db4o.Internal.Buffer SourceReaderByAddress(int address, int length);

		Db4objects.Db4o.Internal.Buffer TargetReaderByAddress(int address, int length);

		Db4objects.Db4o.Internal.Buffer SourceReaderByID(int sourceID);

		Slot AllocateTargetSlot(int targetLength);

		void TargetWriteBytes(Db4objects.Db4o.Internal.Buffer targetPointerReader, int targetID
			);

		Transaction SystemTrans();

		void TargetWriteBytes(ReaderPair readers, int targetAddress);

		void TraverseAllIndexSlots(BTree tree, IVisitor4 visitor4);

		ClassMetadata YapClass(int id);

		StatefulBuffer SourceWriterByID(int sourceID);

		int MappedID(int id, bool lenient);

		void RegisterUnindexed(int id);

		IEnumerator UnindexedIDs();
	}
}
