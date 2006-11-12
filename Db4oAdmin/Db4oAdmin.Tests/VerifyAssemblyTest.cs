using System;
using System.IO;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	class VerifyAssemblyTest : ITest
	{
		private string _assemblyPath;

		public VerifyAssemblyTest(string assemblyPath)
		{
			_assemblyPath = assemblyPath;
		}
			
		public string GetLabel()
		{
			return string.Format("peverify \"{0}\"", Path.GetFileNameWithoutExtension(_assemblyPath));
		}

		public void Run(TestResult result)
		{
			result.TestStarted(this);
			try
			{
				VerifyAssembly();
			}
			catch (Exception x)
			{
				result.TestFailed(this, x);
			}
		}

		void VerifyAssembly()
		{
			ShellUtilities.ProcessOutput output = ShellUtilities.shell("peverify.exe", _assemblyPath);
			string stdout = output.ToString();
			if (stdout.Contains("1.1.4322.573")) return; // ignore older peverify version errors
			if (output.ExitCode == 0 && !stdout.ToUpper().Contains("WARNING")) return;
			Assert.Fail(stdout);
		}
	}
}