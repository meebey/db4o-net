using System;
using System.IO;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.Util
{
	class WorkspaceServices
	{
		public static string JavacPath()
		{
			string path = ReadProperty(MachinePropertiesPath(), "file.compiler.jdk1.3");
			Assert.IsTrue(File.Exists(path));
			return path;
		}

		public static string ReadProperty(string fname, string property)
		{
			using (StreamReader reader = File.OpenText(fname))
			{
				string line = null;
				while (null != (line = reader.ReadLine()))
				{
					if (line.StartsWith(property))
					{
						return line.Substring(property.Length + 1);
					}
				}
			}
			throw new ArgumentException("property '" + property + "' not found in '" + fname + "'");
		}

		public static string MachinePropertiesPath()
		{
			string path = WorkspacePath("db4obuild/machine.properties");
			Assert.IsTrue(File.Exists(path));
			return path;
		}

		public static String Db4ojarPath()
		{
			string db4oVersion = string.Format("{0}.{1}", Db4oVersion.MAJOR, Db4oVersion.MINOR);
			return WorkspacePath("db4obuild/dist/java/lib/db4o-" + db4oVersion + "-java1.2.jar");
		}

		public static string WorkspacePath(string fname)
		{
			string root = WorkspaceRoot;
			return null == root ? null : Path.Combine(root, fname);
		}
		
		public static string WorkspaceTestFilePath(string fname)
		{
			string testFolder = WorkspaceLocations.TEST_FOLDER;
			if (testFolder == null) return null;
			return Path.Combine(testFolder, fname);
		}

		public static string WorkspaceRoot
		{
			get { return IOServices.FindParentDirectory("db4obuild"); }
		}
	}
}
