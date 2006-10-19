namespace Db4oUnit
{
	/// <summary>For test cases that need setUp/tearDown support.</summary>
	/// <remarks>For test cases that need setUp/tearDown support.</remarks>
	public interface ITestLifeCycle : Db4oUnit.ITestCase
	{
		void SetUp();

		void TearDown();
	}
}
