/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTestSuite : FixtureTestSuiteDescription
	{
		public IoAdapterTestSuite()
		{
			{
				FixtureProviders(new IFixtureProvider[] { new SubjectFixtureProvider(new object[]
					 { new RandomAccessFileAdapter(), new CachedIoAdapter(new RandomAccessFileAdapter
					()) }) });
				TestUnits(new Type[] { typeof(IoAdapterTest), typeof(ReadOnlyIoAdapterTest) });
			}
		}
		//	combinationToRun(2);
	}
}
