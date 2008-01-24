/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.References;

namespace Db4objects.Db4o.Tests.Common.References
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.References.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(HardObjectReferenceTestCase), typeof(ReferenceSystemIntegrationTestCase
				), typeof(ReferenceSystemRegistryTestCase), typeof(HashcodeReferenceSystemTestCase
				), typeof(TransactionalReferenceSystemTestCase) };
		}
	}
}
