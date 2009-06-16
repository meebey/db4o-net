using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using Db4objects.Db4o.Tests.Util;

namespace Db4objects.Db4o.Tests.Common.Migration
{
	public class Db4oLibrarian
	{
		private const double MinimumVersionToTest = 6.0;
		private readonly Db4oLibraryEnvironmentProvider _provider;

		public Db4oLibrarian(Db4oLibraryEnvironmentProvider provider)
		{
			_provider = provider;
		}

		public Db4oLibrary[] Libraries()
		{
            List <Db4oLibrary> libraries = new List<Db4oLibrary>();
			foreach (string path in Directory.GetDirectories(LibraryPath()))
			{
				// comment out the next line to run against legacy versions
				if (!IsVersionOrGreater(path, MinimumVersionToTest))
					continue;

				string db4oLib = FindLibraryFile(path);
				if (null == db4oLib) continue;
				libraries.Add(ForFile(db4oLib));
			}
			return libraries.ToArray();
		}

		private static bool IsVersionOrGreater(string versionName, double minimumVersion)
		{
			return VersionForPath(versionName) >= minimumVersion;
		}

		private static double VersionForPath(string path)
		{	
#if !CF
			double version;
			// TryParse to deal with a possible .svn path
			if (double.TryParse(Path.GetFileName(path), NumberStyles.AllowDecimalPoint | NumberStyles.Float, NumberFormatInfo.InvariantInfo, out version))
				return version;
#endif
			return 0.0;
		}

		public static bool IsLegacyVersion(string versionName)
		{
			return VersionFromVersionName(versionName) < MinimumVersionToTest;
		}

		private static double VersionFromVersionName(string versionName)
		{
			Version version = new Version(versionName);
			return version.Major + version.Minor / 10.0;
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

		private static string FindLibraryFile(string directory)
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