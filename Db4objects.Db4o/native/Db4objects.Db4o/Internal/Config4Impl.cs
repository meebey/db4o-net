using System;
using System.Reflection;
using Db4objects.Db4o.Config;

namespace Db4objects.Db4o.Internal
{
	public partial class Config4Impl
	{
		private static IClientServerFactory DefaultClientServerFactory()
		{
			AssemblyName csAssemblyName = NewAssemblyName(typeof(Config4Impl).Assembly, "Db4objects.Db4o.CS");
			return (IClientServerFactory) Activator.CreateInstance(Assembly.Load(csAssemblyName).GetType("Db4objects.Db4o.Internal.CS.Config.ClientServerFactoryImpl"));
		}

		private static AssemblyName NewAssemblyName(Assembly template, string newName)
		{
			AssemblyName name = template.GetName(true);
			name.Name = newName;
			return name;
		}
	}
}
