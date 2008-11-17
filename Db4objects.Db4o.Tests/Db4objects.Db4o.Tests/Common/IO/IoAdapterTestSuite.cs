/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class IoAdapterTestSuite : FixtureBasedTestSuite
	{
		/// <decaf.replaceFirst>return jdk11FixtureProviders();</decaf.replaceFirst>
		public override IFixtureProvider[] FixtureProviders()
		{
			return Jdk11FixtureProviders();
		}

		private IFixtureProvider[] Jdk11FixtureProviders()
		{
			return new IFixtureProvider[] { new SubjectFixtureProvider(new object[] { new RandomAccessFileAdapter
				(), new CachedIoAdapter(new RandomAccessFileAdapter()) }) };
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(IoAdapterTest), typeof(ReadOnlyIoAdapterTest) };
		}
	}
}
