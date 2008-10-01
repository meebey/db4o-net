/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.IO;
using System.Text;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.Util
{
	class JavaServices
	{
		public static bool CanRunJavaCompatibilityTests()
		{
#if CF 
			return false;
#else
			if (null == WorkspaceServices.WorkspaceRoot)
			{
				Console.WriteLine("'db4obuild' directory not found, skipping java compatibility test.");
				return false;
			}
			return true;
#endif
		}

        public static string Db4oCoreJarPath()
        {
            return Db4oJarPath("-core");
        }

		public static string Db4oJarPath(string extension)
		{
			string db4oVersion = string.Format("{0}.{1}.{2}.{3}", Db4oVersion.Major, Db4oVersion.Minor,
                Db4oVersion.Iteration, Db4oVersion.Revision);
			string distDir = WorkspaceServices.ReadProperty(WorkspaceServices.MachinePropertiesPath(), "dir.dist", true);
			if(distDir == null || distDir.Length == 0)
			{
				distDir = "db4obuild/dist";
			}
			return WorkspaceServices.WorkspacePath(distDir + "/java/lib/db4o-" + db4oVersion + extension + "-java1.2.jar");
		}

		public static string JavaTempPath
		{
			get { return IOServices.BuildTempPath("java"); }
		}

		public static void ResetJavaTempPath()
		{
			string tempPath = JavaServices.JavaTempPath;
			if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
		}

		public static string CompileJavaCode(string fname, string code)
		{
			string srcFile = Path.Combine(JavaServices.JavaTempPath, fname);
			IOServices.WriteFile(srcFile, code);
			return javac(srcFile);
		}

		public static string javac(string srcFile)
		{
#if CF 
            return null;
#else
            Assert.IsTrue(File.Exists(JavaServices.Db4oCoreJarPath()), string.Format("'{0}' not found. Make sure the jar was built before running this test.", JavaServices.Db4oCoreJarPath()));
            Assert.IsTrue(File.Exists(JavaServices.Db4oJarPath("-optional")), string.Format("'{0}' not found. Make sure the jar was built before running this test.", JavaServices.Db4oJarPath("-optional")));
            return IOServices.Exec(WorkspaceServices.JavacPath(),
                    "-classpath",
                    IOServices.JoinQuotedArgs(Path.PathSeparator, JavaServices.Db4oCoreJarPath(), JavaServices.Db4oJarPath("-optional")),
                    srcFile);
#endif
		}

		public static string java(string className, params string[] args)
		{
#if CF
            return null;
#else
            return IOServices.Exec(WorkspaceServices.JavaPath(),
                    "-cp",
                    IOServices.JoinQuotedArgs(Path.PathSeparator, JavaServices.JavaTempPath, Db4oCoreJarPath(), JavaServices.Db4oJarPath("-optional")),
                    className,
                    IOServices.JoinQuotedArgs(args));
#endif
        }
	}
}
