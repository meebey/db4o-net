/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Config;

namespace Db4objects.Db4o.Tests.Common.Config
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Config.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ConfigurationItemTestCase), typeof(CustomStringEncodingTestCase
				), typeof(GlobalVsNonStaticConfigurationTestCase), typeof(LatinStringEncodingTestCase
				), typeof(ObjectTranslatorTestCase), typeof(UnicodeStringEncodingTestCase), typeof(
				UTF8StringEncodingTestCase) };
		}
	}
}
