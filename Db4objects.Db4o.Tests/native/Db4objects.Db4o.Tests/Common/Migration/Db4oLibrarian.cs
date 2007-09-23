using System.Collections;
using System.IO;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class Db4oLibrarian
	{
		private Db4oLibraryEnvironmentProvider _provider;

		public Db4oLibrarian(Db4oLibraryEnvironmentProvider provider)
		{
			_provider = provider;
		}

		public Db4oLibrary[] Libraries()
		{
			ArrayList libraries = new ArrayList();
			foreach (string directory in Directory.GetDirectories(WorkspaceServices.WorkspacePath("db4o.archives/net-2.0")))
			{
				// for now, only 6.X libraries
				if (!Path.GetFileName(directory).StartsWith("6")) continue;

				string db4oLib = FindLibraryFile(directory);
				if (null == db4oLib) continue;
				libraries.Add(new Db4oLibrary(db4oLib, EnvironmentFor(db4oLib)));
			}
			return (Db4oLibrary[]) libraries.ToArray(typeof(Db4oLibrary));
		}

		private string FindLibraryFile(string directory)
		{
			string[] found = Directory.GetFiles(directory, "*.dll");
			return found.Length == 1 ? found[0] : null;
		}

		private Db4oLibraryEnvironment EnvironmentFor(string db4oLib)
		{
			return _provider.EnvironmentFor(db4oLib);
		}
	}
}