namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public sealed class YapBit
	{
		private int i_value;

		public YapBit(int a_value)
		{
			i_value = a_value;
		}

		public void Set(bool a_bit)
		{
			i_value = i_value * 2;
			if (a_bit)
			{
				i_value++;
			}
		}

		public bool Get()
		{
			double cmp = (double)i_value / 2;
			i_value = i_value / 2;
			return (cmp != i_value);
		}

		public byte GetByte()
		{
			return (byte)i_value;
		}
	}
}
