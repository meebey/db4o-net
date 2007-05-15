/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.IO;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
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
            if (!JavaServices.CanRunJavaCompatibilityTests())
            {
                return;
            }

            GenerateJavaData();
            using (IObjectContainer container = OpenJavaDataFile())
            {
                AssertAliasedData(container);
            }
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

        private static string GetJavaDataFile()
        {
            return IOServices.BuildTempPath("java.yap");
        }

        private void GenerateJavaData()
        {
            DeleteOldDataFile();
            CompileJavaProgram();
        	RunJavaProgram();
        }

    	private static void RunJavaProgram()
    	{
    		string stdout = JavaServices.java("com.db4o.test.aliases.Program", JavaServices.Quote(GetJavaDataFile()));
    		Console.WriteLine(stdout);
    	}

    	private static void DeleteOldDataFile()
    	{
    		File.Delete(GetJavaDataFile());
    	}

    	private void CompileJavaProgram()
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

    		JavaServices.ResetJavaTempPath();
    		string stdout = JavaServices.CompileJavaCode("com/db4o/test/aliases/Program.java", code);
    		Console.WriteLine(stdout);
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
