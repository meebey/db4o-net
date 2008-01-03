/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class LatinStringIO
	{
		public virtual int BytesPerChar()
		{
			return 1;
		}

		public virtual byte EncodingByte()
		{
			return Const4.Iso8859;
		}

		internal static LatinStringIO ForEncoding(byte encodingByte)
		{
			switch (encodingByte)
			{
				case Const4.Iso8859:
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

		public virtual int Length(string str)
		{
			return str.Length + Const4.ObjectLength + Const4.IntLength;
		}

		public virtual string Read(IReadBuffer buffer, int length)
		{
			char[] chars = new char[length];
			for (int ii = 0; ii < length; ii++)
			{
				chars[ii] = (char)(buffer.ReadByte() & unchecked((int)(0xff)));
			}
			return new string(chars, 0, length);
		}

		public virtual string Read(byte[] bytes)
		{
			char[] chars = new char[bytes.Length];
			for (int i = 0; i < bytes.Length; i++)
			{
				chars[i] = (char)(bytes[i] & unchecked((int)(0xff)));
			}
			return new string(chars, 0, bytes.Length);
		}

		public virtual int ShortLength(string str)
		{
			return str.Length + Const4.IntLength;
		}

		public virtual void Write(IWriteBuffer buffer, string str)
		{
			int length = str.Length;
			char[] chars = new char[length];
			Sharpen.Runtime.GetCharsForString(str, 0, length, chars, 0);
			for (int i = 0; i < length; i++)
			{
				buffer.WriteByte((byte)(chars[i] & unchecked((int)(0xff))));
			}
		}

		public virtual byte[] Write(string str)
		{
			int length = str.Length;
			char[] chars = new char[length];
			Sharpen.Runtime.GetCharsForString(str, 0, length, chars, 0);
			byte[] bytes = new byte[length];
			for (int i = 0; i < length; i++)
			{
				bytes[i] = (byte)(chars[i] & unchecked((int)(0xff)));
			}
			return bytes;
		}
	}
}
