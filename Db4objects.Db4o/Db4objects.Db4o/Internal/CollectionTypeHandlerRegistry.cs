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

		private ITypeHandler4 _listTypeHandler;

		private ITypeHandler4 _mapTypeHandler;

		public CollectionTypeHandlerRegistry(Config4Impl config)
		{
			_config = config;
		}

		public virtual void ListTypeHandler(ITypeHandler4 listTypeHandler)
		{
			_listTypeHandler = listTypeHandler;
		}

		public virtual void MapTypeHandler(ITypeHandler4 mapTypehandler)
		{
			_mapTypeHandler = mapTypehandler;
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

		public virtual void RegisterMap(Type clazz)
		{
			if (!Enabled())
			{
				return;
			}
			RegisterMapTypeHandlerFor(clazz);
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
			RegisterTypeHandlerFor(_listTypeHandler, clazz);
		}

		private void RegisterMapTypeHandlerFor(Type clazz)
		{
			RegisterTypeHandlerFor(_mapTypeHandler, clazz);
		}

		private void RegisterTypeHandlerFor(ITypeHandler4 typeHandler, Type clazz)
		{
			_config.RegisterTypeHandler(new SingleClassTypeHandlerPredicate(clazz), typeHandler
				);
		}
	}
}
