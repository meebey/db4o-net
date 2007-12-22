/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;

namespace Db4objects.Db4o.Internal.Freespace
{
	public abstract class AbstractFreespaceManager : IFreespaceManager
	{
		internal readonly LocalObjectContainer _file;

		public const byte FM_DEBUG = 127;

		public const byte FM_DEFAULT = 0;

		public const byte FM_LEGACY_RAM = 1;

		public const byte FM_RAM = 2;

		public const byte FM_IX = 3;

		public const byte FM_BTREE = 4;

		private const int INTS_IN_SLOT = 12;

		public AbstractFreespaceManager(LocalObjectContainer file)
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

		public static Db4objects.Db4o.Internal.Freespace.AbstractFreespaceManager CreateNew
			(LocalObjectContainer file)
		{
			return CreateNew(file, file.SystemData().FreespaceSystem());
		}

		public abstract int OnNew(LocalObjectContainer file);

		public static Db4objects.Db4o.Internal.Freespace.AbstractFreespaceManager CreateNew
			(LocalObjectContainer file, byte systemType)
		{
			systemType = CheckType(systemType);
			switch (systemType)
			{
				case FM_IX:
				{
					return new FreespaceManagerIx(file);
				}

				case FM_BTREE:
				{
					return new BTreeFreespaceManager(file);
				}

				default:
				{
					return new RamFreespaceManager(file);
					break;
				}
			}
		}

		public static int InitSlot(LocalObjectContainer file)
		{
			int address = file.GetSlot(SlotLength()).Address();
			SlotEntryToZeroes(file, address);
			return address;
		}

		public virtual void MigrateTo(IFreespaceManager fm)
		{
			Traverse(new _IVisitor4_59(this, fm));
		}

		private sealed class _IVisitor4_59 : IVisitor4
		{
			public _IVisitor4_59(AbstractFreespaceManager _enclosing, IFreespaceManager fm)
			{
				this._enclosing = _enclosing;
				this.fm = fm;
			}

			public void Visit(object obj)
			{
				fm.Free((Slot)obj);
			}

			private readonly AbstractFreespaceManager _enclosing;

			private readonly IFreespaceManager fm;
		}

		internal static void SlotEntryToZeroes(LocalObjectContainer file, int address)
		{
			StatefulBuffer writer = new StatefulBuffer(file.SystemTransaction(), address, SlotLength
				());
			for (int i = 0; i < INTS_IN_SLOT; i++)
			{
				writer.WriteInt(0);
			}
			writer.WriteEncrypt();
		}

		internal static int SlotLength()
		{
			return Const4.INT_LENGTH * INTS_IN_SLOT;
		}

		public virtual int TotalFreespace()
		{
			IntByRef mint = new IntByRef();
			Traverse(new _IVisitor4_84(this, mint));
			return mint.value;
		}

		private sealed class _IVisitor4_84 : IVisitor4
		{
			public _IVisitor4_84(AbstractFreespaceManager _enclosing, IntByRef mint)
			{
				this._enclosing = _enclosing;
				this.mint = mint;
			}

			public void Visit(object obj)
			{
				Slot slot = (Slot)obj;
				mint.value += slot.Length();
			}

			private readonly AbstractFreespaceManager _enclosing;

			private readonly IntByRef mint;
		}

		public abstract void BeginCommit();

		protected int BlockedDiscardLimit()
		{
			return _file.BlocksToBytes(DiscardLimit());
		}

		protected virtual int DiscardLimit()
		{
			return _file.ConfigImpl().DiscardFreeSpace();
		}

		internal bool CanDiscard(int blocks)
		{
			return blocks == 0 || blocks < BlockedDiscardLimit();
		}

		public static void Migrate(IFreespaceManager oldFM, IFreespaceManager newFM)
		{
			oldFM.MigrateTo(newFM);
			oldFM.FreeSelf();
			newFM.BeginCommit();
			newFM.EndCommit();
		}

		public virtual void DebugCheckIntegrity()
		{
			IntByRef lastStart = new IntByRef();
			IntByRef lastEnd = new IntByRef();
			Traverse(new _IVisitor4_117(this, lastEnd, lastStart));
		}

		private sealed class _IVisitor4_117 : IVisitor4
		{
			public _IVisitor4_117(AbstractFreespaceManager _enclosing, IntByRef lastEnd, IntByRef
				 lastStart)
			{
				this._enclosing = _enclosing;
				this.lastEnd = lastEnd;
				this.lastStart = lastStart;
			}

			public void Visit(object obj)
			{
				Slot slot = (Slot)obj;
				if (slot.Address() <= lastEnd.value)
				{
					throw new InvalidOperationException();
				}
				lastStart.value = slot.Address();
				lastEnd.value = slot.Address() + slot.Length();
			}

			private readonly AbstractFreespaceManager _enclosing;

			private readonly IntByRef lastEnd;

			private readonly IntByRef lastStart;
		}

		protected LocalTransaction Transaction()
		{
			return (LocalTransaction)_file.SystemTransaction();
		}

		public static bool MigrationRequired(byte systemType)
		{
			return systemType == FM_LEGACY_RAM || systemType == FM_IX;
		}

		public abstract Slot AllocateTransactionLogSlot(int arg1);

		public abstract void Commit();

		public abstract void EndCommit();

		public abstract void Free(Slot arg1);

		public abstract void FreeSelf();

		public abstract void FreeTransactionLogSlot(Slot arg1);

		public abstract Slot GetSlot(int arg1);

		public abstract void Read(int arg1);

		public abstract int SlotCount();

		public abstract void Start(int arg1);

		public abstract byte SystemType();

		public abstract void Traverse(IVisitor4 arg1);

		public abstract int Write();
	}
}
