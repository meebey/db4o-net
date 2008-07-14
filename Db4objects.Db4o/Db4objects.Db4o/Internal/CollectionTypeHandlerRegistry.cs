/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Internal
{
	/// <exclude></exclude>
	public class CollectionTypeHandlerRegistry
	{
		private const int InstalledFromVersion = 4;

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

		public virtual void RegisterLists(Type[] classes)
		{
			if (!Enabled())
			{
				return;
			}
			for (int i = 0; i < classes.Length; i++)
			{
				RegisterListTypeHandler(classes[i]);
			}
		}

		private void RegisterListTypeHandler(Type clazz)
		{
			IReflectClass claxx = _config.Reflector().ForClass(clazz);
			_config.RegisterTypeHandler(new _ITypeHandlerPredicate_43(claxx), _listTypeHandler
				);
		}

		private sealed class _ITypeHandlerPredicate_43 : ITypeHandlerPredicate
		{
			public _ITypeHandlerPredicate_43(IReflectClass claxx)
			{
				this.claxx = claxx;
			}

			public bool Match(IReflectClass classReflector, int version)
			{
				if (version < Db4objects.Db4o.Internal.CollectionTypeHandlerRegistry.InstalledFromVersion
					)
				{
					return false;
				}
				return claxx.Equals(classReflector);
			}

			private readonly IReflectClass claxx;
		}
	}
}
