namespace Db4objects.Db4o.Foundation
{
	public sealed class PrimitiveCodec
	{
		public const int INT_LENGTH = 4;

		public const int LONG_LENGTH = 8;

		public static int ReadInt(byte[] buffer, int offset)
		{
			offset += 3;
			return (buffer[offset] & 255) | (buffer[--offset] & 255) << 8 | (buffer[--offset]
				 & 255) << 16 | buffer[--offset] << 24;
		}

		public static void WriteInt(byte[] buffer, int offset, int val)
		{
			offset += 3;
			buffer[offset] = (byte)val;
			buffer[--offset] = (byte)(val >>= 8);
			buffer[--offset] = (byte)(val >>= 8);
			buffer[--offset] = (byte)(val >> 8);
		}

		public static void WriteLong(byte[] buffer, long val)
		{
			WriteLong(buffer, 0, val);
		}

		public static void WriteLong(byte[] buffer, int offset, long val)
		{
			for (int i = 0; i < LONG_LENGTH; i++)
			{
				buffer[offset++] = (byte)(val >> ((7 - i) * 8));
			}
		}

		public static long ReadLong(byte[] buffer, int offset)
		{
			long ret = 0;
			for (int i = 0; i < LONG_LENGTH; i++)
			{
				ret = (ret << 8) + (buffer[offset++] & unchecked((int)(0xff)));
			}
			return ret;
		}
	}
}
