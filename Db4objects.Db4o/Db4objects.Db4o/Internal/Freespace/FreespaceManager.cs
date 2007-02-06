namespace Db4objects.Db4o.Internal.Freespace
{
	public abstract class FreespaceManager
	{
		internal readonly Db4objects.Db4o.Internal.LocalObjectContainer _file;

		public const byte FM_DEFAULT = 0;

		public const byte FM_LEGACY_RAM = 1;

		public const byte FM_RAM = 2;

		public const byte FM_IX = 3;

		public const byte FM_DEBUG = 4;

		private const int INTS_IN_SLOT = 12;

		public FreespaceManager(Db4objects.Db4o.Internal.LocalObjectContainer file)
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

		public static Db4objects.Db4o.Internal.Freespace.FreespaceManager CreateNew(Db4objects.Db4o.Internal.LocalObjectContainer
			 file)
		{
			return CreateNew(file, file.SystemData().FreespaceSystem());
		}

		public abstract void OnNew(Db4objects.Db4o.Internal.LocalObjectContainer file);

		public static Db4objects.Db4o.Internal.Freespace.FreespaceManager CreateNew(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, byte systemType)
		{
			systemType = CheckType(systemType);
			switch (systemType)
			{
				case FM_IX:
				{
					return new Db4objects.Db4o.Internal.Freespace.FreespaceManagerIx(file);
				}

				default:
				{
					return new Db4objects.Db4o.Internal.Freespace.FreespaceManagerRam(file);
					break;
				}
			}
		}

		public static int InitSlot(Db4objects.Db4o.Internal.LocalObjectContainer file)
		{
			int address = file.GetSlot(SlotLength());
			SlotEntryToZeroes(file, address);
			return address;
		}

		internal static void SlotEntryToZeroes(Db4objects.Db4o.Internal.LocalObjectContainer
			 file, int address)
		{
			Db4objects.Db4o.Internal.StatefulBuffer writer = new Db4objects.Db4o.Internal.StatefulBuffer
				(file.GetSystemTransaction(), address, SlotLength());
			for (int i = 0; i < INTS_IN_SLOT; i++)
			{
				writer.WriteInt(0);
			}
			writer.WriteEncrypt();
		}

		internal static int SlotLength()
		{
			return Db4objects.Db4o.Internal.Const4.INT_LENGTH * INTS_IN_SLOT;
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

		public abstract void Migrate(Db4objects.Db4o.Internal.Freespace.FreespaceManager 
			newFM);

		public abstract void Read(int freeSlotsID);

		public abstract void Start(int slotAddress);

		public abstract byte SystemType();

		public abstract int Write(bool shuttingDown);

		public virtual bool RequiresMigration(byte configuredSystem, byte readSystem)
		{
			return (configuredSystem != 0 || readSystem == FM_LEGACY_RAM) && (SystemType() !=
				 configuredSystem);
		}

		public static void Migrate(Db4objects.Db4o.Internal.Freespace.FreespaceManager oldFM
			, Db4objects.Db4o.Internal.Freespace.FreespaceManager newFM)
		{
			oldFM.Migrate(newFM);
			oldFM.FreeSelf();
			newFM.BeginCommit();
			newFM.EndCommit();
		}
	}
}
