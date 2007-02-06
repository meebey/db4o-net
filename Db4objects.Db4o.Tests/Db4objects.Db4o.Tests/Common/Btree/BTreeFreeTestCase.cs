namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class BTreeFreeTestCase : Db4objects.Db4o.Tests.Common.Btree.BTreeTestCaseBase
	{
		private static readonly int[] VALUES = new int[] { 1, 2, 5, 7, 8, 9, 12 };

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Btree.BTreeFreeTestCase().RunSolo();
		}

		public virtual void Test()
		{
			Add(VALUES);
			System.Collections.IEnumerator allSlotIDs = _btree.AllNodeIds(SystemTrans());
			Db4objects.Db4o.Foundation.Collection4 allSlots = new Db4objects.Db4o.Foundation.Collection4
				();
			while (allSlotIDs.MoveNext())
			{
				int slotID = (int)allSlotIDs.Current;
				Db4objects.Db4o.Internal.Slots.Slot slot = FileTransaction().GetCurrentSlotOfID(slotID
					);
				allSlots.Add(slot);
			}
			Db4objects.Db4o.Internal.LocalObjectContainer yapFile = (Db4objects.Db4o.Internal.LocalObjectContainer
				)Stream();
			Db4objects.Db4o.Foundation.Collection4 freedSlots = new Db4objects.Db4o.Foundation.Collection4
				();
			yapFile.InstallDebugFreespaceManager(new Db4objects.Db4o.Tests.Common.Btree.FreespaceManagerForDebug
				(yapFile, new _AnonymousInnerClass40(this, freedSlots)));
			_btree.Free(SystemTrans());
			SystemTrans().Commit();
			Db4oUnit.Assert.IsTrue(freedSlots.ContainsAll(allSlots.GetEnumerator()));
		}

		private sealed class _AnonymousInnerClass40 : Db4objects.Db4o.Tests.Common.Btree.ISlotListener
		{
			public _AnonymousInnerClass40(BTreeFreeTestCase _enclosing, Db4objects.Db4o.Foundation.Collection4
				 freedSlots)
			{
				this._enclosing = _enclosing;
				this.freedSlots = freedSlots;
			}

			public void OnFree(Db4objects.Db4o.Internal.Slots.Slot slot)
			{
				freedSlots.Add(slot);
			}

			private readonly BTreeFreeTestCase _enclosing;

			private readonly Db4objects.Db4o.Foundation.Collection4 freedSlots;
		}

		private Db4objects.Db4o.Internal.LocalTransaction FileTransaction()
		{
			return ((Db4objects.Db4o.Internal.LocalTransaction)Trans());
		}
	}
}
