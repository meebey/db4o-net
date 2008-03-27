/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class SingleTypeCollectionReplicationTest : Db4objects.Drs.Tests.DrsTestCase
	{
		public virtual void _Test()
		{
			Db4objects.Drs.Tests.CollectionHolder h1 = new Db4objects.Drs.Tests.CollectionHolder
				();
			h1.map.Add("1", "one");
			h1.set.Add("two");
			h1.list.Add("three");
			StoreNewAndCommit(A().Provider(), h1);
			ReplicateAll(A().Provider(), B().Provider());
			System.Collections.IEnumerator it = B().Provider().GetStoredObjects(typeof(Db4objects.Drs.Tests.CollectionHolder
				)).GetEnumerator();
			Db4oUnit.Assert.IsTrue(it.MoveNext());
			Db4objects.Drs.Tests.CollectionHolder replica = (Db4objects.Drs.Tests.CollectionHolder
				)it.Current;
			Db4oUnit.Assert.AreEqual("one", replica.map["1"]);
			Db4oUnit.Assert.IsTrue(replica.set.Contains("two"));
			Db4oUnit.Assert.AreEqual("three", replica.list[0]);
		}

		private void StoreNewAndCommit(Db4objects.Drs.Inside.ITestableReplicationProviderInside
			 provider, Db4objects.Drs.Tests.CollectionHolder h1)
		{
			provider.StoreNew(h1);
			provider.Activate(h1);
			provider.Commit();
		}
	}
}
