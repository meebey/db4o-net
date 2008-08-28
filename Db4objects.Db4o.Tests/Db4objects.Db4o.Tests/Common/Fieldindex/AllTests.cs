/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Fieldindex;
using Db4objects.Db4o.Tests.Common.Util;

namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.AllTests().RunSolo();
		}

		protected override Type[] TestCases()
		{
			Type[] fieldBased = new Type[] { typeof(IndexedNodeTestCase), typeof(FieldIndexTestCase
				), typeof(FieldIndexProcessorTestCase), typeof(StringFieldIndexTestCase) };
			Type[] neutral = new Type[] { typeof(DoubleFieldIndexTestCase), typeof(RuntimeFieldIndexTestCase
				), typeof(SecondLevelIndexTestCase), typeof(StringIndexTestCase), typeof(StringIndexCorruptionTestCase
				), typeof(StringIndexWithSuperClassTestCase) };
			return Db4oUnitTestUtil.MergeClasses(neutral, fieldBased);
		}
	}
}
