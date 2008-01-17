/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	/// <exclude></exclude>
	public interface IFreespaceManager
	{
		void BeginCommit();

		void EndCommit();

		int SlotCount();

		void Free(Slot slot);

		void FreeSelf();

		int TotalFreespace();

		Slot GetSlot(int length);

		void MigrateTo(IFreespaceManager fm);

		void Read(int freeSpaceID);

		void Start(int slotAddress);

		byte SystemType();

		void Traverse(IVisitor4 visitor);

		int Write();

		void Commit();

		Slot AllocateTransactionLogSlot(int length);

		void FreeTransactionLogSlot(Slot slot);
	}
}
