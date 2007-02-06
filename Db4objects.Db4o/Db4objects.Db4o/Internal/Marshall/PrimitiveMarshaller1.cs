namespace Db4objects.Db4o.Internal.Marshall
{
	public class PrimitiveMarshaller1 : Db4objects.Db4o.Internal.Marshall.PrimitiveMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

		public override int WriteNew(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.PrimitiveFieldHandler
			 yapClassPrimitive, object obj, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer
			 writer, bool withIndirection, bool restoreLinkOffset)
		{
			if (obj != null)
			{
				Db4objects.Db4o.Internal.ITypeHandler4 handler = yapClassPrimitive.i_handler;
				handler.WriteNew(_family, obj, topLevel, writer, withIndirection, restoreLinkOffset
					);
			}
			return 0;
		}

		public override Sharpen.Util.Date ReadDate(Db4objects.Db4o.Internal.Buffer bytes)
		{
			return new Sharpen.Util.Date(Db4objects.Db4o.Internal.Handlers.LongHandler.ReadLong
				(bytes));
		}
	}
}
