using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class FreespaceManagerForDebug : FreespaceManager
	{
		private readonly ISlotListener _listener;

		public FreespaceManagerForDebug(LocalObjectContainer file, ISlotListener listener
			) : base(file)
		{
			_listener = listener;
		}

		public override void BeginCommit()
		{
		}

		public override void Debug()
		{
		}

		public override void EndCommit()
		{
		}

		public override int EntryCount()
		{
			return 0;
		}

		public override void Free(int address, int length)
		{
			_listener.OnFree(new Slot(address, length));
		}

		public override void FreeSelf()
		{
		}

		public override int FreeSize()
		{
			return 0;
		}

		public override int GetSlot(int length)
		{
			return 0;
		}

		public override void Migrate(FreespaceManager newFM)
		{
		}

		public override void OnNew(LocalObjectContainer file)
		{
		}

		public override void Read(int freeSlotsID)
		{
		}

		public override void Start(int slotAddress)
		{
		}

		public override byte SystemType()
		{
			return FM_DEBUG;
		}

		public override int Shutdown()
		{
			return 0;
		}
	}
}
