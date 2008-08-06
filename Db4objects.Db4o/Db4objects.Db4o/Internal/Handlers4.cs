/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Handlers.Array;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

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

		public static ITypeHandler4 CorrectHandlerVersion(IHandlerVersionContext context, 
			ITypeHandler4 handler)
		{
			int version = context.HandlerVersion();
			if (version >= HandlerRegistry.HandlerVersion)
			{
				return handler;
			}
			return context.Transaction().Container().Handlers().CorrectHandlerVersion(handler
				, version);
		}

		public static bool HandlerCanHold(ITypeHandler4 handler, IReflector reflector, IReflectClass
			 claxx)
		{
			ITypeHandler4 baseTypeHandler = BaseTypeHandler(handler);
			if (HandlesSimple(baseTypeHandler))
			{
				if (NullableArrayHandling.Enabled())
				{
					if (baseTypeHandler is PrimitiveHandler)
					{
						return claxx.Equals(((IBuiltinTypeHandler)baseTypeHandler).ClassReflector()) || claxx
							.Equals(((PrimitiveHandler)baseTypeHandler).PrimitiveClassReflector());
					}
				}
				return claxx.Equals(((IBuiltinTypeHandler)baseTypeHandler).ClassReflector());
			}
			if (baseTypeHandler is UntypedFieldHandler)
			{
				return true;
			}
			if (handler is ICanHoldAnythingHandler)
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
			return BaseTypeHandler(handler) is IFirstClassHandler;
		}

		public static IReflectClass PrimitiveClassReflector(ITypeHandler4 handler, IReflector
			 reflector)
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
				return ((ArrayHandler)handler).DelegateTypeHandler();
			}
			if (handler is PrimitiveFieldHandler)
			{
				return ((PrimitiveFieldHandler)handler).TypeHandler();
			}
			return handler;
		}

		public static IReflectClass BaseType(IReflectClass clazz)
		{
			if (clazz == null)
			{
				return null;
			}
			if (clazz.IsArray())
			{
				return BaseType(clazz.GetComponentType());
			}
			return clazz;
		}
	}
}
