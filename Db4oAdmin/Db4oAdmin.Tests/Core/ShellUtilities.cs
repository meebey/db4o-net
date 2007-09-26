/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.IO;
using System.Diagnostics;
using System.Reflection;

namespace Db4oAdmin.Tests.Core
{
	public delegate void Action();

	public class ShellUtilities
	{
		public static string WithStdout(Action code)
		{
			StringWriter writer = new StringWriter();
			TextWriter old = Console.Out;
			try
			{
				Console.SetOut(writer);
				code();
				return writer.ToString().Trim();
			}
			finally
			{
				Console.SetOut(old);
			}
		}

		public static string CopyFileToFolder(string fname, string path)
		{
			string targetFileName = Path.Combine(path, Path.GetFileName(fname));
            Directory.CreateDirectory(path);
			File.Copy(fname, targetFileName, true);
			return targetFileName;
		}

		public class ProcessOutput
		{
			public int ExitCode;
			public string StdOut;
			public string StdErr;

			public ProcessOutput()
			{
			}

			public ProcessOutput(int exitCode, string stdout, string stderr)
			{
				ExitCode = exitCode;
				StdOut = stdout;
				StdErr = stderr;
			}

			public override string ToString()
			{
				return StdOut + StdErr;
			}
		}

		public static ProcessOutput shellm(string fname, params string[] args)
		{
			StringWriter stdout = new System.IO.StringWriter();
			StringWriter stderr = new System.IO.StringWriter();
			TextWriter saved = Console.Out;
			TextWriter savedErr = Console.Error;
			try
			{
				Console.SetOut(stdout);
				Console.SetError(stderr);
				Assembly.LoadFrom(fname).EntryPoint.Invoke(null, new object[] { args });
				return new ProcessOutput(0, stdout.ToString(), stderr.ToString());
			}
			finally
			{
				Console.SetOut(saved);
				Console.SetError(savedErr);
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
