namespace Db4oUnit
{
	public class ArrayAssert
	{
		public static void Contains(long[] array, long expected)
		{
			if (-1 != IndexOf(array, expected))
			{
				return;
			}
			Db4oUnit.Assert.Fail("Expecting '" + expected + "'.");
		}

		public static void AreEqual(object[] expected, object[] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Db4oUnit.Assert.AreSame(expected, actual);
			}
			Db4oUnit.Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Db4oUnit.Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
			}
		}

		private static string IndexMessage(int i)
		{
			return "expected[" + i + "]";
		}

		public static void AreEqual(byte[] expected, byte[] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Db4oUnit.Assert.AreSame(expected, actual);
			}
			Db4oUnit.Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Db4oUnit.Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
			}
		}

		public static void AreNotEqual(byte[] expected, byte[] actual)
		{
			Db4oUnit.Assert.AreNotSame(expected, actual);
			for (int i = 0; i < expected.Length; i++)
			{
				if (expected[i] != actual[i])
				{
					return;
				}
			}
			Db4oUnit.Assert.IsTrue(false);
		}

		public static void AreEqual(int[] expected, int[] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Db4oUnit.Assert.AreSame(expected, actual);
			}
			Db4oUnit.Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Db4oUnit.Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
			}
		}

		public static void AreEqual(double[] expected, double[] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Db4oUnit.Assert.AreSame(expected, actual);
			}
			Db4oUnit.Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Db4oUnit.Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
			}
		}

		private static int IndexOf(long[] array, long expected)
		{
			for (int i = 0; i < array.Length; ++i)
			{
				if (expected == array[i])
				{
					return i;
				}
			}
			return -1;
		}
	}
}
