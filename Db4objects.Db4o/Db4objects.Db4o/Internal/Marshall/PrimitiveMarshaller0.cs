/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class PrimitiveMarshaller0 : PrimitiveMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return true;
		}

		public override DateTime ReadDate(BufferImpl bytes)
		{
			long value = bytes.ReadLong();
			if (value == long.MaxValue)
			{
				return MarshallingConstants0.NULL_DATE;
			}
			return new DateTime(value);
		}

		public override object ReadInteger(BufferImpl bytes)
		{
			int value = bytes.ReadInt();
			if (value == int.MaxValue)
			{
				return null;
			}
			return value;
		}

		public override object ReadFloat(BufferImpl bytes)
		{
			float value = UnmarshallFloat(bytes);
			if (float.IsNaN(value))
			{
				return null;
			}
			return value;
		}

		public override object ReadDouble(BufferImpl buffer)
		{
			double value = UnmarshalDouble(buffer);
			if (double.IsNaN(value))
			{
				return null;
			}
			return value;
		}

		public override object ReadLong(BufferImpl buffer)
		{
			long value = buffer.ReadLong();
			if (value == long.MaxValue)
			{
				return null;
			}
			return value;
		}

		public override object ReadShort(BufferImpl buffer)
		{
			short value = UnmarshallShort(buffer);
			if (value == short.MaxValue)
			{
				return null;
			}
			return value;
		}

		public static double UnmarshalDouble(BufferImpl buffer)
		{
			return Platform4.LongToDouble(buffer.ReadLong());
		}

		public static float UnmarshallFloat(BufferImpl buffer)
		{
			return Sharpen.Runtime.IntBitsToFloat(buffer.ReadInt());
		}

		public static short UnmarshallShort(BufferImpl buffer)
		{
			int ret = 0;
			for (int i = 0; i < Const4.SHORT_BYTES; i++)
			{
				ret = (ret << 8) + (buffer._buffer[buffer._offset++] & unchecked((int)(0xff)));
			}
			return (short)ret;
		}
	}
}
