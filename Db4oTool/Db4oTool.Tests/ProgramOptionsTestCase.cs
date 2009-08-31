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

		public void TestInstallPerformanceCounters()
		{
			ProgramOptions options = new ProgramOptions();
			Assert.IsFalse(options.InstallPerformanceCounters);
			options.ProcessArgs(new string[] { "--install-performance-counters"});
			Assert.IsTrue(options.InstallPerformanceCounters);
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
