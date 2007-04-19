using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>For test cases that need setUp/tearDown support.</summary>
	/// <remarks>For test cases that need setUp/tearDown support.</remarks>
	public interface ITestLifeCycle : ITestCase
	{
		void SetUp();

		void TearDown();
	}
}
