using Db4oUnit;

namespace Db4oUnit
{
	public interface ITest
	{
		string GetLabel();

		void Run(TestResult result);
	}
}
