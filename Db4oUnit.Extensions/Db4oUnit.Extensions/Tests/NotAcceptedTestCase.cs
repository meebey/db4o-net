namespace Db4oUnit.Extensions.Tests
{
	public class NotAcceptedTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutFromTestFixture
	{
		public virtual void Test()
		{
			Db4oUnit.Assert.Fail("Opted out test should not be run.");
		}
	}
}
