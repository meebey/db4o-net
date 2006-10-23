namespace Db4oUnit
{
	public interface ITest
	{
		string GetLabel();

		void Run(Db4oUnit.TestResult result);
	}
}
