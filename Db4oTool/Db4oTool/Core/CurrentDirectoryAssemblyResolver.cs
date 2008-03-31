using System;
using System.Reflection;
using System.IO;

namespace Db4oTool.Core
{
	public class CurrentDirectoryAssemblyResolver : DirectoryAssemblyResolver
	{
		public CurrentDirectoryAssemblyResolver()
			: base(Environment.CurrentDirectory)
		{
		}
	}
}
