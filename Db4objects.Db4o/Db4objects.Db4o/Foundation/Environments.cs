/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal;
using Sharpen.Lang;

namespace Db4objects.Db4o.Foundation
{
	public partial class Environments
	{
		private static readonly DynamicVariable _current = DynamicVariable.NewInstance();

		public static object My(Type service)
		{
			IEnvironment environment = ((IEnvironment)_current.Value);
			if (null == environment)
			{
				throw new InvalidOperationException();
			}
			return environment.Provide(service);
		}

		public static void RunWith(IEnvironment environment, IRunnable runnable)
		{
			_current.With(environment, runnable);
		}

		public static IEnvironment NewConventionBasedEnvironment()
		{
			return new Environments.ConventionBasedEnvironment();
		}

		private sealed class ConventionBasedEnvironment : IEnvironment
		{
			private readonly IDictionary _bindings = new Hashtable();

			public object Provide(Type service)
			{
				object existing = _bindings[service];
				if (null != existing)
				{
					return (object)existing;
				}
				object binding = Resolve(service);
				_bindings[service] = binding;
				return binding;
			}

			/// <summary>
			/// Resolves a service interface to its default implementation using the
			/// db4o namespace convention:
			/// interface foo.bar.Baz
			/// default implementation foo.internal.bar.BazImpl
			/// </summary>
			/// <returns>the convention based type name for the requested service</returns>
			private object Resolve(Type service)
			{
				string className = DefaultImplementationFor(service);
				object binding = ReflectPlatform.CreateInstance(className);
				if (null == binding)
				{
					throw new ArgumentException("Cant find default implementation for " + service.ToString
						() + ": " + className);
				}
				return (object)binding;
			}
		}
	}
}
