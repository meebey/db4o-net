/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Freespace;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public class FreespaceManagerDiscardLimitTestCase : FreespaceManagerTestCaseBase
	{
		public static void Main(string[] args)
		{
			new FreespaceManagerDiscardLimitTestCase().RunSolo();
		}

		protected override void Configure(IConfiguration config)
		{
			config.Freespace().DiscardSmallerThan(10 * ((Config4Impl)config).BlockSize());
		}

		public virtual void TestGetSlot()
		{
			for (int i = 0; i < fm.Length; i++)
			{
				if (fm[i].SystemType() == AbstractFreespaceManager.FmIx)
				{
					continue;
				}
				fm[i].Free(new Slot(20, 15));
				Slot slot = fm[i].GetSlot(5);
				AssertSlot(new Slot(20, 5), slot);
				Assert.AreEqual(1, fm[i].SlotCount());
				fm[i].Free(slot);
				Assert.AreEqual(1, fm[i].SlotCount());
				slot = fm[i].GetSlot(6);
				AssertSlot(new Slot(20, 15), slot);
				Assert.AreEqual(0, fm[i].SlotCount());
				fm[i].Free(slot);
				Assert.AreEqual(1, fm[i].SlotCount());
				slot = fm[i].GetSlot(10);
				AssertSlot(new Slot(20, 15), slot);
				Assert.AreEqual(0, fm[i].SlotCount());
			}
		}
	}
}
