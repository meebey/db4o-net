namespace Db4objects.Db4o.Tests.Common.Pending
{
	/// <summary>Failing test cases</summary>
	public class AllTestsPending : Db4oUnit.Extensions.Db4oTestSuite
	{
		protected override System.Type[] TestCases()
		{
			return new System.Type[] { typeof(Db4objects.Db4o.Tests.Common.Pending.SortMultipleTestCase)
				 };
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Pending.AllTestsPending().RunSolo();
		}
	}
}
