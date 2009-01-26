/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Foundation.Collections;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Typehandlers
{

	public class GenericCollectionTypeHandler : IFirstClassHandler, ICanHoldAnythingHandler, IVariableLengthTypeHandler
	{
		public virtual IPreparedComparison PrepareComparison(IContext context, object obj)
		{
			return null;
		}

		public virtual void Write(IWriteContext context, object obj)
		{
            ICollectionInitializer initializer = CollectionInitializer.For(obj);
            IEnumerable enumerable = (IEnumerable)obj;
			ITypeHandler4 elementHandler = DetectElementTypeHandler(Container(context), enumerable);
			WriteElementTypeHandlerId(context, elementHandler);
			WriteElementCount(context, initializer);
			WriteElements(context, enumerable, elementHandler);
		}

		public virtual object Read(IReadContext context)
		{
			object collection = ((UnmarshallingContext)context).PersistentObject();
			ICollectionInitializer initializer = CollectionInitializer.For(collection);
			initializer.Clear();

			ReadElements(context, initializer, ReadElementTypeHandler(context, context));

			initializer.FinishAdding();

			return collection;
		}

		public virtual void Delete(IDeleteContext context)
		{
			if (!context.CascadeDelete()) return;

			ITypeHandler4 handler = ReadElementTypeHandler(context, context);
			int elementCount = context.ReadInt();
			for (int i = elementCount; i > 0; i--)
			{
				handler.Delete(context);
			}
		}

		public virtual void Defragment(IDefragmentContext context)
		{
			DefragmentElementHandlerId(context);
			ITypeHandler4 handler = ReadElementTypeHandler(context, context);
			int elementCount = context.ReadInt();
			for (int i = 0; i < elementCount; i++)
			{
				context.Defragment(handler);
			}
		}

		public void CascadeActivation(ActivationContext4 context)
		{
            IEnumerable collection = ((IEnumerable)context.TargetObject());
			foreach (object item in collection)
			{
				context.CascadeActivationToChild(item);
			}
		}

		public virtual ITypeHandler4 ReadCandidateHandler(QueryingReadContext context)
		{
			return this;
		}


		public virtual void CollectIDs(QueryingReadContext context)
		{
			ITypeHandler4 elementHandler = ReadElementTypeHandler(context, context);
			int elementCount = context.ReadInt();
			for (int i = 0; i < elementCount; i++)
			{
				context.ReadId(elementHandler);
			}
		}

		private static void DefragmentElementHandlerId(IDefragmentContext context)
		{
			int offset = context.Offset();
			context.CopyID();
			context.Seek(offset);
		}

		private static ITypeHandler4 UntypedObjectHandlerFrom(IContext context)
		{
			return context.Transaction().Container().Handlers().UntypedObjectHandler();
		}

		private static void ReadElements(IReadContext context, ICollectionInitializer initializer, ITypeHandler4 elementHandler)
		{
			int elementCount = context.ReadInt();
			for (int i = 0; i < elementCount; i++)
			{
				initializer.Add(context.ReadObject(elementHandler));
			}
		}

		private static void WriteElementTypeHandlerId(IWriteContext context, ITypeHandler4 elementHandler)
		{
			int id = IsUntypedField(elementHandler) ? 0 : Container(context).Handlers().TypeHandlerID(elementHandler);
			context.WriteInt(id);
		}

		private static bool IsUntypedField(ITypeHandler4 elementHandler)
		{
			return (elementHandler is UntypedFieldHandler);
		}

        private static void WriteElementCount(IWriteBuffer context, ICollectionInitializer initializer)
		{
            context.WriteInt(initializer.Count());
		}

		private static void WriteElements(IWriteContext context, IEnumerable enumerable, ITypeHandler4 elementHandler)
		{
			IEnumerator elements = enumerable.GetEnumerator();
			while (elements.MoveNext())
			{
				context.WriteObject(elementHandler, elements.Current);
			}
		}

		private static ObjectContainerBase Container(IContext context)
		{
			return ((IInternalObjectContainer)context.ObjectContainer()).Container();
		}

		private static ITypeHandler4 ReadElementTypeHandler(IReadBuffer buffer, IContext context)
		{
			int elementHandlerId = buffer.ReadInt();
			if (elementHandlerId == 0) return UntypedObjectHandlerFrom(context);

			ITypeHandler4 elementHandler = (ITypeHandler4)Container(context).FieldHandlerForId(elementHandlerId);
			return elementHandler ?? UntypedObjectHandlerFrom(context);
		}

		private static ITypeHandler4 DetectElementTypeHandler(ObjectContainerBase container, IEnumerable collection)
		{
			Type elementType = ElementTypeOf(collection);
			if (IsNullableInstance(elementType))
			{
				return container.Handlers().UntypedObjectHandler();
			}

			ITypeHandler4 elementHandler = (ITypeHandler4) container.FieldHandlerForClass(ReflectClassFor(container, elementType));
			return elementHandler ?? container.Handlers().UntypedObjectHandler();
		}

		private static IReflectClass ReflectClassFor(ObjectContainerBase container, Type elementType)
		{
			return container.Reflector().ForClass(elementType);
		}

		private static bool IsNullableInstance(Type elementType)
		{
			return elementType.IsGenericType && (elementType.GetGenericTypeDefinition() == typeof(Nullable<>));
		}

		private static Type ElementTypeOf(IEnumerable collection)
		{
			return collection.GetType().GetGenericArguments()[0];
		}
	}

}
