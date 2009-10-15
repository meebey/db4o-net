/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */
using System;
using System.Diagnostics;
using Db4objects.Db4o.Monitoring;
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
				PerformanceCounterCategory.Delete(Db4oPerformanceCounters.CategoryName);
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
			return PerformanceCounterCategory.Exists(Db4oPerformanceCounters.CategoryName);
		}
	}
}
