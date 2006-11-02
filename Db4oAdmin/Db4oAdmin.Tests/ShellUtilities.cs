using System.IO;
using System.Diagnostics;

namespace Db4oAdmin.Tests
{
	public class ShellUtilities
	{
		public static void CopyFileToFolder(string fname, string path)
		{
			File.Copy(fname, Path.Combine(path, Path.GetFileName(fname)), true);
		}

		public class ProcessOutput
		{
			public int ExitCode;
			public string StdOut;
			public string StdErr;
			
			public override string ToString()
			{
				return StdOut + StdErr;
			}
		}

		public static ProcessOutput shell(string fname, params string[] args)
		{
			Process p = StartProcess(fname, args);
			ProcessOutput output = new ProcessOutput();
			output.StdOut = p.StandardOutput.ReadToEnd();
            output.StdErr = p.StandardError.ReadToEnd();
			p.WaitForExit();
			output.ExitCode = p.ExitCode;
			return output;
		}

		public static Process StartProcess(string filename, params string[] args)
		{
			Process p = new Process();
			p.StartInfo.CreateNoWindow = true;
			p.StartInfo.UseShellExecute = false;
			p.StartInfo.RedirectStandardOutput = true;
			p.StartInfo.RedirectStandardInput = true;
			p.StartInfo.RedirectStandardError = true;
			p.StartInfo.FileName = filename;
			p.StartInfo.Arguments = string.Join(" ", quote(args));
			p.Start();
			return p;
		}

		private static string[] quote(string[] args)
		{
			for (int i = 0; i < args.Length; ++i)
			{
				args[i] = string.Format("\"{0}\"", args[i]);
			}
			return args;
		}
	}
}
