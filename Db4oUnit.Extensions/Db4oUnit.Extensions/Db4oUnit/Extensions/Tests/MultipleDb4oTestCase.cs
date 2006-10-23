namespace Db4oUnit.Extensions.Tests
{
	public class MultipleDb4oTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static int configureCalls = 0;

		public static void ResetConfigureCalls()
		{
			configureCalls = 0;
		}

		public static int ConfigureCalls()
		{
			return configureCalls;
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			configureCalls++;
		}

		public virtual void TestFirst()
		{
			Db4oUnit.Assert.Fail();
		}

		public virtual void TestSecond()
		{
			Db4oUnit.Assert.Fail();
		}
	}
}
