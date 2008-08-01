/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class CollectionTypeHandlerRegistry
	{
		private readonly Config4Impl _config;

		private readonly ITypeHandler4 _listTypeHandler;

		public CollectionTypeHandlerRegistry(Config4Impl config, ITypeHandler4 listTypeHandler
			)
		{
			_config = config;
			_listTypeHandler = listTypeHandler;
		}

		public static bool Enabled()
		{
			return NullableArrayHandling.Enabled();
		}

		public virtual void RegisterCollection(Type clazz)
		{
			if (!Enabled())
			{
				return;
			}
			RegisterListTypeHandlerFor(clazz);
		}

		public virtual void IgnoreFieldsOn(Type clazz)
		{
			if (!Enabled())
			{
				return;
			}
			_config.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(clazz), new IgnoreFieldsTypeHandler
				());
		}

		private void RegisterListTypeHandlerFor(Type clazz)
		{
			_config.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(clazz), _listTypeHandler
				);
		}
	}
}
