/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Freespace;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Freespace;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public abstract class FreespaceManagerTestCaseBase : FileSizeTestCaseBase, IOptOutCS
	{
		protected IFreespaceManager[] fm;

		/// <exception cref="Exception"></exception>
		protected override void Db4oSetupAfterStore()
		{
			LocalObjectContainer container = (LocalObjectContainer)Db();
			BTreeFreespaceManager btreeFm = new BTreeFreespaceManager(container);
			btreeFm.Start(0);
			fm = new IFreespaceManager[] { new RamFreespaceManager(container), btreeFm };
		}

		protected virtual void Clear(IFreespaceManager freespaceManager)
		{
			Slot slot = null;
			do
			{
				slot = freespaceManager.GetSlot(1);
			}
			while (slot != null);
			Assert.AreEqual(0, freespaceManager.SlotCount());
			Assert.AreEqual(0, freespaceManager.TotalFreespace());
		}

		protected virtual void AssertSame(IFreespaceManager fm1, IFreespaceManager fm2)
		{
			Assert.AreEqual(fm1.SlotCount(), fm2.SlotCount());
			Assert.AreEqual(fm1.TotalFreespace(), fm2.TotalFreespace());
		}

		protected virtual void AssertSlot(Slot expected, Slot actual)
		{
			Assert.AreEqual(expected, actual);
		}

		protected virtual void ProduceSomeFreeSpace()
		{
			IFreespaceManager fm = CurrentFreespaceManager();
			int length = 300;
			Slot slot = Container().GetSlot(length);
			ByteArrayBuffer buffer = new ByteArrayBuffer(length);
			Container().WriteBytes(buffer, slot.Address(), 0);
			fm.Free(slot);
		}

		protected virtual IFreespaceManager CurrentFreespaceManager()
		{
			return Container().FreespaceManager();
		}

		public class Item
		{
			public int _int;
		}

		protected virtual void StoreSomeItems()
		{
			for (int i = 0; i < 3; i++)
			{
				Store(new FreespaceManagerTestCaseBase.Item());
			}
			Db().Commit();
		}

		protected virtual LocalObjectContainer Container()
		{
			return Fixture().FileSession();
		}
	}
}
