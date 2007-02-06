namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class FreespaceManagerForDebug : Db4objects.Db4o.Internal.Freespace.FreespaceManager
	{
		private readonly Db4objects.Db4o.Tests.Common.Btree.ISlotListener _listener;

		public FreespaceManagerForDebug(Db4objects.Db4o.Internal.LocalObjectContainer file
			, Db4objects.Db4o.Tests.Common.Btree.ISlotListener listener) : base(file)
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
			_listener.OnFree(new Db4objects.Db4o.Internal.Slots.Slot(address, length));
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

		public override void Migrate(Db4objects.Db4o.Internal.Freespace.FreespaceManager 
			newFM)
		{
		}

		public override void OnNew(Db4objects.Db4o.Internal.LocalObjectContainer file)
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

		public override int Write(bool shuttingDown)
		{
			return 0;
		}
	}
}
