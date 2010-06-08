/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class AllTests : ComposibleTestSuite
	{
		public static void Main(string[] arguments)
		{
			new ConsoleTestRunner(typeof(Db4objects.Db4o.Tests.Common.IO.AllTests)).Run();
		}

		protected override Type[] TestCases()
		{
			return ComposeTests(new Type[] { typeof(BlockAwareBinTestSuite), typeof(MemoryBinGrowthTestCase
				), typeof(MemoryBinIsReusableTestCase), typeof(MemoryIoAdapterTestCase), typeof(
				NonFlushingStorageTestCase), typeof(RandomAccessFileStorageFactoryTestCase), typeof(
				SaveAsStorageTestCase), typeof(StorageTestSuite) });
		}

		#if !SILVERLIGHT
		protected override Type[] ComposeWith()
		{
			return new Type[] { typeof(BlockSizeDependentBinTestCase), typeof(IoAdapterTestSuite
				), typeof(RandomAccessFileFactoryTestCase) };
		}
		#endif // !SILVERLIGHT
	}
}
