using System;
using System.Diagnostics;
using System.IO;

namespace Db4objects.Db4o.Tests.Util
{
	class IOServices
	{
		public static string FindParentDirectory(string path)
		{
#if !CF_1_0 && !CF_2_0
			string parent = Path.GetFullPath("..");
			while (true)
			{
				if (Directory.Exists(Path.Combine(parent, path))) return parent;
				string oldParent = parent;
				parent = Path.GetDirectoryName(parent);
				if (parent == oldParent || parent == null) break;
			}
#endif
			return null;
		}

		public static void WriteFile(String fname, String contents)
		{
			Directory.CreateDirectory(Path.GetDirectoryName(fname));
			using (StreamWriter writer = new StreamWriter(fname))
			{
				writer.Write(contents);
			}
		}
		
#if !CF_1_0 && !CF_2_0
		public static String Exec(String program, params String[] arguments)
		{
			ProcessStartInfo psi = new ProcessStartInfo(program);
			psi.UseShellExecute = false;
			psi.Arguments = string.Join(" ", arguments);
			psi.RedirectStandardOutput = true;
			psi.RedirectStandardError = true;
			psi.WorkingDirectory = Path.GetTempPath();
			psi.CreateNoWindow = true;

			Process p = Process.Start(psi);
			string stdout = p.StandardOutput.ReadToEnd();
			string stderr = p.StandardError.ReadToEnd();
			p.WaitForExit();
			return stdout + stderr;
		}
#endif

		public static String BuildTempPath(String fname)
		{
			return Path.Combine(Path.GetTempPath(), fname);
		}
	}
}
