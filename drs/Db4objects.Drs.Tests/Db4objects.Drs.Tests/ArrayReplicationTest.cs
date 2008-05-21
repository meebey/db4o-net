/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Drs;
using Db4objects.Drs.Inside;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class ArrayReplicationTest : DrsTestCase
	{
		public virtual void Test()
		{
			if (!A().Provider().SupportsMultiDimensionalArrays())
			{
				return;
			}
			if (!B().Provider().SupportsMultiDimensionalArrays())
			{
				return;
			}
			ArrayHolder h1 = new ArrayHolder("h1");
			ArrayHolder h2 = new ArrayHolder("h2");
			h1._array = new ArrayHolder[] { h1 };
			h2._array = new ArrayHolder[] { h1, h2, null };
			h1._arrayN = new ArrayHolder[][] { new ArrayHolder[] { h1 } };
			h2._arrayN = new ArrayHolder[][] { new ArrayHolder[] { h1, null }, new ArrayHolder
				[] { null, h2 }, new ArrayHolder[] { null, null } };
			//TODO Fix ReflectArray.shape() and test with innermost arrays of varying sizes:  {{h1}, {null, h2}, {null}}
			B().Provider().StoreNew(h2);
			B().Provider().StoreNew(h1);
			IReplicationSession replication = new GenericReplicationSession(A().Provider(), B
				().Provider());
			replication.Replicate(h2);
			//Traverses to h1.
			replication.Commit();
			IEnumerator objects = A().Provider().GetStoredObjects(typeof(ArrayHolder)).GetEnumerator
				();
			CheckNext(objects);
			CheckNext(objects);
		}

		private void CheckNext(IEnumerator objects)
		{
			Assert.IsTrue(objects.MoveNext());
			Check((ArrayHolder)objects.Current);
		}

		private void Check(ArrayHolder holder)
		{
			if (holder._name.Equals("h1"))
			{
				CheckH1(holder);
			}
			else
			{
				CheckH2(holder);
			}
		}

		protected virtual void CheckH1(ArrayHolder holder)
		{
			Assert.AreEqual(holder._array[0], holder);
			Assert.AreEqual(holder._arrayN[0][0], holder);
		}

		protected virtual void CheckH2(ArrayHolder holder)
		{
			Assert.AreEqual(holder._array[0]._name, "h1");
			Assert.AreEqual(holder._array[1], holder);
			Assert.AreEqual(holder._array[2], null);
			Assert.AreEqual(holder._arrayN[0][0]._name, "h1");
			Assert.AreEqual(holder._arrayN[1][0], null);
			Assert.AreEqual(holder._arrayN[1][1], holder);
			Assert.AreEqual(holder._arrayN[2][0], null);
		}
	}
}
