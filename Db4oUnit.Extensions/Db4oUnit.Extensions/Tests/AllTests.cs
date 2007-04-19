using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Tests;
using Db4oUnit.Tests;

namespace Db4oUnit.Extensions.Tests
{
	public class AllTests : ITestCase
	{
		private sealed class ExcludingInMemoryFixture : Db4oInMemory
		{
			public ExcludingInMemoryFixture(AllTests _enclosing, IConfigurationSource source)
				 : base(source)
			{
				this._enclosing = _enclosing;
			}

			public override bool Accept(Type clazz)
			{
				return !typeof(IOptOutFromTestFixture).IsAssignableFrom(clazz);
			}

			private readonly AllTests _enclosing;
		}

		public static void Main(string[] args)
		{
			new TestRunner(typeof(AllTests)).Run();
		}

		public virtual void TestSingleTestWithDifferentFixtures()
		{
			IConfigurationSource configSource = new IndependentConfigurationSource();
			AssertSimpleDb4o(new Db4oInMemory(configSource));
			AssertSimpleDb4o(new Db4oSolo(configSource));
		}

		public virtual void TestMultipleTestsSingleFixture()
		{
			MultipleDb4oTestCase.ResetConfigureCalls();
			FrameworkTestCase.RunTestAndExpect(new Db4oTestSuiteBuilder(new Db4oInMemory(new 
				IndependentConfigurationSource()), typeof(MultipleDb4oTestCase)).Build(), 2, false
				);
			Assert.AreEqual(2, MultipleDb4oTestCase.ConfigureCalls());
		}

		public virtual void TestSelectiveFixture()
		{
			IDb4oFixture fixture = new AllTests.ExcludingInMemoryFixture(this, new IndependentConfigurationSource
				());
			TestSuite suite = new Db4oTestSuiteBuilder(fixture, new Type[] { typeof(AcceptedTestCase)
				, typeof(NotAcceptedTestCase) }).Build();
			Assert.AreEqual(1, suite.GetTests().Length);
			FrameworkTestCase.RunTestAndExpect(suite, 0);
		}

		private void AssertSimpleDb4o(IDb4oFixture fixture)
		{
			TestSuite suite = new Db4oTestSuiteBuilder(fixture, typeof(SimpleDb4oTestCase)).Build
				();
			SimpleDb4oTestCase subject = GetTestSubject(suite);
			subject.ExpectedFixture(fixture);
			FrameworkTestCase.RunTestAndExpect(suite, 0);
			Assert.IsTrue(subject.EverythingCalled());
		}

		private SimpleDb4oTestCase GetTestSubject(TestSuite suite)
		{
			return ((SimpleDb4oTestCase)((TestMethod)suite.GetTests()[0]).GetSubject());
		}

		public virtual void TestInterfaceIsAvailable()
		{
			Assert.IsTrue(typeof(IDb4oTestCase).IsAssignableFrom(typeof(AbstractDb4oTestCase)
				));
		}
	}
}
