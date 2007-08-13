/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Internal.Slots;
using Sharpen.Util;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class PrimitiveMarshaller0 : PrimitiveMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return true;
		}

		public override int WriteNew(Transaction trans, PrimitiveFieldHandler yapClassPrimitive
			, object obj, bool topLevel, StatefulBuffer parentWriter, bool withIndirection, 
			bool restoreLinkOffset)
		{
			int id = 0;
			if (obj != null)
			{
				ITypeHandler4 handler = yapClassPrimitive.i_handler;
				ObjectContainerBase stream = trans.Container();
				id = stream.NewUserObject();
				Slot slot = new Slot(-1, ObjectLength(handler));
				if (!stream.IsClient())
				{
					slot = ((LocalTransaction)trans).File().GetSlot(slot.Length());
				}
				Pointer4 pointer = new Pointer4(id, slot);
				trans.SetPointer(pointer);
				StatefulBuffer writer = new StatefulBuffer(trans, pointer);
				writer.WriteInt(yapClassPrimitive.GetID());
				handler.Write(_family, obj, false, writer, true, false);
				writer.WriteEnd();
				stream.WriteNew(yapClassPrimitive, writer);
			}
			if (parentWriter != null)
			{
				parentWriter.WriteInt(id);
			}
			return id;
		}

		public override Date ReadDate(Db4objects.Db4o.Internal.Buffer bytes)
		{
			long value = bytes.ReadLong();
			if (value == long.MaxValue)
			{
				return null;
			}
			return new Date(value);
		}

		public override object ReadInteger(Db4objects.Db4o.Internal.Buffer bytes)
		{
			int value = bytes.ReadInt();
			if (value == int.MaxValue)
			{
				return null;
			}
			return value;
		}

		public override object ReadFloat(Db4objects.Db4o.Internal.Buffer bytes)
		{
			float value = UnmarshallFloat(bytes);
			if (float.IsNaN(value))
			{
				return null;
			}
			return value;
		}

		public override object ReadDouble(Db4objects.Db4o.Internal.Buffer buffer)
		{
			double value = UnmarshalDouble(buffer);
			if (double.IsNaN(value))
			{
				return null;
			}
			return value;
		}

		public override object ReadLong(Db4objects.Db4o.Internal.Buffer buffer)
		{
			long value = buffer.ReadLong();
			if (value == long.MaxValue)
			{
				return null;
			}
			return value;
		}

		public override object ReadShort(Db4objects.Db4o.Internal.Buffer buffer)
		{
			short value = UnmarshallShort(buffer);
			if (value == short.MaxValue)
			{
				return null;
			}
			return value;
		}

		public static double UnmarshalDouble(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return Platform4.LongToDouble(buffer.ReadLong());
		}

		public static float UnmarshallFloat(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return Sharpen.Runtime.IntBitsToFloat(buffer.ReadInt());
		}

		public static short UnmarshallShort(Db4objects.Db4o.Internal.Buffer buffer)
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
