/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class AllTests : ITestSuiteBuilder
	{
		public virtual TestSuite Build()
		{
			return new ReflectionTestSuiteBuilder(new Type[] { typeof(IoAdapterTest) }).Build
				();
		}

		public static void Main(string[] args)
		{
			new TestRunner(typeof(Db4objects.Db4o.Tests.Common.IO.AllTests)).Run();
		}
	}
}
