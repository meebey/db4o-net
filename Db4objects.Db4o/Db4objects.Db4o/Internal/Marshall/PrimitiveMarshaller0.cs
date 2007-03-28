namespace Db4objects.Db4o.Internal.Marshall
{
	public class PrimitiveMarshaller0 : Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return true;
		}

		public override int WriteNew(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.PrimitiveFieldHandler
			 yapClassPrimitive, object obj, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer
			 parentWriter, bool withIndirection, bool restoreLinkOffset)
		{
			int id = 0;
			if (obj != null)
			{
				Db4objects.Db4o.Internal.ITypeHandler4 handler = yapClassPrimitive.i_handler;
				Db4objects.Db4o.Internal.ObjectContainerBase stream = trans.Stream();
				id = stream.NewUserObject();
				int address = -1;
				int length = ObjectLength(handler);
				if (!stream.IsClient())
				{
					address = ((Db4objects.Db4o.Internal.LocalTransaction)trans).File().GetSlot(length
						);
				}
				trans.SetPointer(id, address, length);
				Db4objects.Db4o.Internal.StatefulBuffer writer = new Db4objects.Db4o.Internal.StatefulBuffer
					(trans, length);
				writer.UseSlot(id, address, length);
				writer.WriteInt(yapClassPrimitive.GetID());
				handler.WriteNew(_family, obj, false, writer, true, false);
				writer.WriteEnd();
				stream.WriteNew(yapClassPrimitive, writer);
			}
			if (parentWriter != null)
			{
				parentWriter.WriteInt(id);
			}
			return id;
		}

		public override Sharpen.Util.Date ReadDate(Db4objects.Db4o.Internal.Buffer bytes)
		{
			long value = bytes.ReadLong();
			if (value == long.MaxValue)
			{
				return null;
			}
			return new Sharpen.Util.Date(value);
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
			return Db4objects.Db4o.Internal.Platform4.LongToDouble(buffer.ReadLong());
		}

		public static float UnmarshallFloat(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return Sharpen.Runtime.IntBitsToFloat(buffer.ReadInt());
		}

		public static short UnmarshallShort(Db4objects.Db4o.Internal.Buffer buffer)
		{
			int ret = 0;
			for (int i = 0; i < Db4objects.Db4o.Internal.Const4.SHORT_BYTES; i++)
			{
				ret = (ret << 8) + (buffer._buffer[buffer._offset++] & unchecked((int)(0xff)));
			}
			return (short)ret;
		}
	}
}
