using Db4oTool.Core;

namespace Db4oTool.Tests.Core
{
	static class InstrumentationServices
	{
		public static ShellUtilities.ProcessOutput InstrumentAssembly(string options, string path)
		{
			string[] commandLine = BuildCommandLine(options, path);
			return System.Diagnostics.Debugger.IsAttached
			       	? ShellUtilities.shellm(InstrumentationUtilityPath, commandLine)
			       	: ShellUtilities.shell(InstrumentationUtilityPath, commandLine);
		}

		public static string[] BuildCommandLine(string options, string path)
		{
			string[] cmdLine = options.Split(' ');
			cmdLine = ArrayServices.Append(cmdLine, path);
			//cmdLine = ArrayServices.Append(cmdLine, "-vv");
			return cmdLine;
		}

		public static string InstrumentationUtilityPath
		{
			get { return typeof(InstrumentationPipeline).Module.FullyQualifiedName; }
		}
	}
}
