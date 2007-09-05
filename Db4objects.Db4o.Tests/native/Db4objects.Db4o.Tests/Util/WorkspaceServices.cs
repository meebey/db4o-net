/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.IO;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.Util
{
	class WorkspaceServices
	{
		public static string JavacPath()
		{
            return ReadMachinePathProperty("file.compiler.jdk1.3");
		}
		
		public static string JavaPath()
		{
			return ReadMachinePathProperty("file.jvm.jdk1.5");
		}
		
		public static string ReadMachineProperty(string property)
		{
			return ReadProperty(MachinePropertiesPath(), property);
		}
		
		public static string ReadMachinePathProperty(string property)
		{
			string path = ReadMachineProperty(property);
			Assert.IsTrue(File.Exists(path), string.Format("File '{0}' could not be found ({1}).", path, property));
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
            string fileName = Environment.GetEnvironmentVariable("DB4O_MACHINE_PROPERTIES");
            if (fileName == null || fileName.Length == 0)
            {
                fileName = "machine.properties";
            }
			string path = WorkspacePath("db4obuild/" + fileName);
			Assert.IsTrue(File.Exists(path));
			return path;
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
