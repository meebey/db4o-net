namespace Db4objects.Db4o.Inside.Marshall
{
	public class PrimitiveMarshaller1 : Db4objects.Db4o.Inside.Marshall.PrimitiveMarshaller
	{
		public override bool UseNormalClassRead()
		{
			return false;
		}

		public override int WriteNew(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClassPrimitive
			 yapClassPrimitive, object obj, bool topLevel, Db4objects.Db4o.YapWriter writer, 
			bool withIndirection, bool restoreLinkOffset)
		{
			if (obj != null)
			{
				Db4objects.Db4o.ITypeHandler4 handler = yapClassPrimitive.i_handler;
				handler.WriteNew(_family, obj, topLevel, writer, withIndirection, restoreLinkOffset
					);
			}
			return 0;
		}
	}
}
