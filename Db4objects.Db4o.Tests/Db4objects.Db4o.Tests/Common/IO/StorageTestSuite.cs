/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class StorageTestSuite : FixtureTestSuiteDescription
	{
		public StorageTestSuite()
		{
			{
				FixtureProviders(new IFixtureProvider[] { new SubjectFixtureProvider(new object[]
					 { new FileStorage(), new MemoryStorage(), new CachingStorage(new FileStorage())
					, new IoAdapterStorage(new RandomAccessFileAdapter()) }) });
				TestUnits(new Type[] { typeof(BinTest), typeof(ReadOnlyBinTest), typeof(StorageTest
					) });
			}
		}
		//	combinationToRun(2);
	}
}
