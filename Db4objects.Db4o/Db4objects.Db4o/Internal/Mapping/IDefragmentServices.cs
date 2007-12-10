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
		Db4objects.Db4o.Internal.Buffer SourceBufferByAddress(int address, int length);

		/// <exception cref="IOException"></exception>
		Db4objects.Db4o.Internal.Buffer TargetBufferByAddress(int address, int length);

		/// <exception cref="IOException"></exception>
		Db4objects.Db4o.Internal.Buffer SourceBufferByID(int sourceID);

		Slot AllocateTargetSlot(int targetLength);

		void TargetWriteBytes(Db4objects.Db4o.Internal.Buffer targetPointerReader, int targetID
			);

		Transaction SystemTrans();

		void TargetWriteBytes(BufferPair readers, int targetAddress);

		void TraverseAllIndexSlots(BTree tree, IVisitor4 visitor4);

		ClassMetadata YapClass(int id);

		int MappedID(int id, bool lenient);

		void RegisterUnindexed(int id);

		IEnumerator UnindexedIDs();

		int SourceAddressByID(int sourceID);
	}
}
