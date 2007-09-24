/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class Handlers4
	{
		public const int INT_ID = 1;

		public const int LONG_ID = 2;

		public const int FLOAT_ID = 3;

		public const int BOOLEAN_ID = 4;

		public const int DOUBLE_ID = 5;

		public const int BYTE_ID = 6;

		public const int CHAR_ID = 7;

		public const int SHORT_ID = 8;

		public const int STRING_ID = 9;

		public const int DATE_ID = 10;

		public const int UNTYPED_ID = 11;

		public const int ANY_ARRAY_ID = 12;

		public const int ANY_ARRAY_N_ID = 13;

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
