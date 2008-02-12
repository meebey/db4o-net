/* Copyright (C) 2004-2008   db4objects Inc.   http://www.db4o.com */

#if CF_3_5

using System.Collections.Generic;
using Db4oUnit;
using Db4oUnit.Extensions;
using System.Linq;
using Db4objects.Db4o.Linq;

namespace Db4objects.Db4o.Tests.Compact
{
	class UnoptimizedLinqTestCase : AbstractDb4oTestCase
	{
		protected override void Store()
		{
			TestSubItem bar = new TestSubItem("bar", 1);
			
			Store(new TestSubject("foo", bar));
			Store(new TestSubject("baz", bar));
		}

		public void Test()
		{
			var result = from TestSubject subject in Db() where subject._name == "baz" select subject;

			Assert.IsTrue(result.GetType().FullName.Contains("System.Linq.Enumerable+<WhereIterator>"));
			AssertItemCount(result, 1);

			TestSubject item = GetFirstItem(result);

			Assert.IsNotNull(item);
			Assert.AreEqual("baz", item._name);
			Assert.IsNotNull(item._item);
			Assert.AreEqual("bar", item._item._name);
		}

		private TestSubject GetFirstItem(IEnumerable<TestSubject> result)
		{
			if (result == null) return null;

			IEnumerator<TestSubject> enumerator = result.GetEnumerator();
			enumerator.MoveNext();
			return enumerator.Current;
		}

		private void AssertItemCount(IEnumerable<TestSubject> result, int expected)
		{
			int actual = 0;
			foreach(TestSubject subject in result)
			{
				actual++;
			}

			Assert.AreEqual(expected, actual);
		}
	}

	public class TestSubject
	{
		public TestSubject(string name, TestSubItem item)
		{
			_name = name;
			_item = item;
		}

		public string _name;
		public TestSubItem _item;
	}

	public class TestSubItem
	{
		public TestSubItem(string name, int value)
		{
			_name = name;
			_value = value;
		}

		public string _name;
		public int _value;
	}
}

#endif