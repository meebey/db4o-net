using System;
using System.Collections.Generic;
using Db4objects.Db4o.Internal.Collections;
using Db4objects.Db4o.native.Db4objects.Db4o.Typehandlers;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Net;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
    class TypeHandlerConfigurationDotNet : TypeHandlerConfiguration
    {
        public TypeHandlerConfigurationDotNet(Config4Impl config) : base(config)
        {
            ListTypeHandler(new CollectionTypeHandler());
            MapTypeHandler(new MapTypeHandler());
        }

        public override void Apply()
        {
            RegisterCollection(typeof(System.Collections.ArrayList));
            RegisterCollection(typeof (System.Collections.CollectionBase));
            RegisterMap(typeof (System.Collections.Hashtable));
#if ! CF
            RegisterMap(typeof (System.Collections.DictionaryBase));
#endif
            RegisterGenericTypeHandlers();
			RegisterBigSetTypeHandler();
            RegisterSystemArrayTypeHandler();
        }

    	private void RegisterBigSetTypeHandler()
    	{
    		RegisterGenericTypeHandler(typeof(BigSet<>), new BigSetTypeHandler());
    	}

    	private void RegisterGenericTypeHandlers()
        {
			GenericCollectionTypeHandler collectionHandler = new GenericCollectionTypeHandler();
        	RegisterGenericTypeHandler(typeof(List<>), collectionHandler);
			RegisterGenericTypeHandler(typeof(LinkedList<>), collectionHandler);
			RegisterGenericTypeHandler(typeof(Stack<>), collectionHandler);
			RegisterGenericTypeHandler(typeof(Queue<>), collectionHandler);
#if NET_3_5 && ! CF
            RegisterGenericTypeHandler(typeof(HashSet<>), collectionHandler);
            _config.Reflector().RegisterCollection(new GenericCollectionTypePredicate(typeof(HashSet<>)));

#endif 

			System.Type[] dictionaryTypes = new Type[] {
				typeof(Dictionary<,>),
				typeof(SortedList<,>),
#if !CF
				typeof(SortedDictionary<,>),
#endif
			};
            _config.RegisterTypeHandler(new GenericTypeHandlerPredicate(dictionaryTypes), new MapTypeHandler());

        }

    	private void RegisterGenericTypeHandler(Type genericTypeDefinition, ITypeHandler4 handler)
    	{
    		_config.RegisterTypeHandler(new GenericTypeHandlerPredicate(genericTypeDefinition), handler);
    	}

    	internal class GenericTypeHandlerPredicate : ITypeHandlerPredicate
        {
            private readonly Type[] _genericTypes;

            internal GenericTypeHandlerPredicate(params Type[] genericType)
            {
                _genericTypes = genericType;
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
            	return ((IList<Type>) _genericTypes).Contains(type.GetGenericTypeDefinition());
            }
        }

        private void RegisterSystemArrayTypeHandler()
        {
            _config.RegisterTypeHandler(new SystemArrayPredicate(), new SystemArrayTypeHandler());
        }

        internal class GenericCollectionTypePredicate : IReflectClassPredicate
        {
            private readonly Type _type;

            internal GenericCollectionTypePredicate(Type t)
            {
                _type = t;
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
                return _type == type.GetGenericTypeDefinition();
            }
        }

    }
}
