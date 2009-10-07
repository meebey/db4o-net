/* Copyright (C) 2009  Versant Inc.   http://www.db4o.com */

#if !CF && !SILVERLIGHT

using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.Monitoring
{
	public class PerformanceCounterTestCaseBase : AbstractDb4oTestCase
	{
		private static bool _installed;

		protected override void Db4oSetupBeforeConfigure()
		{
			Db4oPerformanceCounterInstaller.ReInstall();
		}
	}
}

#endif