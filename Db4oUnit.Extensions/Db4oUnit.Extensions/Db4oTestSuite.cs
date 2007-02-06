namespace Db4oUnit.Extensions
{
	/// <summary>Base class for composable db4o test suites (AllTests classes inside each package, for instance).
	/// 	</summary>
	/// <remarks>Base class for composable db4o test suites (AllTests classes inside each package, for instance).
	/// 	</remarks>
	public abstract class Db4oTestSuite : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.ITestSuiteBuilder
	{
		public virtual Db4oUnit.TestSuite Build()
		{
			return new Db4oUnit.Extensions.Db4oTestSuiteBuilder(Fixture(), TestCases()).Build
				();
		}

		protected abstract override System.Type[] TestCases();
	}
}
