/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using Db4oAdmin.Core;
using Db4oUnit;

namespace Db4oAdmin.Tests.Core
{
	public abstract class AbstractCommandLineInstrumentationTestCase : AbstractInstrumentationTestCase
	{
		protected abstract string CommandLine { get; }

		override protected void InstrumentAssembly(string path)
		{
			ShellUtilities.ProcessOutput output = null;
			if (System.Diagnostics.Debugger.IsAttached)
			{
				output = ShellUtilities.shellm(InstrumentationUtilityPath, BuildCommandLine(path));
			}
			else
			{
				output = ShellUtilities.shell(InstrumentationUtilityPath, BuildCommandLine(path));
			}
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
			if (output.ExitCode != 0) Assert.Fail(output.ToString());
		}

		private static string InstrumentationUtilityPath
		{
			get { return typeof(InstrumentationPipeline).Module.FullyQualifiedName; }
		}
	}
}
