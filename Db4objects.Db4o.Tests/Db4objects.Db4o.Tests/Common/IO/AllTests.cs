/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class AllTests : ITestSuiteBuilder
	{
		public virtual IEnumerator GetEnumerator()
		{
			return new ReflectionTestSuiteBuilder(new Type[] { typeof(IoAdapterTest) }).GetEnumerator
				();
		}

		public static void Main(string[] args)
		{
			new ConsoleTestRunner(typeof(Db4objects.Db4o.Tests.Common.IO.AllTests)).Run();
		}
	}
}
