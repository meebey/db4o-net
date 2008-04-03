/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class SingleTypeCollectionReplicationTest : DrsTestCase
	{
		public virtual void _Test()
		{
			CollectionHolder h1 = new CollectionHolder();
			h1.map.Add("1", "one");
			h1.set.Add("two");
			h1.list.Add("three");
			StoreNewAndCommit(A().Provider(), h1);
			ReplicateAll(A().Provider(), B().Provider());
			IEnumerator it = B().Provider().GetStoredObjects(typeof(CollectionHolder)).GetEnumerator
				();
			Assert.IsTrue(it.MoveNext());
			CollectionHolder replica = (CollectionHolder)it.Current;
			Assert.AreEqual("one", replica.map["1"]);
			Assert.IsTrue(replica.set.Contains("two"));
			Assert.AreEqual("three", replica.list[0]);
		}

		private void StoreNewAndCommit(ITestableReplicationProviderInside provider, CollectionHolder
			 h1)
		{
			provider.StoreNew(h1);
			provider.Activate(h1);
			provider.Commit();
		}
	}
}
