/* Copyright (C) 2009   db4objects Inc.   http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Internal
{
	public partial class Config4Impl
	{
		private static IClientServerFactory DefaultClientServerFactory()
		{
			Assembly csAssembly = Assembly.Load(ClientServerAssemblyName());
			return (IClientServerFactory) Activator.CreateInstance(csAssembly.GetType("Db4objects.Db4o.Internal.CS.Config.ClientServerFactoryImpl"));
		}

		private static string ClientServerAssemblyName()
		{
			Assembly db4oAssembly = typeof(IObjectContainer).Assembly;
			string db4oAssemblySimpleName = db4oAssembly.GetName().Name;
			return db4oAssembly.FullName.Replace(db4oAssemblySimpleName, "Db4objects.Db4o.CS");
		}

		private static Type[] IgnoredClasses()
		{
#if SILVERLIGHT
			return new Type[] { typeof(StaticClass), typeof(StaticField) };
#else
			return new Type[] { typeof(P1HashElement), typeof(P1ListElement), typeof(P1Object), typeof(P1Collection), typeof(StaticClass), typeof(StaticField) };
#endif
		}
	}
}
