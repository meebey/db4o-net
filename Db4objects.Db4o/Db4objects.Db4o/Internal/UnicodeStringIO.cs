/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public sealed class UnicodeStringIO : LatinStringIO
	{
		public override int BytesPerChar()
		{
			return 2;
		}

		public override byte EncodingByte()
		{
			return Const4.UNICODE;
		}

		public override int Length(string a_string)
		{
			return (a_string.Length * 2) + Const4.OBJECT_LENGTH + Const4.INT_LENGTH;
		}

		public override string Read(IReadBuffer buffer, int length)
		{
			CheckBufferLength(length);
			for (int ii = 0; ii < length; ii++)
			{
				chars[ii] = (char)((buffer.ReadByte() & unchecked((int)(0xff))) | ((buffer.ReadByte
					() & unchecked((int)(0xff))) << 8));
			}
			return new string(chars, 0, length);
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
			return (a_string.Length * 2) + Const4.INT_LENGTH;
		}

		public override void Write(IWriteBuffer buffer, string @string)
		{
			int len = MarshalledLength(@string);
			for (int i = 0; i < len; i++)
			{
				buffer.WriteByte((byte)(chars[i] & unchecked((int)(0xff))));
				buffer.WriteByte((byte)(chars[i] >> 8));
			}
		}

		internal override byte[] Write(string @string)
		{
			int len = MarshalledLength(@string);
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
