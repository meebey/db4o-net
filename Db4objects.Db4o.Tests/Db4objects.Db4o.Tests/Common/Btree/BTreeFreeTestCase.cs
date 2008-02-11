/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Btree;

namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeFreeTestCase : BTreeTestCaseBase
	{
		private static readonly int[] Values = new int[] { 1, 2, 5, 7, 8, 9, 12 };

		public static void Main(string[] args)
		{
			new BTreeFreeTestCase().RunSolo();
		}

		public virtual void Test()
		{
			Add(Values);
			IEnumerator allSlotIDs = _btree.AllNodeIds(SystemTrans());
			Collection4 allSlots = new Collection4();
			while (allSlotIDs.MoveNext())
			{
				int slotID = ((int)allSlotIDs.Current);
				Slot slot = GetSlotForID(slotID);
				allSlots.Add(slot);
			}
			Slot bTreeSlot = GetSlotForID(_btree.GetID());
			allSlots.Add(bTreeSlot);
			LocalObjectContainer container = (LocalObjectContainer)Stream();
			Collection4 freedSlots = new Collection4();
			container.InstallDebugFreespaceManager(new FreespaceManagerForDebug(container, new 
				_ISlotListener_43(this, freedSlots, container)));
			_btree.Free(SystemTrans());
			SystemTrans().Commit();
			Assert.IsTrue(freedSlots.ContainsAll(allSlots.GetEnumerator()));
		}

		private sealed class _ISlotListener_43 : ISlotListener
		{
			public _ISlotListener_43(BTreeFreeTestCase _enclosing, Collection4 freedSlots, LocalObjectContainer
				 container)
			{
				this._enclosing = _enclosing;
				this.freedSlots = freedSlots;
				this.container = container;
			}

			public void OnFree(Slot slot)
			{
				freedSlots.Add(container.ToNonBlockedLength(slot));
			}

			private readonly BTreeFreeTestCase _enclosing;

			private readonly Collection4 freedSlots;

			private readonly LocalObjectContainer container;
		}

		private Slot GetSlotForID(int slotID)
		{
			return FileTransaction().GetCurrentSlotOfID(slotID);
		}

		private LocalTransaction FileTransaction()
		{
			return ((LocalTransaction)Trans());
		}
	}
}
