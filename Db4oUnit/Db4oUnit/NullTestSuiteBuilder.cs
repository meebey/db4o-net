using Db4oUnit;

namespace Db4oUnit
{
	public class NullTestSuiteBuilder : ITestSuiteBuilder
	{
		private TestSuite _suite;

		public NullTestSuiteBuilder(TestSuite suite)
		{
			_suite = suite;
		}

		public virtual TestSuite Build()
		{
			return _suite;
		}
	}
}
