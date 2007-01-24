namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public abstract class AbstractDb4oDefragTestCase : Db4oUnit.ITest
	{
		public virtual string GetLabel()
		{
			return "DefragAllTestCase: " + TestSuite().FullName;
		}

		public abstract System.Type TestSuite();

		public virtual void Run(Db4oUnit.TestResult result)
		{
			try
			{
				new Db4oUnit.Extensions.Db4oTestSuiteBuilder(new Db4objects.Db4o.Tests.Common.Defragment.Db4oDefragSolo
					(new Db4oUnit.Extensions.Fixtures.IndependentConfigurationSource()), TestSuite()
					).Build().Run(result);
			}
			catch (System.Exception e)
			{
				result.TestFailed(this, e);
			}
		}
	}
}
