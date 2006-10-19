namespace Db4oUnit.Extensions.Tests
{
	public class AllTests : Db4oUnit.ITestCase
	{
		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(typeof(Db4oUnit.Extensions.Tests.AllTests)).Run();
		}

		public virtual void TestSingleTestWithDifferentFixtures()
		{
			Db4oUnit.Extensions.Fixtures.IConfigurationSource configSource = new Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource
				();
			AssertSimpleDb4o(new Db4oUnit.Extensions.Fixtures.Db4oInMemory(configSource));
			AssertSimpleDb4o(new Db4oUnit.Extensions.Fixtures.Db4oSolo(configSource));
		}

		public virtual void TestMultipleTestsSingleFixture()
		{
			Db4oUnit.Extensions.Tests.MultipleDb4oTestCase.ResetConfigureCalls();
			Db4oUnit.Tests.FrameworkTestCase.RunTestAndExpect(new Db4oUnit.Extensions.Db4oTestSuiteBuilder
				(new Db4oUnit.Extensions.Fixtures.Db4oInMemory(new Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource
				()), typeof(Db4oUnit.Extensions.Tests.MultipleDb4oTestCase)).Build(), 2, false);
			Db4oUnit.Assert.AreEqual(2, Db4oUnit.Extensions.Tests.MultipleDb4oTestCase.ConfigureCalls
				());
		}

		private void AssertSimpleDb4o(Db4oUnit.Extensions.IDb4oFixture fixture)
		{
			Db4oUnit.TestSuite suite = new Db4oUnit.Extensions.Db4oTestSuiteBuilder(fixture, 
				typeof(Db4oUnit.Extensions.Tests.SimpleDb4oTestCase)).Build();
			Db4oUnit.Extensions.Tests.SimpleDb4oTestCase subject = GetTestSubject(suite);
			subject.ExpectedFixture(fixture);
			Db4oUnit.Tests.FrameworkTestCase.RunTestAndExpect(suite, 0);
			Db4oUnit.Assert.IsTrue(subject.EverythingCalled());
		}

		private Db4oUnit.Extensions.Tests.SimpleDb4oTestCase GetTestSubject(Db4oUnit.TestSuite
			 suite)
		{
			return ((Db4oUnit.Extensions.Tests.SimpleDb4oTestCase)((Db4oUnit.TestMethod)suite
				.GetTests()[0]).GetSubject());
		}
	}
}
