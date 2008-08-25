/* Copyright (C) 2008   db4objects Inc.   http://www.db4o.com */
using System.Collections.Generic;
using Db4objects.Db4o.native.Db4objects.Db4o.Internal;
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
    }
}
