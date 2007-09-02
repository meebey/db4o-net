/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Handlers4
	{
		public static bool HandlerCanHold(ITypeHandler4 handler, IReflectClass claxx)
		{
			ITypeHandler4 baseTypeHandler = BaseTypeHandler(handler);
			if (Handlers4.HandlesSimple(baseTypeHandler))
			{
				return claxx.Equals(baseTypeHandler.ClassReflector());
			}
			if (baseTypeHandler is UntypedFieldHandler)
			{
				return true;
			}
			ClassMetadata classMetadata = (ClassMetadata)baseTypeHandler;
			IReflectClass classReflector = classMetadata.ClassReflector();
			if (classReflector.IsCollection())
			{
				return true;
			}
			return classReflector.IsAssignableFrom(claxx);
		}

		public static bool HandlesSimple(ITypeHandler4 handler)
		{
			ITypeHandler4 baseTypeHandler = BaseTypeHandler(handler);
			return (baseTypeHandler is PrimitiveHandler) || (baseTypeHandler is StringHandler
				);
		}

		public static bool HandlesClass(ITypeHandler4 handler)
		{
			return BaseTypeHandler(handler) is ClassMetadata;
		}

		public static IReflectClass PrimitiveClassReflector(ITypeHandler4 handler)
		{
			ITypeHandler4 baseTypeHandler = BaseTypeHandler(handler);
			if (baseTypeHandler is PrimitiveHandler)
			{
				return ((PrimitiveHandler)baseTypeHandler).PrimitiveClassReflector();
			}
			return null;
		}

		public static ITypeHandler4 BaseTypeHandler(ITypeHandler4 handler)
		{
			if (handler is ArrayHandler)
			{
				return ((ArrayHandler)handler)._handler;
			}
			if (handler is PrimitiveFieldHandler)
			{
				return ((PrimitiveFieldHandler)handler).i_handler;
			}
			return handler;
		}
	}
}
