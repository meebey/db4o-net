/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
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
				Assert.IsTrue(actual.MoveNext(), "'" + expected.Current + "' expected.");
				Assert.AreEqual(expected.Current, actual.Current);
			}
			Assert.IsFalse(actual.MoveNext());
		}

		public static void AreEqual(object[] expected, IEnumerator iterator)
		{
			AreEqual(new ArrayIterator4(expected), iterator);
		}
	}
}
