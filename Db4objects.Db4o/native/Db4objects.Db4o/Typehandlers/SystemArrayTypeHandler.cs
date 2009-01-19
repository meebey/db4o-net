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
using Db4objects.Db4o.Reflect.Net;
using Db4objects.Db4o.Typehandlers;
using Db4objects.Db4o.Internal.Handlers.Array;


namespace Db4objects.Db4o.native.Db4objects.Db4o.Typehandlers
{
    public class SystemArrayTypeHandler : IFirstClassHandler, ICanHoldAnythingHandler, IVariableLengthTypeHandler, IEmbeddedTypeHandler
    {

        public virtual IPreparedComparison PrepareComparison(IContext context, object obj)
        {
            return readArrayHandler(context).PrepareComparison(context, obj);
        }

        public virtual void Write(IWriteContext context, object obj)
        {
            Array collection = (Array) obj;
            ITypeHandler4 elementHandler = DetectElementTypeHandler(Container(context), collection);
            WriteElementTypeHandlerId(context, elementHandler);
            new ArrayHandler(elementHandler, false).Write(context, obj);
        }

        public virtual object Read(IReadContext context)
        {
            return readArrayHandler(context).Read(context);
        }

        private ArrayHandler readArrayHandler(IContext context)
        {
            ITypeHandler4 handler = ReadElementTypeHandler((IReadBuffer)context, context);
            return new ArrayHandler(handler, false);
        }

        public virtual void Delete(IDeleteContext context)
        {
            readArrayHandler(context).Delete(context);
        }

        public virtual void Defragment(IDefragmentContext context)
        {
            DefragmentElementHandlerId(context);
            readArrayHandler(context).Defragment(context);
        }

        public void CascadeActivation(ActivationContext4 context)
        {
            ICollection collection = ((ICollection)context.TargetObject());
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
            readArrayHandler(context).CollectIDs(context);
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

        private static void WriteElementTypeHandlerId(IWriteContext context, ITypeHandler4 elementHandler)
        {
            int id = IsUntypedField(elementHandler) ? 0 : Container(context).Handlers().TypeHandlerID(elementHandler);
            context.WriteInt(id);
        }

        private static bool IsUntypedField(ITypeHandler4 elementHandler)
        {
            return (elementHandler is UntypedFieldHandler);
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

        private static ITypeHandler4 DetectElementTypeHandler(ObjectContainerBase container, Array collection)
        {
            Type elementType = ElementTypeOf(collection);
            if (IsNullableInstance(elementType))
            {
                return container.Handlers().UntypedObjectHandler();
            }

            ITypeHandler4 elementHandler = (ITypeHandler4)container.FieldHandlerForClass(ReflectClassFor(container, elementType));
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

        private static Type ElementTypeOf(Array array)
        {
            return array.GetType().GetElementType();
        }

    }

    internal class SystemArrayPredicate : ITypeHandlerPredicate
    {
        public bool Match(IReflectClass classReflector)
        {
            if(classReflector == null)
            {
                return false;
            }
            Type type = NetReflector.ToNative(classReflector);
            return type == typeof(System.Array);
        }
    }

}
