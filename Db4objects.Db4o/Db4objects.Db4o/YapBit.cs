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
			i_value <<= 1;
			if (a_bit)
			{
				i_value |= 1;
			}
		}

		public bool Get()
		{
			bool ret = ((i_value & 1) != 0);
			i_value >>= 1;
			return ret;
		}

		public byte GetByte()
		{
			return (byte)i_value;
		}
	}
}
