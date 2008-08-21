using System;
using System.Collections.Generic;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
    class TypeHandlerConfigurationDotNet : TypeHandlerConfiguration
    {
        public TypeHandlerConfigurationDotNet(Config4Impl config) : base(config)
        {
            ListTypeHandler(new ListTypeHandler());
            MapTypeHandler(new MapTypeHandler());
        }


        public override void Apply()
        {
            RegisterCollection(typeof(System.Collections.ArrayList));
            RegisterGenericTypeHandlers();
        }

        private void RegisterGenericTypeHandlers()
        {
			GenericCollectionTypeHandler collectionHandler = new GenericCollectionTypeHandler();
			_config.RegisterTypeHandler(new GenericTypeHandlerPredicate(typeof(List<>)), collectionHandler);
			_config.RegisterTypeHandler(new GenericTypeHandlerPredicate(typeof(LinkedList<>)), collectionHandler);
			_config.RegisterTypeHandler(new GenericTypeHandlerPredicate(typeof(Dictionary<,>)), new MapTypeHandler());
		}

        internal class GenericTypeHandlerPredicate : ITypeHandlerPredicate
        {
            private Type _genericType;

            internal GenericTypeHandlerPredicate(Type genericType)
            {
                _genericType = genericType;
            }

            public bool Match(IReflectClass classReflector)
            {
                Type type = NetReflector.ToNative(classReflector);
                if (type == null)
                {
                    return false;
                }
                if (!type.IsGenericType)
                {
                    return false;
                }
                return type.GetGenericTypeDefinition() == _genericType;
            }
        }
    }
}
