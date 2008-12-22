/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Activation;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Marshall;
using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Typehandlers
{
    public class EnumTypeHandler : IEmbeddedTypeHandler, ITypeFamilyTypeHandler
    {
        private class PreparedEnumComparison : IPreparedComparison
        {
            private readonly long _enumValue;
            public PreparedEnumComparison(object obj)
            {
                if (obj is TransactionContext)
                {
                    obj = ((TransactionContext)obj)._object;
                }

                if (obj == null) return;
                _enumValue = Convert.ToInt64(obj);
            }

            public int CompareTo(object obj)
            {
                if (obj is TransactionContext)
                {
                    obj = ((TransactionContext)obj)._object;
                }

                if (obj == null) return 1;

                long other = Convert.ToInt64(obj);
                if (_enumValue == other) return 0;
                if (_enumValue < other) return -1;

                return 1;
            }
        }

        public IPreparedComparison PrepareComparison(IContext context, object obj)
        {
            return new PreparedEnumComparison(obj);
        }

        public void Delete(IDeleteContext context)
        {
            int offset = context.Offset() + Const4.IdLength + Const4.LongLength;
            context.Seek(offset);
        }

        public void Defragment(IDefragmentContext context)
        {
            context.CopyID();
            context.IncrementOffset(Const4.LongLength);
        }

        public object Read(IReadContext context)
        {
            int classId = context.ReadInt();
            ClassMetadata clazz = Container(context).ClassMetadataForId(classId);

            Type enumType = NetReflector.ToNative(clazz.ClassReflector());
            long enumValue = context.ReadLong();
            return Enum.ToObject(enumType, enumValue); ;
        }

        public void Write(IWriteContext context, object obj)
        {
            int classId = ClassMetadataIdFor(context, obj);

            context.WriteInt(classId);
            context.WriteLong(Convert.ToInt64(obj));
        }

        public bool CanHold(IReflectClass claxx)
        {
            return NetReflector.ToNative(claxx).IsEnum;
        }

        public bool IsSimple()
        {
            return true;
        }

        public int LinkLength()
        {
            return Const4.IdLength + Const4.LongLength;
        }

        private static int ClassMetadataIdFor(IContext context, object obj)
        {
            IReflectClass claxx = Container(context).Reflector().ForObject(obj);
            ClassMetadata clazz = Container(context).ProduceClassMetadata(claxx);

            //TODO: Handle clazz == null!! Must not happen!

            return clazz.GetID();
        }

        private static ITypeHandler4 StringTypeHandler(IContext context)
        {
            return Container(context).Handlers().TypeHandlerForClass(Container(context).Ext().Reflector().ForClass(typeof(string)));
        }

        private static ObjectContainerBase Container(IContext context)
        {
            return ((IInternalObjectContainer)context.ObjectContainer()).Container();
        }

    }

    public class EnumTypeHandlerPredicate : ITypeHandlerPredicate
    {
        public bool Match(IReflectClass classReflector)
        {
            Type type = NetReflector.ToNative(classReflector);
            if(type == null)
            {
                return false;
            }
            return type.IsEnum;
        }
    }
}
