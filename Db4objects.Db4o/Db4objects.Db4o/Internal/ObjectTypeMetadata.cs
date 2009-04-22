/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	public class ObjectTypeMetadata : Db4objects.Db4o.Internal.PrimitiveTypeMetadata
	{
		public ObjectTypeMetadata(ObjectContainerBase container, ITypeHandler4 handler, int
			 id, IReflectClass classReflector) : base(container, handler, id, classReflector
			)
		{
		}

		public override object Instantiate(UnmarshallingContext context)
		{
			object @object = new object();
			OnInstantiate(context, @object);
			return @object;
		}
	}
}
