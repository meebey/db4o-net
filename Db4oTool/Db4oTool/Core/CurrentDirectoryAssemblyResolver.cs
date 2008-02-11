using System;
using System.Reflection;
using System.IO;

namespace Db4oTool.Core
{
	public class CurrentDirectoryAssemblyResolver : IDisposable
	{
		public CurrentDirectoryAssemblyResolver()
		{
			CurrentDomain().AssemblyResolve += AppDomain_AssemblyResolve;
		}

		Assembly AppDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			string baseName = Path.Combine(Environment.CurrentDirectory, SimpleName(args.Name));
			Assembly found = ProbeFile(baseName + ".dll");
			if (found != null) return found;
			return ProbeFile(baseName + ".exe");
		}

		private string SimpleName(string assemblyName)
		{
			return assemblyName.Split(',')[0];
		}

		private Assembly ProbeFile(string fname)
		{
			if (!File.Exists(fname)) return null;
			return Assembly.LoadFile(fname);
		}

		public void Dispose()
		{
			CurrentDomain().AssemblyResolve -= AppDomain_AssemblyResolve;
		}

		private static AppDomain CurrentDomain()
		{
			return AppDomain.CurrentDomain;
		}
	}
}
