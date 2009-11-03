/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Defragment.AllTests().RunAll();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(BlockSizeDefragTestCase), typeof(DefragEncryptedFileTestCase
				), typeof(DefragInheritedFieldIndexTestCase), typeof(DefragInMemoryTestSuite), typeof(
				DefragmentUntypedPrimitiveArrayTestCase), typeof(SlotDefragmentTestCase), typeof(
				StoredClassFilterTestCase), typeof(TranslatedDefragTestCase) };
		}
	}
}
