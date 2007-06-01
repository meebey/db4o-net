/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.IO;
using System.Text;

namespace Db4objects.Db4o.Tests.Util
{
	class JavaServices
	{
		public static bool CanRunJavaCompatibilityTests()
		{
#if CF_1_0 || CF_2_0 
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

		public static string Db4ojarPath()
		{
			string db4oVersion = string.Format("{0}.{1}", Db4oVersion.MAJOR, Db4oVersion.MINOR);
			return WorkspaceServices.WorkspacePath("db4obuild/dist/java/lib/db4o-" + db4oVersion + "-java1.2.jar");
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
#if CF_1_0 || CF_2_0 
            return null;
#else
            return IOServices.Exec(WorkspaceServices.JavacPath(),
                    "-classpath",
                    JavaServices.Db4ojarPath(),
                    srcFile);
#endif
        }

		public static string java(string className, params string[] args)
		{
#if CF_1_0 || CF_2_0
            return null;
#else
            return IOServices.Exec("java",
                    "-cp",
                    IOServices.JoinQuotedArgs(Path.PathSeparator, JavaServices.JavaTempPath, Db4ojarPath()),
                    className,
                    IOServices.JoinQuotedArgs(args));
#endif
        }
	}
}
