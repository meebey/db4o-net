using Db4oUnit;

namespace Db4oTool.Tests
{
	class ProgramOptionsTestCase : ITestCase
	{	
		public void TestTpSameAsTA()
		{
			AssertTransparentPersistence("-tp");
			AssertTransparentPersistence("-ta");
		}

		private static void AssertTransparentPersistence(string arg)
		{
			ProgramOptions options = new ProgramOptions();
			Assert.IsFalse(options.TransparentPersistence);
			options.ProcessArgs(new string[] { arg });
			Assert.IsTrue(options.TransparentPersistence);
		}
	}
}
