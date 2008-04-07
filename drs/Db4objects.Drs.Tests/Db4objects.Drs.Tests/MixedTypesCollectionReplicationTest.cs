/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class MixedTypesCollectionReplicationTest : DrsTestCase
	{
		protected virtual void ActualTest()
		{
			if (!A().Provider().SupportsHybridCollection())
			{
				return;
			}
			if (!B().Provider().SupportsHybridCollection())
			{
				return;
			}
			CollectionHolder h1 = new CollectionHolder("h1");
			CollectionHolder h2 = new CollectionHolder("h2");
			h1.map.Add("key", "value");
			h1.map.Add("key2", h1);
			h1.map.Add(h1, "value2");
			h2.map.Add("key", h1);
			h2.map.Add(h2, h1);
			h1.list.Add("one");
			h1.list.Add(h1);
			h2.list.Add("two");
			h2.list.Add(h1);
			h2.list.Add(h2);
			h1.set.Add("one");
			h1.set.Add(h1);
			h2.set.Add("two");
			h2.set.Add(h1);
			h2.set.Add(h2);
			B().Provider().StoreNew(h2);
			B().Provider().StoreNew(h1);
			IReplicationSession replication = new GenericReplicationSession(A().Provider(), B
				().Provider());
			replication.Replicate(h2);
			//Traverses to h1.
			replication.Commit();
			IEnumerator objects = A().Provider().GetStoredObjects(typeof(CollectionHolder)).GetEnumerator
				();
			Check(NextCollectionHolder(objects), h1, h2);
			Check(NextCollectionHolder(objects), h1, h2);
		}

		private CollectionHolder NextCollectionHolder(IEnumerator objects)
		{
			Assert.IsTrue(objects.MoveNext());
			return (CollectionHolder)objects.Current;
		}

		private void Check(CollectionHolder holder, CollectionHolder original1, CollectionHolder
			 original2)
		{
			Assert.IsTrue(holder != original1);
			Assert.IsTrue(holder != original2);
			if (holder.name.Equals("h1"))
			{
				CheckH1(holder);
			}
			else
			{
				CheckH2(holder);
			}
		}

		private void CheckH1(CollectionHolder holder)
		{
			Assert.AreEqual("value", holder.map["key"]);
			Assert.AreEqual(holder, holder.map["key2"]);
			Assert.AreEqual("value2", holder.map[holder]);
			Assert.AreEqual("one", holder.list[0]);
			Assert.AreEqual(holder, holder.list[1]);
			Assert.IsTrue(holder.set.Contains("one"));
			Assert.IsTrue(holder.set.Contains(holder));
		}

		private void CheckH2(CollectionHolder holder)
		{
			Assert.AreEqual("h1", ((CollectionHolder)holder.map["key"]).name);
			Assert.AreEqual("h1", ((CollectionHolder)holder.map[holder]).name);
			Assert.AreEqual("two", holder.list[0]);
			Assert.AreEqual("h1", ((CollectionHolder)holder.list[1]).name);
			Assert.AreEqual(holder, holder.list[2]);
			Assert.IsTrue(holder.set.Remove("two"));
			Assert.IsTrue(holder.set.Remove(holder));
			CollectionHolder remaining = (CollectionHolder)holder.set.GetEnumerator().Current;
			Assert.AreEqual("h1", remaining.name);
		}

		public virtual void Test()
		{
			ActualTest();
		}
	}
}
