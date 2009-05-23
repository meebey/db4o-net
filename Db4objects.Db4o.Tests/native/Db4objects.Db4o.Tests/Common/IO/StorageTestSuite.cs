/* Copyright (C) 2009 Versant Inc.  http://www.db4o.com */
using Db4objects.Db4o.IO;
using Db4oUnit;
using Db4oUnit.Fixtures;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public partial class StorageTestSuite
	{
		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[] 
						{
							new EnvironmentProvider(), 
							new SubjectFixtureProvider(
									new object[] 
									{ 
										TestPlatform.NewPersistentStorage(),
										new MemoryStorage(), 
										new CachingStorage(TestPlatform.NewPersistentStorage()), 
#if !SILVERLIGHT
										new IoAdapterStorage(new RandomAccessFileAdapter()) 
#endif
									})
						};
		}
	}
}
