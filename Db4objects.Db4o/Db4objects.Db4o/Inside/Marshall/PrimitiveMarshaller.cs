namespace Db4objects.Db4o.Inside.Marshall
{
	public abstract class PrimitiveMarshaller
	{
		public Db4objects.Db4o.Inside.Marshall.MarshallerFamily _family;

		public abstract bool UseNormalClassRead();

		public abstract int WriteNew(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.YapClassPrimitive
			 yapClassPrimitive, object obj, bool topLevel, Db4objects.Db4o.YapWriter parentWriter
			, bool withIndirection, bool restoreLinkOffset);

		protected int ObjectLength(Db4objects.Db4o.ITypeHandler4 handler)
		{
			return handler.LinkLength() + Db4objects.Db4o.YapConst.OBJECT_LENGTH + Db4objects.Db4o.YapConst
				.ID_LENGTH;
		}
	}
}
