/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Internal.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(Comparable4TestCase), typeof(DeactivateTestCase), typeof(
				EmbeddedClientObjectContainerTestCase), typeof(InternalObjectContainerAPITestCase
				), typeof(MarshallingBufferTestCase), typeof(MarshallingContextTestCase), typeof(
				PartialObjectContainerTestCase), typeof(SerializerTestCase), typeof(TransactionTestCase
				) };
		}
	}
}
