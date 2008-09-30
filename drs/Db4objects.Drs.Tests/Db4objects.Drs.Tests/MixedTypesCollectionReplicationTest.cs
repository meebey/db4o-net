/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
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
			CollectionHolder remaining = NextCollectionHolder(holder.set.GetEnumerator());
			Assert.AreEqual("h1", remaining.name);
		}

		public virtual void Test()
		{
			ActualTest();
		}
	}
}
