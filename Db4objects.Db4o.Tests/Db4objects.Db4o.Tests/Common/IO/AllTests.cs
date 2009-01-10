/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class AllTests : ReflectionTestSuite
	{
		public static void Main(string[] arguments)
		{
			new ConsoleTestRunner(typeof(Db4objects.Db4o.Tests.Common.IO.AllTests)).Run();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(BlockAwareBinTestSuite), typeof(BlockSizeDependentBinTestCase
				), typeof(IoAdapterTestSuite), typeof(MemoryBinIsReusableTestCase), typeof(RandomAccessFileStorageFactoryTestCase
				), typeof(StorageTestSuite), typeof(NonFlushingStorageTestCase) };
		}
	}
}
