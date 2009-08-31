using System;
using System.Diagnostics;
using Db4objects.Db4o.Monitoring.Internal;
using Db4oUnit;

namespace Db4oTool.Tests.Core
{
	class InstallPerformanceCountersTestCase : ITestCase
	{
		public void Test()
		{
			if (!UnrestrictedRegistryAccess())
			{
				Console.Error.WriteLine("WARNING: {0} requires unrestricted access to the registry to run.", GetType());
				return;
			}

			if (Db4oCategoryExists())
			{
				PerformanceCounterCategory.Delete(Db4oPerformanceCounterCategory.CategoryName);
			}

			ProgramOptions options = new ProgramOptions();
			options.InstallPerformanceCounters = true;

			Db4oTool.Program.Run(options);

			Assert.IsTrue(Db4oCategoryExists());
		}

		private bool UnrestrictedRegistryAccess()
		{
			return System.Security.SecurityManager.IsGranted(new System.Security.Permissions.RegistryPermission(System.Security.Permissions.PermissionState.Unrestricted));
		}

		private bool Db4oCategoryExists()
		{
			return PerformanceCounterCategory.Exists(Db4oPerformanceCounterCategory.CategoryName);
		}
	}
}
