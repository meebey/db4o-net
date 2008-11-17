/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTestSuite : FixtureTestSuiteDescription
	{
		private sealed class _IoAdapterWithCache_14 : IoAdapterWithCache
		{
			public _IoAdapterWithCache_14(RandomAccessFileAdapter baseArg1) : base(baseArg1)
			{
			}

			protected override ICache4 NewCache(int pageCount)
			{
				return CacheFactory.New2QCache(pageCount);
			}
		}

		internal IoAdapterTestSuite()
		{
			FixtureProviders(new IFixtureProvider[] { new SubjectFixtureProvider(new object[]
				 { new RandomAccessFileAdapter(), new CachedIoAdapter(new RandomAccessFileAdapter
				()), new _IoAdapterWithCache_14(new RandomAccessFileAdapter()) }) });
			TestUnits(new Type[] { typeof(IoAdapterTest), typeof(ReadOnlyIoAdapterTest) });
		}
		//	combinationToRun(2);
	}
}
