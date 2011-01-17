/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Consistency;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Slots;
using Db4objects.Db4o.Tests.Common.Api;
using Db4objects.Db4o.Tests.Optional;

namespace Db4objects.Db4o.Tests.Optional
{
	public class ConsistencyCheckerTestCase : Db4oTestWithTempFile
	{
		public class Item
		{
		}

		public virtual void TestFreeUsedSlot()
		{
			AssertInconsistencyDetected(new _IProcedure4_20());
		}

		private sealed class _IProcedure4_20 : IProcedure4
		{
			public _IProcedure4_20()
			{
			}

			public void Apply(object state)
			{
				ConsistencyCheckerTestCase.Item item = ((ConsistencyCheckerTestCase.Item)((Pair)state
					).second);
				LocalObjectContainer db = ((LocalObjectContainer)((Pair)state).first);
				int id = (int)db.GetID(item);
				Slot slot = db.IdSystem().CommittedSlot(id);
				db.FreespaceManager().Free(slot);
			}
		}

		public virtual void TestFreeShiftedUsedSlot()
		{
			AssertInconsistencyDetected(new _IProcedure4_32());
		}

		private sealed class _IProcedure4_32 : IProcedure4
		{
			public _IProcedure4_32()
			{
			}

			public void Apply(object state)
			{
				ConsistencyCheckerTestCase.Item item = ((ConsistencyCheckerTestCase.Item)((Pair)state
					).second);
				LocalObjectContainer db = ((LocalObjectContainer)((Pair)state).first);
				int id = (int)db.GetID(item);
				Slot slot = db.IdSystem().CommittedSlot(id);
				db.FreespaceManager().Free(new Slot(slot.Address() + 1, slot.Length()));
			}
		}

		private void AssertInconsistencyDetected(IProcedure4 proc)
		{
			IEmbeddedConfiguration config = NewConfiguration();
			LocalObjectContainer db = (LocalObjectContainer)Db4oEmbedded.OpenFile(config, TempFile
				());
			try
			{
				ConsistencyCheckerTestCase.Item item = new ConsistencyCheckerTestCase.Item();
				db.Store(item);
				db.Commit();
				Assert.IsTrue(new ConsistencyChecker(db).CheckSlotConsistency().Consistent());
				proc.Apply(new Pair(db, item));
				db.Commit();
				Assert.IsFalse(new ConsistencyChecker(db).CheckSlotConsistency().Consistent());
			}
			finally
			{
				db.Close();
			}
		}
	}
}
