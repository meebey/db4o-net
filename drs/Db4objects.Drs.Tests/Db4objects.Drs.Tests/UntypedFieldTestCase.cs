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

		public sealed class ItemWithCloneable
		{
			public ICloneable value;

			public ItemWithCloneable(ICloneable c)
			{
				value = c;
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

		public virtual void TestUntypedStringJaggedArray()
		{
			AssertJaggedArray("42");
		}

		public virtual void TestUntypedFirstClassJaggedArray()
		{
			AssertJaggedArray(new UntypedFieldTestCase.Data(42));
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

		public virtual void TestArrayAsCloneable()
		{
			object[] array = new object[] { "42", new UntypedFieldTestCase.Data(42) };
			UntypedFieldTestCase.ItemWithCloneable replicated = (UntypedFieldTestCase.ItemWithCloneable
				)Replicate(new UntypedFieldTestCase.ItemWithCloneable(array));
			AssertEquals(array, replicated.value);
		}

		private void AssertUntypedReplication(object data)
		{
			AssertEquals(data, ReplicateItem(data).untyped);
		}

		private void AssertJaggedArray(object data)
		{
			object[] expected = new object[] { new object[] { data } };
			object[] actual = (object[])ReplicateItem(expected).untyped;
			Assert.AreEqual(expected.Length, actual.Length);
			object[] nested = (object[])actual[0];
			object actualValue = nested[0];
			Assert.AreEqual(data, actualValue);
			AssertNotSame(data, actualValue);
		}

		private void AssertNotSame(object expectedFirstClass, object actual)
		{
			if (IsFirstClass(expectedFirstClass.GetType()))
			{
				Assert.AreNotSame(expectedFirstClass, actual);
			}
		}

		private bool IsFirstClass(Type klass)
		{
			if (klass.IsPrimitive)
			{
				return false;
			}
			if (klass == typeof(string))
			{
				return false;
			}
			if (klass == typeof(DateTime))
			{
				return false;
			}
			return true;
		}

		private void AssertEquals(object expected, object actual)
		{
			if (expected is object[])
			{
				AssertEquals((object[])expected, (object[])actual);
			}
			else
			{
				Assert.AreEqual(expected, actual);
				AssertNotSame(expected, actual);
			}
		}

		private void AssertEquals(object[] expectedArray, object[] actualArray)
		{
			ArrayAssert.AreEqual(expectedArray, actualArray);
			for (int i = 0; i < expectedArray.Length; ++i)
			{
				AssertNotSame(expectedArray[i], actualArray[i]);
			}
		}

		private UntypedFieldTestCase.Item ReplicateItem(object data)
		{
			return (UntypedFieldTestCase.Item)Replicate(new UntypedFieldTestCase.Item(data));
		}

		private object Replicate(object item)
		{
			A().Provider().StoreNew(item);
			A().Provider().Commit();
			ReplicateAll(A().Provider(), B().Provider());
			return SingleReplicatedInstance(item.GetType());
		}

		private object SingleReplicatedInstance(Type klass)
		{
			IObjectSet found = B().Provider().GetStoredObjects(klass);
			Assert.AreEqual(1, found.Count);
			return found[0];
		}
	}
}
