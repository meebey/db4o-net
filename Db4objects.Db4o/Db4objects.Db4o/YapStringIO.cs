namespace Db4objects.Db4o
{
	/// <exclude></exclude>
	public class YapStringIO
	{
		protected char[] chars = new char[0];

		public virtual int BytesPerChar()
		{
			return 1;
		}

		public virtual byte EncodingByte()
		{
			return Db4objects.Db4o.YapConst.ISO8859;
		}

		internal static Db4objects.Db4o.YapStringIO ForEncoding(byte encodingByte)
		{
			switch (encodingByte)
			{
				case Db4objects.Db4o.YapConst.ISO8859:
				{
					return new Db4objects.Db4o.YapStringIO();
				}

				default:
				{
					return new Db4objects.Db4o.YapStringIOUnicode();
					break;
				}
			}
		}

		public virtual int Length(string a_string)
		{
			return a_string.Length + Db4objects.Db4o.YapConst.OBJECT_LENGTH + Db4objects.Db4o.YapConst
				.INT_LENGTH;
		}

		protected virtual void CheckBufferLength(int a_length)
		{
			if (a_length > chars.Length)
			{
				chars = new char[a_length];
			}
		}

		public virtual string Read(Db4objects.Db4o.YapReader bytes, int a_length)
		{
			CheckBufferLength(a_length);
			for (int ii = 0; ii < a_length; ii++)
			{
				chars[ii] = (char)(bytes._buffer[bytes._offset++] & unchecked((int)(0xff)));
			}
			return new string(chars, 0, a_length);
		}

		public virtual string Read(byte[] a_bytes)
		{
			CheckBufferLength(a_bytes.Length);
			for (int i = 0; i < a_bytes.Length; i++)
			{
				chars[i] = (char)(a_bytes[i] & unchecked((int)(0xff)));
			}
			return new string(chars, 0, a_bytes.Length);
		}

		public virtual int ShortLength(string a_string)
		{
			return a_string.Length + Db4objects.Db4o.YapConst.INT_LENGTH;
		}

		protected virtual int WritetoBuffer(string str)
		{
			int len = str.Length;
			CheckBufferLength(len);
			Sharpen.Runtime.GetCharsForString(str, 0, len, chars, 0);
			return len;
		}

		public virtual void Write(Db4objects.Db4o.YapReader bytes, string @string)
		{
			int len = WritetoBuffer(@string);
			for (int i = 0; i < len; i++)
			{
				bytes._buffer[bytes._offset++] = (byte)(chars[i] & unchecked((int)(0xff)));
			}
		}

		internal virtual byte[] Write(string @string)
		{
			int len = WritetoBuffer(@string);
			byte[] bytes = new byte[len];
			for (int i = 0; i < len; i++)
			{
				bytes[i] = (byte)(chars[i] & unchecked((int)(0xff)));
			}
			return bytes;
		}
	}
}
