namespace Db4oUnit
{
	public class TestResult : Db4oUnit.Printable
	{
		private Db4oUnit.TestFailureCollection _failures = new Db4oUnit.TestFailureCollection
			();

		private int _testCount = 0;

		public virtual void TestStarted()
		{
			++_testCount;
		}

		public virtual void TestFailed(Db4oUnit.ITest test, System.Exception failure)
		{
			_failures.Add(new Db4oUnit.TestFailure(test, failure));
		}

		public virtual bool Green()
		{
			return _failures.Size() == 0;
		}

		public virtual Db4oUnit.TestFailureCollection Failures()
		{
			return _failures;
		}

		public override void Print(System.IO.TextWriter writer)
		{
			if (Green())
			{
				writer.Write("GREEN (" + _testCount + " tests)\n");
				return;
			}
			writer.Write("RED (" + _failures.Size() + " out of " + _testCount + " tests failed)\n"
				);
			_failures.Print(writer);
		}

		public virtual int Assertions()
		{
			return 0;
		}
	}
}
