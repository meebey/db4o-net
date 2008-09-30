/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4oUnit.Extensions
{
	public class FreespaceManagerForDebug : AbstractFreespaceManager
	{
		private readonly ISlotListener _listener;

		public FreespaceManagerForDebug(LocalObjectContainer file, ISlotListener listener
			) : base(file)
		{
			_listener = listener;
		}

		public override Slot AllocateTransactionLogSlot(int length)
		{
			return null;
		}

		public override void FreeTransactionLogSlot(Slot slot)
		{
		}

		public override void BeginCommit()
		{
		}

		public override void Commit()
		{
		}

		public override void EndCommit()
		{
		}

		public override int SlotCount()
		{
			return 0;
		}

		public override void Free(Slot slot)
		{
			_listener.OnFree(slot);
		}

		public override void FreeSelf()
		{
		}

		public override Slot GetSlot(int length)
		{
			return null;
		}

		public override void Read(int freeSlotsID)
		{
		}

		public override void Start(int slotAddress)
		{
		}

		public override byte SystemType()
		{
			return FmDebug;
		}

		public override void Traverse(IVisitor4 visitor)
		{
		}

		public override int Write()
		{
			return 0;
		}
	}
}
