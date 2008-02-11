using System;
using System.Collections;
using System.Globalization;
using System.IO;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class Db4oLibrarian
	{
		private const double MinimunVersionToTest = 6.0;
		private Db4oLibraryEnvironmentProvider _provider;

		public Db4oLibrarian(Db4oLibraryEnvironmentProvider provider)
		{
			_provider = provider;
		}

		public Db4oLibrary[] Libraries()
		{
			ArrayList libraries = new ArrayList();
			foreach (string directory in Directory.GetDirectories(LibraryPath()))
			{
				// comment out the next line to run against legacy versions
				if (!IsVersionOrGreater(directory, MinimunVersionToTest)) continue;

				string db4oLib = FindLibraryFile(directory);
				if (null == db4oLib) continue;
				libraries.Add(ForFile(db4oLib));
			}
			return (Db4oLibrary[]) libraries.ToArray(typeof(Db4oLibrary));
		}

		private static bool IsVersionOrGreater(string versionName, double minimumVersion)
		{
#if !CF
			double currentVersion;
			if (!Double.TryParse(Path.GetFileName(versionName), NumberStyles.AllowDecimalPoint | NumberStyles.Float, NumberFormatInfo.InvariantInfo, out currentVersion))
			{
				return false;
			}

			return currentVersion >= minimumVersion;
#else
			return false;
#endif
		}

		public static bool IsLegacyVersion(string versionName)
		{
			return VersionFromVersionName(versionName) < MinimunVersionToTest;
		}

		private static double VersionFromVersionName(string versionName)
		{
			Version version = new Version(versionName);
			return version.Major + version.Minor / 10;
		}

		public static string LibraryPath()
		{
			return WorkspaceServices.WorkspacePath("db4o.archives/net-2.0");
		}

		public Db4oLibrary ForVersion(string version)
		{
			return ForFile(FindLibraryFile(Path.Combine(LibraryPath(), version)));
		}

		public Db4oLibrary ForFile(string db4oLib)
		{
			return new Db4oLibrary(db4oLib, EnvironmentFor(db4oLib));
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