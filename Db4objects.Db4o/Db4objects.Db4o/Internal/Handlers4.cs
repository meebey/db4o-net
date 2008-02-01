/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Handlers4
	{
		public const int IntId = 1;

		public const int LongId = 2;

		public const int FloatId = 3;

		public const int BooleanId = 4;

		public const int DoubleId = 5;

		public const int ByteId = 6;

		public const int CharId = 7;

		public const int ShortId = 8;

		public const int StringId = 9;

		public const int DateId = 10;

		public const int UntypedId = 11;

		public const int AnyArrayId = 12;

		public const int AnyArrayNId = 13;

		public static bool HandlerCanHold(ITypeHandler4 handler, IReflectClass claxx)
		{
			ITypeHandler4 baseTypeHandler = BaseTypeHandler(handler);
			if (HandlesSimple(baseTypeHandler))
			{
				return claxx.Equals(((IBuiltinTypeHandler)baseTypeHandler).ClassReflector());
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
				) || (baseTypeHandler is ISecondClassTypeHandler);
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
				return ((PrimitiveFieldHandler)handler).TypeHandler();
			}
			return handler;
		}

		public static IReflectClass BaseType(IReflectClass clazz)
		{
			if (clazz.IsArray())
			{
				return BaseType(clazz.GetComponentType());
			}
			return clazz;
		}
	}
}
