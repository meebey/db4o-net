/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.IO;
using Db4objects.Db4o.Tests.Common.Util;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			Type[] commonCases = new Type[] { typeof(IoAdapterTest) };
			return Db4oUnitTestUtil.MergeClasses(commonCases, StackTraceBasedCases());
		}

		/// <decaf.replaceFirst>return new Class[0];</decaf.replaceFirst>
		private Type[] StackTraceBasedCases()
		{
			return new Type[0];
		}
	}
}
