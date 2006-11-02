using System;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	public abstract class AbstractCommandLineInstrumentationTestCase : AbstractInstrumentationTestCase
	{
		protected abstract string CommandLine { get; }

		override protected void InstrumentAssembly(string path)
		{
			ShellUtilities.ProcessOutput output = ShellUtilities.shell(InstrumentationUtilityPath, Append(CommandLine.Split(' '), path));
			CheckInstrumentationOutput(output);
		}

		protected virtual void CheckInstrumentationOutput(ShellUtilities.ProcessOutput output)
		{
			if (output.ExitCode != 0) Assert.Fail(output.ToString());
		}

		private static string InstrumentationUtilityPath
		{
			get { return typeof(InstrumentationPipeline).Module.FullyQualifiedName; }
		}

		private string[] Append(string[] line, string path)
		{
			Array.Resize(ref line, line.Length + 1);
			line[line.Length - 1] = path;
			return line;
		}
	}
}
