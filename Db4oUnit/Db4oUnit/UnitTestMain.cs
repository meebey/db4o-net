namespace Db4oUnit
{
	public class UnitTestMain
	{
		public static void Main(string[] args)
		{
			System.Type[] classes = new System.Type[args.Length];
			for (int idx = 0; idx < args.Length; idx++)
			{
				classes[idx] = System.Type.GetType(args[idx]);
			}
			Db4oUnit.ITestSuiteBuilder builder = new Db4oUnit.ReflectionTestSuiteBuilder(classes
				);
			Db4oUnit.TestRunner runner = new Db4oUnit.TestRunner(builder);
			runner.Run();
		}
	}
}
