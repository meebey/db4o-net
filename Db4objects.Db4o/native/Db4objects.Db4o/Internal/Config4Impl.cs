/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */

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
			return (IClientServerFactory) Activator.CreateInstance(csAssembly.GetType("Db4objects.Db4o.CS.Internal.Config.ClientServerFactoryImpl"));
		}

		private static string ClientServerAssemblyName()
		{
			Assembly db4oAssembly = typeof(IObjectContainer).Assembly;
			string db4oAssemblySimpleName = db4oAssembly.GetName().Name;
			return db4oAssembly.FullName.Replace(db4oAssemblySimpleName, "Db4objects.Db4o.CS");
		}

		private static Type[] IgnoredClasses()
		{
			return new Type[] { typeof(P1Object), typeof(StaticClass), typeof(StaticField) };
		}
	}
}
