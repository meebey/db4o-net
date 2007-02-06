namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class PrimitiveMarshaller
	{
		public Db4objects.Db4o.Internal.Marshall.MarshallerFamily _family;

		public abstract bool UseNormalClassRead();

		public abstract int WriteNew(Db4objects.Db4o.Internal.Transaction trans, Db4objects.Db4o.Internal.PrimitiveFieldHandler
			 yapClassPrimitive, object obj, bool topLevel, Db4objects.Db4o.Internal.StatefulBuffer
			 parentWriter, bool withIndirection, bool restoreLinkOffset);

		public abstract Sharpen.Util.Date ReadDate(Db4objects.Db4o.Internal.Buffer bytes);

		protected int ObjectLength(Db4objects.Db4o.Internal.ITypeHandler4 handler)
		{
			return handler.LinkLength() + Db4objects.Db4o.Internal.Const4.OBJECT_LENGTH + Db4objects.Db4o.Internal.Const4
				.ID_LENGTH;
		}
	}
}
