/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public abstract class TypeHandlerConfiguration
	{
		protected readonly Config4Impl _config;

		private ITypeHandler4 _listTypeHandler;

		private ITypeHandler4 _mapTypeHandler;

		public abstract void Apply();

		public TypeHandlerConfiguration(Config4Impl config)
		{
			_config = config;
		}

		protected virtual void ListTypeHandler(ITypeHandler4 listTypeHandler)
		{
			_listTypeHandler = listTypeHandler;
		}

		protected virtual void MapTypeHandler(ITypeHandler4 mapTypehandler)
		{
			_mapTypeHandler = mapTypehandler;
		}

		public static bool Enabled()
		{
			return NullableArrayHandling.Enabled();
		}

		protected virtual void RegisterCollection(Type clazz)
		{
			RegisterListTypeHandlerFor(clazz);
		}

		protected virtual void RegisterMap(Type clazz)
		{
			RegisterMapTypeHandlerFor(clazz);
		}

		protected virtual void IgnoreFieldsOn(Type clazz)
		{
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
