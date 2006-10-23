namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class IntArrays4
	{
		public static int[] Fill(int[] array, int value)
		{
			for (int i = 0; i < array.Length; ++i)
			{
				array[i] = value;
			}
			return array;
		}

		public static int[] Concat(int[] a, int[] b)
		{
			int[] array = new int[a.Length + b.Length];
			System.Array.Copy(a, 0, array, 0, a.Length);
			System.Array.Copy(b, 0, array, a.Length, b.Length);
			return array;
		}

		public static int Occurences(int[] values, int value)
		{
			int count = 0;
			for (int i = 0; i < values.Length; i++)
			{
				if (values[i] == value)
				{
					count++;
				}
			}
			return count;
		}

		public static int[] Clone(int[] bars)
		{
			int[] array = new int[bars.Length];
			System.Array.Copy(bars, 0, array, 0, bars.Length);
			return array;
		}

		public static object[] ToObjectArray(int[] values)
		{
			object[] ret = new object[values.Length];
			for (int i = 0; i < values.Length; i++)
			{
				ret[i] = values[i];
			}
			return ret;
		}

		public static System.Collections.IEnumerator NewIterator(int[] values)
		{
			return new Db4objects.Db4o.Foundation.ArrayIterator4(ToObjectArray(values));
		}
	}
}
