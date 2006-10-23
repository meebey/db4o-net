namespace Db4objects.Db4o.Inside.Freespace
{
	public abstract class FreespaceManager
	{
		internal readonly Db4objects.Db4o.YapFile _file;

		public const byte FM_DEFAULT = 0;

		public const byte FM_LEGACY_RAM = 1;

		public const byte FM_RAM = 2;

		public const byte FM_IX = 3;

		public const byte FM_DEBUG = 4;

		private const int INTS_IN_SLOT = 12;

		public FreespaceManager(Db4objects.Db4o.YapFile file)
		{
			_file = file;
		}

		public static byte CheckType(byte systemType)
		{
			if (systemType == FM_DEFAULT)
			{
				return FM_RAM;
			}
			return systemType;
		}

		public static Db4objects.Db4o.Inside.Freespace.FreespaceManager CreateNew(Db4objects.Db4o.YapFile
			 file)
		{
			return CreateNew(file, file.SystemData().FreespaceSystem());
		}

		public static Db4objects.Db4o.Inside.Freespace.FreespaceManager CreateNew(Db4objects.Db4o.YapFile
			 file, byte systemType)
		{
			systemType = CheckType(systemType);
			switch (systemType)
			{
				case FM_LEGACY_RAM:				case FM_RAM:
				{
					return new Db4objects.Db4o.Inside.Freespace.FreespaceManagerRam(file);
				}

				default:
				{
					return new Db4objects.Db4o.Inside.Freespace.FreespaceManagerIx(file);
					break;
				}
			}
		}

		public static int InitSlot(Db4objects.Db4o.YapFile file)
		{
			int address = file.GetSlot(SlotLength());
			SlotEntryToZeroes(file, address);
			return address;
		}

		internal static void SlotEntryToZeroes(Db4objects.Db4o.YapFile file, int address)
		{
			Db4objects.Db4o.YapWriter writer = new Db4objects.Db4o.YapWriter(file.GetSystemTransaction
				(), address, SlotLength());
			for (int i = 0; i < INTS_IN_SLOT; i++)
			{
				writer.WriteInt(0);
			}
			writer.WriteEncrypt();
		}

		internal static int SlotLength()
		{
			return Db4objects.Db4o.YapConst.INT_LENGTH * INTS_IN_SLOT;
		}

		public abstract void BeginCommit();

		internal int BlockSize()
		{
			return _file.BlockSize();
		}

		public abstract void Debug();

		internal int DiscardLimit()
		{
			return _file.ConfigImpl().DiscardFreeSpace();
		}

		public abstract void EndCommit();

		public abstract int EntryCount();

		public abstract void Free(int a_address, int a_length);

		public abstract int FreeSize();

		public abstract void FreeSelf();

		public abstract int GetSlot(int length);

		public abstract void Migrate(Db4objects.Db4o.Inside.Freespace.FreespaceManager newFM
			);

		public abstract void Read(int freeSlotsID);

		public abstract void Start(int slotAddress);

		public abstract byte SystemType();

		public abstract int Write(bool shuttingDown);

		public virtual bool RequiresMigration(byte configuredSystem, byte readSystem)
		{
			return (configuredSystem != 0 || readSystem == FM_LEGACY_RAM) && (SystemType() !=
				 configuredSystem);
		}

		public virtual Db4objects.Db4o.Inside.Freespace.FreespaceManager Migrate(Db4objects.Db4o.YapFile
			 file, byte toSystemType)
		{
			Db4objects.Db4o.Inside.Freespace.FreespaceManager newFM = CreateNew(file, toSystemType
				);
			newFM.Start(file.NewFreespaceSlot(toSystemType));
			Migrate(newFM);
			FreeSelf();
			newFM.BeginCommit();
			newFM.EndCommit();
			return newFM;
		}
	}
}
