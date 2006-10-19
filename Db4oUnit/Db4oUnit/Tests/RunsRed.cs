namespace Db4oUnit.Tests
{
	internal class RunsRed : Db4oUnit.ITest
	{
		private System.Exception _exception;

		public RunsRed(System.Exception exception)
		{
			_exception = exception;
		}

		public virtual string GetLabel()
		{
			return "RunsRed";
		}

		public virtual void Run(Db4oUnit.TestResult result)
		{
			result.TestFailed(this, _exception);
		}
	}
}
