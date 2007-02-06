namespace Db4objects.Db4o.Foundation
{
	/// <exclude></exclude>
	public class Arrays4
	{
		public static int IndexOf(object[] array, object element)
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

		public static bool AreEqual(byte[] x, byte[] y)
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

		public static bool ContainsInstanceOf(object[] array, System.Type klass)
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
	}
}
