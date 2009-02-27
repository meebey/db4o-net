/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Handlers.Array;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
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

		public static bool HandlerCanHold(ITypeHandler4 handler, IReflector reflector, IReflectClass
			 claxx)
		{
			return handler.CanHold(claxx);
		}

		public static bool HandlesSimple(ITypeHandler4 handler)
		{
			ITypeHandler4 baseTypeHandler = BaseTypeHandler(handler);
			if (!(baseTypeHandler is IQueryableTypeHandler))
			{
				return false;
			}
			return ((IQueryableTypeHandler)baseTypeHandler).IsSimple();
		}

		public static bool HandlesArray(ITypeHandler4 handler)
		{
			return handler is ArrayHandler;
		}

		public static bool HandlesMultidimensionalArray(ITypeHandler4 handler)
		{
			return handler is MultidimensionalArrayHandler;
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

		public static bool IsClassAware(ITypeHandler4 typeHandler)
		{
			return typeHandler is IBuiltinTypeHandler || typeHandler is ClassMetadata || typeHandler
				 is PlainObjectHandler;
		}

		public static int CalculateLinkLength(ITypeHandler4 _handler)
		{
			if (_handler == null)
			{
				// must be ClassMetadata
				return Const4.IdLength;
			}
			if (_handler is ITypeFamilyTypeHandler)
			{
				return ((ITypeFamilyTypeHandler)_handler).LinkLength();
			}
			if (_handler is PersistentBase)
			{
				return ((PersistentBase)_handler).LinkLength();
			}
			if (_handler is PrimitiveHandler)
			{
				return ((PrimitiveHandler)_handler).LinkLength();
			}
			if (_handler is IVariableLengthTypeHandler)
			{
				if (_handler is IEmbeddedTypeHandler)
				{
					return Const4.IndirectionLength;
				}
				return Const4.IdLength;
			}
			// TODO: For custom handlers there will have to be a way 
			//       to calculate the length in the slot.
			//        Options:
			//        (1) Remember when the first object is marshalled.
			//        (2) Add a #defaultValue() method to TypeHandler4,
			//            marshall the default value and check.
			//        (3) Add a way to test the custom handler when it
			//            is installed and remember the length there. 
			throw new NotImplementedException();
		}

		public static IReflectClass ClassReflectorForHandler(HandlerRegistry handlerRegistry
			, ITypeHandler4 handler)
		{
			if (handler is IBuiltinTypeHandler)
			{
				return ((IBuiltinTypeHandler)handler).ClassReflector();
			}
			if (handler is ClassMetadata)
			{
				return ((ClassMetadata)handler).ClassReflector();
			}
			return handlerRegistry.ClassReflectorForHandler(handler);
		}

		public static bool HoldsEmbedded(ITypeHandler4 handler)
		{
			return IsEmbedded(BaseTypeHandler(handler));
		}

		public static bool IsClassMetadata(ITypeHandler4 handler)
		{
			return handler is ClassMetadata;
		}

		public static bool IsEmbedded(ITypeHandler4 handler)
		{
			return handler is IEmbeddedTypeHandler;
		}

		public static bool IsFirstClass(ITypeHandler4 handler)
		{
			return handler is IFirstClassHandler;
		}

		public static bool IsPrimitive(ITypeHandler4 handler)
		{
			return handler is PrimitiveHandler;
		}

		public static bool IsUntyped(ITypeHandler4 handler)
		{
			return handler is UntypedFieldHandler;
		}

		public static bool IsVariableLength(ITypeHandler4 handler)
		{
			return handler is IVariableLengthTypeHandler;
		}

		public static IFieldAwareTypeHandler FieldAwareTypeHandler(ITypeHandler4 typeHandler
			)
		{
			if (typeHandler is IFieldAwareTypeHandler)
			{
				return (IFieldAwareTypeHandler)typeHandler;
			}
			return NullFieldAwareTypeHandler.Instance;
		}

		public static void CollectIDs(QueryingReadContext context, ITypeHandler4 typeHandler
			)
		{
			if (typeHandler is IFirstClassHandler)
			{
				((IFirstClassHandler)typeHandler).CollectIDs(context);
			}
		}

		public static bool UseDedicatedSlot(IContext context, ITypeHandler4 handler)
		{
			if (handler is IEmbeddedTypeHandler)
			{
				return false;
			}
			if (handler is UntypedFieldHandler)
			{
				return false;
			}
			if (handler is ClassMetadata)
			{
				return UseDedicatedSlot(context, ((ClassMetadata)handler).DelegateTypeHandler(context
					));
			}
			return true;
		}

		public static ITypeHandler4 ArrayElementHandler(ITypeHandler4 handler, QueryingReadContext
			 queryingReadContext)
		{
			if (!(handler is IFirstClassHandler))
			{
				return null;
			}
			IFirstClassHandler firstClassHandler = (IFirstClassHandler)HandlerRegistry.CorrectHandlerVersion
				(queryingReadContext, handler);
			return HandlerRegistry.CorrectHandlerVersion(queryingReadContext, firstClassHandler
				.ReadCandidateHandler(queryingReadContext));
		}

		public static object NullRepresentationInUntypedArrays(ITypeHandler4 handler)
		{
			if (handler is PrimitiveHandler)
			{
				return ((PrimitiveHandler)handler).NullRepresentationInUntypedArrays();
			}
			return null;
		}

		public static bool HandleAsObject(ITypeHandler4 typeHandler)
		{
			if (IsEmbedded(typeHandler))
			{
				return false;
			}
			if (typeHandler is UntypedFieldHandler)
			{
				return false;
			}
			return true;
		}

		public static void CascadeActivation(ActivationContext4 context, ITypeHandler4 handler
			)
		{
			if (!(handler is IFirstClassHandler))
			{
				return;
			}
			((IFirstClassHandler)handler).CascadeActivation(context);
		}

		public static bool HandlesPrimitiveArray(ITypeHandler4 classMetadata)
		{
			return classMetadata is PrimitiveFieldHandler && ((PrimitiveFieldHandler)classMetadata
				).IsArray();
		}

		public static bool HasClassIndex(ITypeHandler4 typeHandler)
		{
			if (typeHandler is ClassMetadata)
			{
				return ((ClassMetadata)typeHandler).HasClassIndex();
			}
			return false;
		}

		public static bool CanLoadFieldByIndex(ITypeHandler4 handler)
		{
			if (handler is ClassMetadata)
			{
				ClassMetadata yc = (ClassMetadata)handler;
				if (yc.IsArray())
				{
					return false;
				}
			}
			return true;
		}

		public static object WrapWithTransactionContext(Transaction transaction, object value
			, ITypeHandler4 handler)
		{
			if (handler is ClassMetadata)
			{
				value = ((ClassMetadata)handler).WrapWithTransactionContext(transaction, value);
			}
			return value;
		}

		public static void CollectIdsInternal(CollectIdContext context, ITypeHandler4 handler
			, int linkLength)
		{
			if (!(IsFirstClass(handler)))
			{
				IReadBuffer buffer = context.Buffer();
				buffer.Seek(buffer.Offset() + linkLength);
				return;
			}
			if (handler.GetType() == typeof(ClassMetadata))
			{
				context.AddId();
				return;
			}
			LocalObjectContainer container = (LocalObjectContainer)context.Container();
			SlotFormat slotFormat = context.SlotFormat();
			if (HandleAsObject(handler))
			{
				// TODO: Code is similar to QCandidate.readArrayCandidates. Try to refactor to one place.
				int collectionID = context.ReadInt();
				ByteArrayBuffer collectionBuffer = container.ReadReaderByID(context.Transaction()
					, collectionID);
				ObjectHeader objectHeader = new ObjectHeader(container, collectionBuffer);
				QueryingReadContext subContext = new QueryingReadContext(context.Transaction(), context
					.HandlerVersion(), collectionBuffer, collectionID, context.Collector());
				objectHeader.ClassMetadata().CollectIDs(subContext);
				return;
			}
			QueryingReadContext queryingReadContext = new QueryingReadContext(context.Transaction
				(), context.HandlerVersion(), context.Buffer(), 0, context.Collector());
			slotFormat.DoWithSlotIndirection(queryingReadContext, handler, new _IClosure4_291
				(handler, queryingReadContext));
		}

		private sealed class _IClosure4_291 : IClosure4
		{
			public _IClosure4_291(ITypeHandler4 handler, QueryingReadContext queryingReadContext
				)
			{
				this.handler = handler;
				this.queryingReadContext = queryingReadContext;
			}

			public object Run()
			{
				((IFirstClassHandler)handler).CollectIDs(queryingReadContext);
				return null;
			}

			private readonly ITypeHandler4 handler;

			private readonly QueryingReadContext queryingReadContext;
		}
	}
}
