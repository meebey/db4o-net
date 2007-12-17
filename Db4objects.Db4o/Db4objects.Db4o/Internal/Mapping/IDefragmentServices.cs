/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
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
	public interface IDefragmentServices : IIDMapping
	{
		/// <exception cref="IOException"></exception>
		BufferImpl SourceBufferByAddress(int address, int length);

		/// <exception cref="IOException"></exception>
		BufferImpl TargetBufferByAddress(int address, int length);

		/// <exception cref="IOException"></exception>
		BufferImpl SourceBufferByID(int sourceID);

		Slot AllocateTargetSlot(int targetLength);

		void TargetWriteBytes(BufferImpl targetPointerReader, int targetID);

		Transaction SystemTrans();

		void TargetWriteBytes(DefragmentContextImpl context, int targetAddress);

		void TraverseAllIndexSlots(BTree tree, IVisitor4 visitor4);

		ClassMetadata ClassMetadataForId(int id);

		int MappedID(int id, bool lenient);

		void RegisterUnindexed(int id);

		IEnumerator UnindexedIDs();

		int SourceAddressByID(int sourceID);
	}
}
