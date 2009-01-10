/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Jre12.Collections
{
	public class IteratorAssert
	{
		public static void AreEqual(IEnumerator expected, IEnumerator actual)
		{
			if (null == expected)
			{
				Assert.IsNull(actual);
				return;
			}
			Assert.IsNotNull(actual);
			while (expected.MoveNext())
			{
				Assert.IsTrue(actual.MoveNext());
				Assert.AreEqual(expected.Current, actual.Current);
			}
			Assert.IsFalse(actual.MoveNext());
		}

		public static void AreEqual(object[] expected, IEnumerator iterator)
		{
			ArrayList v = new ArrayList();
			for (int i = 0; i < expected.Length; i++)
			{
				v.Add(expected[i]);
			}
			AreEqual(v.GetEnumerator(), iterator);
		}

		public static void SameContent(IEnumerator expected, IEnumerator actual)
		{
			Collection4 allExpected = new Collection4();
			while (expected.MoveNext())
			{
				allExpected.Add(expected.Current);
			}
			while (actual.MoveNext())
			{
				object current = actual.Current;
				bool removed = allExpected.Remove(current);
				if (!removed)
				{
					Unexpected(current);
				}
			}
			Assert.IsTrue(allExpected.IsEmpty(), allExpected.ToString());
		}

		private static void Unexpected(object element)
		{
			Assert.Fail("Unexpected element: " + element);
		}
	}
}
