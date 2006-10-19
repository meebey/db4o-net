namespace Db4oUnit
{
	public class TestFailure : Db4oUnit.Printable
	{
		internal Db4oUnit.ITest _test;

		internal System.Exception _failure;

		public TestFailure(Db4oUnit.ITest test, System.Exception failure)
		{
			_test = test;
			_failure = failure;
		}

		public virtual Db4oUnit.ITest GetTest()
		{
			return _test;
		}

		public virtual System.Exception GetFailure()
		{
			return _failure;
		}

		public override void Print(System.IO.TextWriter writer)
		{
			writer.Write(_test.GetLabel());
			writer.Write(": ");
			Db4oUnit.TestPlatform.PrintStackTrace(writer, _failure);
		}
	}
}
