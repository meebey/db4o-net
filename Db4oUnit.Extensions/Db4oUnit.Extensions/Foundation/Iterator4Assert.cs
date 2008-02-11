/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit.Extensions.Foundation
{
	public class Iterator4Assert
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
				AssertNext(expected.Current, actual);
			}
			if (actual.MoveNext())
			{
				Assert.Fail("Unexpected element: " + actual.Current);
			}
		}

		public static void AssertNext(object expected, IEnumerator iterator)
		{
			Assert.IsTrue(iterator.MoveNext(), "'" + expected + "' expected.");
			Assert.AreEqual(expected, iterator.Current);
		}

		public static void AreEqual(object[] expected, IEnumerator iterator)
		{
			AreEqual(new ArrayIterator4(expected), iterator);
		}
	}
}
