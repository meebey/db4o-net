namespace Db4oUnit
{
	public class NullTestSuiteBuilder : Db4oUnit.ITestSuiteBuilder
	{
		private Db4oUnit.TestSuite _suite;

		public NullTestSuiteBuilder(Db4oUnit.TestSuite suite)
		{
			_suite = suite;
		}

		public virtual Db4oUnit.TestSuite Build()
		{
			return _suite;
		}
	}
}
