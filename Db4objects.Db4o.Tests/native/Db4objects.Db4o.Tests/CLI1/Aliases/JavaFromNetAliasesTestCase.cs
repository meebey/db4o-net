using System;
using System.IO;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Util;
using Db4oUnit.Extensions.Fixtures;

namespace Db4objects.Db4o.Tests.CLI1.Aliases
{
    /// <summary>
    /// </summary>
    public class JavaFromNetAliasesTestCase : BaseAliasesTestCase, IOptOutCS
    {
#if !CF_1_0 && !CF_2_0
        public void TestAccessingJavaFromDotnet()
        {
            if (!ShouldRunJavaFromDotnetTest())
            {
                return;
            }

            GenerateJavaData();
            using (IObjectContainer container = OpenJavaDataFile())
            {
                AssertAliasedData(container);
            }
        }

        private bool ShouldRunJavaFromDotnetTest()
        {
            if (null == WorkspaceServices.WorkspaceRoot)
            {
                Console.WriteLine("'db4obuild' directory not found, skipping java compatibility test.");
                return false;
            }
            return true;
        }

        private void ConfigureAliases(IConfiguration configuration)
        {
            configuration.AddAlias(new TypeAlias("com.db4o.test.aliases.Person2", GetTypeName(GetAliasedDataType())));
            //	        configuration.AddAlias(
            //	            new WildcardAlias(
            //	                "com.db4o.test.aliases.*",
            //	                CurrentNamespace + ".*, " + CurrentAssemblyName));
            configuration.AddAlias(
                new TypeAlias("com.db4o.ext.Db4oDatabase", "Db4objects.Db4o.Ext.Db4oDatabase, Db4objects.Db4o"));
        }

        private IObjectContainer OpenJavaDataFile()
        {
            IConfiguration configuration = Db4oFactory.NewConfiguration();
            ConfigureAliases(configuration);
            return Db4oFactory.OpenFile(configuration, GetJavaDataFile());
        }

        private String GetJavaDataFile()
        {
            return IOServices.BuildTempPath("java.yap");
        }

        private void GenerateJavaData()
        {
            File.Delete(GetJavaDataFile());
            GenerateClassFile();
            string stdout = IOServices.Exec("java", "-cp " + Quote(JavaTempPath()) + Path.PathSeparator + WorkspaceServices.Db4ojarPath(), "com.db4o.test.aliases.Program", Quote(GetJavaDataFile()));
            Console.WriteLine(stdout);
        }

        private static string JavaTempPath()
        {
            return IOServices.BuildTempPath("aliases");
        }

        private void GenerateClassFile()
        {
            String code = @"
package com.db4o.test.aliases;

import com.db4o.*;

class Person2 {
	String _name;
	public Person2(String name) {
		_name = name;
	}
}

public class Program {
	public static void main(String[] args) {
		String fname = args[0];
		ObjectContainer container = Db4o.openFile(fname);
		container.set(new Person2(""Homer Simpson""));
		container.set(new Person2(""John Cleese""));
		container.close();
		System.out.println(""success"");
	}
}";

            string tempPath = JavaTempPath();
            if (Directory.Exists(tempPath)) Directory.Delete(tempPath, true);
            string srcFile = Path.Combine(tempPath, "com/db4o/test/aliases/Program.java");
            IOServices.WriteFile(srcFile, code);
            string stdout = IOServices.Exec(WorkspaceServices.JavacPath(), "-classpath " + WorkspaceServices.Db4ojarPath(), Quote(srcFile));
            Console.WriteLine(stdout);
        }

        static string Quote(string s)
        {
            return "\"" + s + "\"";
        }
#endif 

        private string GetTypeName(Type type)
        {
            return type.FullName + ", " + CurrentAssemblyName;
        }

        private string CurrentAssemblyName
        {
            get { return GetType().Assembly.GetName().Name; }
        }
    }
}
