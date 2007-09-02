/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LatinStringIO
	{
		protected char[] chars = new char[0];

		public virtual int BytesPerChar()
		{
			return 1;
		}

		public virtual byte EncodingByte()
		{
			return Const4.ISO8859;
		}

		internal static LatinStringIO ForEncoding(byte encodingByte)
		{
			switch (encodingByte)
			{
				case Const4.ISO8859:
				{
					return new LatinStringIO();
				}

				default:
				{
					return new UnicodeStringIO();
					break;
				}
			}
		}

		public virtual int Length(string a_string)
		{
			return a_string.Length + Const4.OBJECT_LENGTH + Const4.INT_LENGTH;
		}

		protected virtual void CheckBufferLength(int a_length)
		{
			if (a_length > chars.Length)
			{
				chars = new char[a_length];
			}
		}

		public virtual string Read(IReadBuffer buffer, int length)
		{
			CheckBufferLength(length);
			for (int ii = 0; ii < length; ii++)
			{
				chars[ii] = (char)(buffer.ReadByte() & unchecked((int)(0xff)));
			}
			return new string(chars, 0, length);
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
			return a_string.Length + Const4.INT_LENGTH;
		}

		protected virtual int MarshalledLength(string str)
		{
			int len = str.Length;
			CheckBufferLength(len);
			Sharpen.Runtime.GetCharsForString(str, 0, len, chars, 0);
			return len;
		}

		public virtual void Write(IWriteBuffer buffer, string @string)
		{
			int len = MarshalledLength(@string);
			for (int i = 0; i < len; i++)
			{
				buffer.WriteByte((byte)(chars[i] & unchecked((int)(0xff))));
			}
		}

		internal virtual byte[] Write(string @string)
		{
			int len = MarshalledLength(@string);
			byte[] bytes = new byte[len];
			for (int i = 0; i < len; i++)
			{
				bytes[i] = (byte)(chars[i] & unchecked((int)(0xff)));
			}
			return bytes;
		}
	}
}
