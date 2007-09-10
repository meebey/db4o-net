/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Sharpen.Util;

namespace Db4objects.Db4o.Internal.Marshall
{
	public abstract class PrimitiveMarshaller
	{
		public MarshallerFamily _family;

		public abstract bool UseNormalClassRead();

		public abstract Date ReadDate(Db4objects.Db4o.Internal.Buffer bytes);

		public abstract object ReadShort(Db4objects.Db4o.Internal.Buffer buffer);

		public abstract object ReadInteger(Db4objects.Db4o.Internal.Buffer buffer);

		public abstract object ReadFloat(Db4objects.Db4o.Internal.Buffer buffer);

		public abstract object ReadDouble(Db4objects.Db4o.Internal.Buffer buffer);

		public abstract object ReadLong(Db4objects.Db4o.Internal.Buffer buffer);

		protected int ObjectLength(ITypeHandler4 handler)
		{
			return handler.LinkLength() + Const4.OBJECT_LENGTH + Const4.ID_LENGTH;
		}
	}
}
