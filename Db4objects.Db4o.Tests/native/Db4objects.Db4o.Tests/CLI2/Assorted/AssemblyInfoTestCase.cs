using System;
using System.Reflection;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
	class AssemblyInfoTestCase : ITestCase
	{
		public void Test()
		{
			Type[] assemblyReferences = new Type[]
				{
					typeof(Db4oFactory),
					typeof(Db4objects.Db4o.Instrumentation.Api.ITypeEditor),
					typeof(Db4objects.Db4o.NativeQueries.NQOptimizer),
				};
			foreach (Type type in assemblyReferences)
			{
				AssemblyName assemblyName = type.Assembly.GetName();
				Assert.AreEqual(ExpectedVersion(), assemblyName.Version, assemblyName.FullName);
			}
		}

		private static Version ExpectedVersion()
		{
			return new Version(Db4oVersion.Major, Db4oVersion.Minor, Db4oVersion.Iteration, Db4oVersion.Revision);
		}
	}
}
