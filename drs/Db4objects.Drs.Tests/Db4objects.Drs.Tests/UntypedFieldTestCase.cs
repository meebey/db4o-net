/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class UntypedFieldTestCase : DrsTestCase
	{
		public sealed class Item
		{
			public object untyped;

			public Item(object value)
			{
				untyped = value;
			}
		}

		public sealed class Data
		{
			public int id;

			public Data(int value)
			{
				id = value;
			}

			public override bool Equals(object obj)
			{
				UntypedFieldTestCase.Data other = (UntypedFieldTestCase.Data)obj;
				return id == other.id;
			}
		}

		public virtual void TestUntypedString()
		{
			AssertUntypedReplication("42");
		}

		public virtual void TestUntypedStringArray()
		{
			AssertUntypedReplication(new object[] { "42" });
		}

		public virtual void TestUntypedDate()
		{
			AssertUntypedReplication(new DateTime(100, 2, 2));
		}

		public virtual void TestUntypedDateArray()
		{
			AssertUntypedReplication(new object[] { new DateTime(100, 2, 2) });
		}

		public virtual void TestUntypedMixedArray()
		{
			AssertUntypedReplication(new object[] { "42", new UntypedFieldTestCase.Data(42) }
				);
			Assert.AreEqual(42, ((UntypedFieldTestCase.Data)SingleReplicatedInstance(typeof(UntypedFieldTestCase.Data
				))).id);
		}

		private void AssertUntypedReplication(object data)
		{
			UntypedFieldTestCase.Item item = new UntypedFieldTestCase.Item(data);
			A().Provider().StoreNew(item);
			A().Provider().Commit();
			ReplicateAll(A().Provider(), B().Provider());
			UntypedFieldTestCase.Item replicated = (UntypedFieldTestCase.Item)SingleReplicatedInstance
				(typeof(UntypedFieldTestCase.Item));
			object expected = item.untyped;
			if (expected is object[])
			{
				ArrayAssert.AreEqual((object[])expected, (object[])replicated.untyped);
			}
			else
			{
				Assert.AreEqual(expected, replicated.untyped);
			}
		}

		private object SingleReplicatedInstance(Type klass)
		{
			IObjectSet found = B().Provider().GetStoredObjects(klass);
			Assert.AreEqual(1, found.Count);
			return found[0];
		}
	}
}
