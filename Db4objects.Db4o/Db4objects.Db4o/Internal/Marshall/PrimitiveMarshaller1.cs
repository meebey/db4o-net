using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Sharpen.Util;

namespace Db4objects.Db4o.Internal.Marshall
{
	public class PrimitiveMarshaller1 : PrimitiveMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

		public override int WriteNew(Transaction trans, PrimitiveFieldHandler yapClassPrimitive
			, object obj, bool topLevel, StatefulBuffer writer, bool withIndirection, bool restoreLinkOffset
			)
		{
			if (obj != null)
			{
				ITypeHandler4 handler = yapClassPrimitive.i_handler;
				handler.WriteNew(_family, obj, topLevel, writer, withIndirection, restoreLinkOffset
					);
			}
			return 0;
		}

		public override Date ReadDate(Db4objects.Db4o.Internal.Buffer bytes)
		{
			return new Date(bytes.ReadLong());
		}

		public override object ReadInteger(Db4objects.Db4o.Internal.Buffer bytes)
		{
			return bytes.ReadInt();
		}

		public override object ReadFloat(Db4objects.Db4o.Internal.Buffer bytes)
		{
			return PrimitiveMarshaller0.UnmarshallFloat(bytes);
		}

		public override object ReadDouble(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return PrimitiveMarshaller0.UnmarshalDouble(buffer);
		}

		public override object ReadLong(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return buffer.ReadLong();
		}

		public override object ReadShort(Db4objects.Db4o.Internal.Buffer buffer)
		{
			return PrimitiveMarshaller0.UnmarshallShort(buffer);
		}
	}
}
