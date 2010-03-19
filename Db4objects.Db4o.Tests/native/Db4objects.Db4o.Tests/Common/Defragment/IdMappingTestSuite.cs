using Db4oUnit.Fixtures;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public partial class IdMappingTestSuite
	{
		public override IFixtureProvider[] FixtureProviders()
		{
			return new IFixtureProvider[] 
						{
							new SimpleFixtureProvider(
								_fixture, 

								new object[] 
								{
									 new InMemoryIdMappingProvider(null),
#if !SILVERLIGHT
									new DatabaseIdMappingProvider(null),
#endif
								}
							)
						};
		}
	}
}
