/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using Db4oTool.Core;
using Db4oUnit;

namespace Db4oTool.Tests.Core
{
	public abstract class AbstractCommandLineInstrumentationTestCase : AbstractInstrumentationTestCase
	{
		protected abstract string CommandLine { get; }

		override protected void InstrumentAssembly(string path)
		{
			string[] commandLine = BuildCommandLine(path);
			ShellUtilities.ProcessOutput output = System.Diagnostics.Debugger.IsAttached
				? ShellUtilities.shellm(InstrumentationUtilityPath, commandLine)
				: ShellUtilities.shell(InstrumentationUtilityPath, commandLine);
			CheckInstrumentationOutput(output);
		}

		private string[] BuildCommandLine(string path)
		{
			string[] cmdLine = CommandLine.Split(' ');
			cmdLine = ArrayServices.Append(cmdLine, path);
			//cmdLine = ArrayServices.Append(cmdLine, "-vv");
			return cmdLine;
		}

		protected virtual void CheckInstrumentationOutput(ShellUtilities.ProcessOutput output)
		{
			if (output.ExitCode == 0) return;

			Assert.Fail(output.ToString());
		}

		private static string InstrumentationUtilityPath
		{
			get { return typeof(InstrumentationPipeline).Module.FullyQualifiedName; }
		}
	}
}
