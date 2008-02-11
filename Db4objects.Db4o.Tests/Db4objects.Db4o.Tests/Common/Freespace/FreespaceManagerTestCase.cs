/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Freespace;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public class FreespaceManagerTestCase : FreespaceManagerTestCaseBase
	{
		public static void Main(string[] args)
		{
			new FreespaceManagerTestCase().RunSolo();
		}

		public virtual void TestAllocateTransactionLogSlot()
		{
			for (int i = 0; i < fm.Length; i++)
			{
				if (fm[i].SystemType() == AbstractFreespaceManager.FmRam)
				{
					Slot slot = fm[i].AllocateTransactionLogSlot(1);
					Assert.IsNull(slot);
					fm[i].Free(new Slot(5, 10));
					fm[i].Free(new Slot(100, 5));
					fm[i].Free(new Slot(140, 27));
					slot = fm[i].AllocateTransactionLogSlot(28);
					Assert.IsNull(slot);
					Assert.AreEqual(3, fm[i].SlotCount());
					slot = fm[i].AllocateTransactionLogSlot(27);
					Assert.AreEqual(2, fm[i].SlotCount());
					Assert.AreEqual(new Slot(140, 27), slot);
				}
			}
		}

		public virtual void TestConstructor()
		{
			for (int i = 0; i < fm.Length; i++)
			{
				Assert.AreEqual(0, fm[i].SlotCount());
				Assert.AreEqual(0, fm[i].TotalFreespace());
			}
		}

		public virtual void TestFree()
		{
			for (int i = 0; i < fm.Length; i++)
			{
				int count = fm[i].SlotCount();
				fm[i].Free(new Slot(1000, 1));
				Assert.AreEqual(count + 1, fm[i].SlotCount());
			}
		}

		public virtual void TestGetSlot()
		{
			for (int i = 0; i < fm.Length; i++)
			{
				Slot slot = fm[i].GetSlot(1);
				Assert.IsNull(slot);
				Assert.AreEqual(0, fm[i].SlotCount());
				fm[i].Free(new Slot(10, 1));
				slot = fm[i].GetSlot(1);
				Assert.AreEqual(slot.Address(), 10);
				Assert.AreEqual(0, fm[i].SlotCount());
				slot = fm[i].GetSlot(1);
				Assert.IsNull(slot);
				fm[i].Free(new Slot(10, 1));
				fm[i].Free(new Slot(20, 2));
				slot = fm[i].GetSlot(1);
				Assert.AreEqual(1, fm[i].SlotCount());
				Assert.AreEqual(slot.Address(), 10);
				slot = fm[i].GetSlot(3);
				Assert.IsNull(slot);
				slot = fm[i].GetSlot(1);
				Assert.IsNotNull(slot);
				Assert.AreEqual(1, fm[i].SlotCount());
			}
		}

		public virtual void TestMerging()
		{
			for (int i = 0; i < fm.Length; i++)
			{
				fm[i].Free(new Slot(5, 5));
				fm[i].Free(new Slot(15, 5));
				fm[i].Free(new Slot(10, 5));
				Assert.AreEqual(1, fm[i].SlotCount());
			}
		}

		public virtual void TestTotalFreeSpace()
		{
			for (int i = 0; i < fm.Length; i++)
			{
				fm[i].Free(new Slot(5, 10));
				fm[i].Free(new Slot(100, 5));
				fm[i].Free(new Slot(140, 27));
				Assert.AreEqual(42, fm[i].TotalFreespace());
				fm[i].GetSlot(8);
				Assert.AreEqual(34, fm[i].TotalFreespace());
				fm[i].GetSlot(6);
				Assert.AreEqual(28, fm[i].TotalFreespace());
				fm[i].Free(new Slot(120, 14));
				Assert.AreEqual(42, fm[i].TotalFreespace());
			}
		}

		public virtual void TestMigrateTo()
		{
			for (int from = 0; from < fm.Length; from++)
			{
				for (int to = 0; to < fm.Length; to++)
				{
					if (to != from)
					{
						Clear(fm[from]);
						Clear(fm[to]);
						AbstractFreespaceManager.Migrate(fm[from], fm[to]);
						AssertSame(fm[from], fm[to]);
						fm[from].Free(new Slot(5, 10));
						fm[from].Free(new Slot(100, 5));
						fm[from].Free(new Slot(140, 27));
						AbstractFreespaceManager.Migrate(fm[from], fm[to]);
						AssertSame(fm[from], fm[to]);
					}
				}
			}
		}
	}
}
