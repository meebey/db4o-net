namespace Db4oUnit
{
	public class TestRunner
	{
		internal Db4oUnit.ITestSuiteBuilder _suiteBuilder;

		public TestRunner(Db4oUnit.TestSuite suite)
		{
			if (null == suite)
			{
				throw new System.ArgumentException("suite");
			}
			_suiteBuilder = new Db4oUnit.NullTestSuiteBuilder(suite);
		}

		public TestRunner(Db4oUnit.ITestSuiteBuilder builder)
		{
			if (null == builder)
			{
				throw new System.ArgumentException("suite");
			}
			_suiteBuilder = builder;
		}

		public TestRunner(System.Type clazz) : this(new Db4oUnit.ReflectionTestSuiteBuilder
			(clazz))
		{
		}

		public virtual int Run()
		{
			Db4oUnit.TestSuite suite = BuildTestSuite();
			if (null == suite)
			{
				return 1;
			}
			Db4oUnit.TestResult result = new Db4oUnit.TestResult();
			suite.Run(result);
			Report(result);
			return result.Failures().Size();
		}

		private Db4oUnit.TestSuite BuildTestSuite()
		{
			try
			{
				return _suiteBuilder.Build();
			}
			catch (System.Exception x)
			{
				Report(x);
			}
			return null;
		}

		private void Report(System.Exception x)
		{
			System.IO.TextWriter stdout = Db4oUnit.TestPlatform.GetStdOut();
			Db4oUnit.TestPlatform.PrintStackTrace(stdout, x);
		}

		private void Report(Db4oUnit.TestResult result)
		{
			try
			{
				System.IO.TextWriter stdout = Db4oUnit.TestPlatform.GetStdOut();
				result.Print(stdout);
				stdout.Flush();
			}
			catch (System.IO.IOException e)
			{
			}
		}
	}
}
