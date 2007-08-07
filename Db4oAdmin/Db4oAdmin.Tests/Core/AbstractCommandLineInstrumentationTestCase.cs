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
			ShellUtilities.ProcessOutput output = ShellUtilities.shell(InstrumentationUtilityPath, ArrayServices.Append(CommandLine.Split(' '), path));
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
	}
}
