namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class IteratorAssert
	{
		public static void AreEqual(System.Collections.IEnumerator expected, System.Collections.IEnumerator
			 actual)
		{
			if (null == expected)
			{
				Db4oUnit.Assert.IsNull(actual);
			}
			Db4oUnit.Assert.IsNotNull(actual);
			while (expected.MoveNext())
			{
				Db4oUnit.Assert.IsTrue(actual.MoveNext(), "'" + expected.Current + "' expected.");
				Db4oUnit.Assert.AreEqual(expected.Current, actual.Current);
			}
			Db4oUnit.Assert.IsFalse(actual.MoveNext());
		}

		public static void AreEqual(object[] expected, System.Collections.IEnumerator iterator
			)
		{
			AreEqual(new Db4objects.Db4o.Foundation.ArrayIterator4(expected), iterator);
		}
	}
}
