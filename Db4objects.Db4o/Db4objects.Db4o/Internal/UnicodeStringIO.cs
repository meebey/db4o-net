namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class UnicodeStringIO : Db4objects.Db4o.Internal.LatinStringIO
	{
		public override int BytesPerChar()
		{
			return 2;
		}

		public override byte EncodingByte()
		{
			return Db4objects.Db4o.Internal.Const4.UNICODE;
		}

		public override int Length(string a_string)
		{
			return (a_string.Length * 2) + Db4objects.Db4o.Internal.Const4.OBJECT_LENGTH + Db4objects.Db4o.Internal.Const4
				.INT_LENGTH;
		}

		public override string Read(Db4objects.Db4o.Internal.Buffer bytes, int a_length)
		{
			CheckBufferLength(a_length);
			for (int ii = 0; ii < a_length; ii++)
			{
				chars[ii] = (char)((bytes._buffer[bytes._offset++] & unchecked((int)(0xff))) | ((
					bytes._buffer[bytes._offset++] & unchecked((int)(0xff))) << 8));
			}
			return new string(chars, 0, a_length);
		}

		public override string Read(byte[] a_bytes)
		{
			int len = a_bytes.Length / 2;
			CheckBufferLength(len);
			int j = 0;
			for (int ii = 0; ii < len; ii++)
			{
				chars[ii] = (char)((a_bytes[j++] & unchecked((int)(0xff))) | ((a_bytes[j++] & unchecked(
					(int)(0xff))) << 8));
			}
			return new string(chars, 0, len);
		}

		public override int ShortLength(string a_string)
		{
			return (a_string.Length * 2) + Db4objects.Db4o.Internal.Const4.INT_LENGTH;
		}

		public override void Write(Db4objects.Db4o.Internal.Buffer bytes, string @string)
		{
			int len = WritetoBuffer(@string);
			for (int i = 0; i < len; i++)
			{
				bytes._buffer[bytes._offset++] = (byte)(chars[i] & unchecked((int)(0xff)));
				bytes._buffer[bytes._offset++] = (byte)(chars[i] >> 8);
			}
		}

		internal override byte[] Write(string @string)
		{
			int len = WritetoBuffer(@string);
			byte[] bytes = new byte[len * 2];
			int j = 0;
			for (int i = 0; i < len; i++)
			{
				bytes[j++] = (byte)(chars[i] & unchecked((int)(0xff)));
				bytes[j++] = (byte)(chars[i] >> 8);
			}
			return bytes;
		}
	}
}
