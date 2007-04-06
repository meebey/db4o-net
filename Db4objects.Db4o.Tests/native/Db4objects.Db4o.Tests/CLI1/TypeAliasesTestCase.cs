using System;
using System.Diagnostics;
using System.IO;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Util;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
	public class Person1
	{
		private String _name;

		public Person1(String name)
		{
			_name = name;
		}

		public String Name
		{
			get { return _name; }
		}
	}

	public class Person2
	{
		private String _name;

		public Person2(String name)
		{
			_name = name;
		}

		public String Name
		{
			get { return _name; }
		}

		public override bool Equals(object obj)
		{
			Person2 other = obj as Person2;
			if (null == other) return false;
			return CFHelper.AreEqual(_name, other._name);
		}
	}

    class CFHelper
	{
		public static bool AreEqual(object l, object r)
		{
			if (l == r) return true;
			if (l == null || r == null) return false;
			return l.Equals(r);
		}
	}

	/// <summary>
	/// </summary>
	public class TypeAliasesTestCase : AbstractDb4oTestCase
	{
		public void TestTypeAlias()
		{
		    Db().Set(new Person1("Homer Simpson"));
			Db().Set(new Person1("John Cleese"));

			Reopen();
			Db().Ext().Configure().AddAlias(
				// Person1 instances should be read as Person2 objects
				new TypeAlias(
				GetTypeName(typeof(Person1)),
				GetTypeName(typeof(Person2))));
			AssertData(Db());
		}

	    private string GetTypeName(Type type)
	    {
	        return type.FullName + ", " + CurrentAssemblyName;
	    }

        private string CurrentAssemblyName
        {
            get { return GetType().Assembly.GetName().Name; }
        }

        private string CurrentNamespace
        {
            get { return GetType().Namespace; }
        }

        private void AssertData(IObjectContainer container)
		{
			IObjectSet os = container.Get(typeof(Person2));
			Assert.AreEqual(2, os.Size());
			EnsureContains(os, new Person2("Homer Simpson"));
			EnsureContains(os, new Person2("John Cleese"));
		}

#if !CF_1_0 && !CF_2_0
		public void TestAccessingJavaFromDotnet()
		{	
			if (IsClientServer()) return;
			
			if (null == WorkspaceServices.WorkspaceRoot)
			{
				Console.WriteLine("'db4obuild' directory not found, skipping java compatibility test.");
				return;
			}

			GenerateJavaData();
			using (IObjectContainer container = OpenJavaDataFile())
			{
				container.Ext().Configure().AddAlias(
					new WildcardAlias(
					"com.db4o.test.aliases.*",
					CurrentNamespace + ".*, " + CurrentAssemblyName));
				AssertData(container);
			}
		}

		private IObjectContainer OpenJavaDataFile()
		{
			return Db4oFactory.OpenFile(GetJavaDataFile());
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

		private void EnsureContains(IObjectSet actual, Object expected)
		{
			actual.Reset();
			while (actual.HasNext())
			{
				Object next = actual.Next();
				if (CFHelper.AreEqual(next, expected)) return;
			}
			Assert.Fail("Expected item: " + expected);
		}

	}
}
