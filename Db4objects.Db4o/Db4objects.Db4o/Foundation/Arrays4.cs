/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Sharpen;

namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Arrays4
	{
		public static int[] CopyOf(int[] src, int newLength)
		{
			int[] copy = new int[newLength];
			System.Array.Copy(src, 0, copy, 0, Math.Min(src.Length, newLength));
			return copy;
		}

		public static int IndexOfIdentity(object[] array, object element)
		{
			for (int i = 0; i < array.Length; i++)
			{
				if (array[i] == element)
				{
					return i;
				}
			}
			return -1;
		}

		public static int IndexOfEquals(object[] array, object expected)
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

		public static bool Equals(byte[] x, byte[] y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null)
			{
				return false;
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < x.Length; i++)
			{
				if (y[i] != x[i])
				{
					return false;
				}
			}
			return true;
		}

		public static bool Equals(object[] x, object[] y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null)
			{
				return false;
			}
			if (x.Length != y.Length)
			{
				return false;
			}
			for (int i = 0; i < x.Length; i++)
			{
				if (!ObjectsAreEqual(y[i], x[i]))
				{
					return false;
				}
			}
			return true;
		}

		private static bool ObjectsAreEqual(object x, object y)
		{
			if (x == y)
			{
				return true;
			}
			if (x == null || y == null)
			{
				return false;
			}
			return x.Equals(y);
		}

		public static bool ContainsInstanceOf(object[] array, Type klass)
		{
			if (array == null)
			{
				return false;
			}
			for (int i = 0; i < array.Length; ++i)
			{
				if (klass.IsInstanceOfType(array[i]))
				{
					return true;
				}
			}
			return false;
		}

		public static void Fill(object[] array, object value)
		{
			for (int i = 0; i < array.Length; ++i)
			{
				array[i] = value;
			}
		}
	}
}
