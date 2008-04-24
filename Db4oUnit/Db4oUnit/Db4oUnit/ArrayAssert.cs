/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	public partial class ArrayAssert
	{
		public static void Contains(long[] array, long expected)
		{
			if (-1 != IndexOf(array, expected))
			{
				return;
			}
			Assert.Fail("Expecting '" + expected + "'.");
		}

		public static void Contains(object[] array, object[] expected)
		{
			for (int i = 0; i < expected.Length; i++)
			{
				if (-1 == IndexOf(array, expected[i]))
				{
					Assert.Fail("Expecting contains '" + expected[i] + "'.");
				}
			}
		}

		public static int IndexOf(object[] array, object expected)
		{
			for (int i = 0; i < array.Length; ++i)
			{
				if (expected.Equals(array[i]))
				{
					return i;
				}
			}
			return -1;
		}

		public static void AreEqual(object[] expected, object[] actual)
		{
			AreEqualImpl(expected, actual);
		}

		public static void AreEqual(int[][] expected, int[][] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Assert.AreSame(expected, actual);
			}
			Assert.AreEqual(expected.Length, actual.Length);
			Assert.AreSame(expected.GetType(), actual.GetType());
			for (int i = 0; i < expected.Length; i++)
			{
				AreEqual(expected[i], actual[i]);
			}
		}

		public static void AreEqual(object[][] expected, object[][] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Assert.AreSame(expected, actual);
			}
			Assert.AreEqual(expected.Length, actual.Length);
			Assert.AreSame(expected.GetType(), actual.GetType());
			for (int i = 0; i < expected.Length; i++)
			{
				AreEqual(expected[i], actual[i]);
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
				Assert.AreSame(expected, actual);
			}
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
			}
		}

		public static void AreNotEqual(byte[] expected, byte[] actual)
		{
			Assert.AreNotSame(expected, actual);
			for (int i = 0; i < expected.Length; i++)
			{
				if (expected[i] != actual[i])
				{
					return;
				}
			}
			Assert.IsTrue(false);
		}

		public static void AreEqual(int[] expected, int[] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Assert.AreSame(expected, actual);
			}
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
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
				Assert.AreSame(expected, actual);
			}
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
			}
		}

		public static void AreEqual(char[] expected, char[] actual)
		{
			if (expected == actual)
			{
				return;
			}
			if (expected == null || actual == null)
			{
				Assert.AreSame(expected, actual);
			}
			Assert.AreEqual(expected.Length, actual.Length);
			for (int i = 0; i < expected.Length; i++)
			{
				Assert.AreEqual(expected[i], actual[i], IndexMessage(i));
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
