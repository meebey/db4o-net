/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Reflection;
using Db4oUnit;

namespace Db4oUnit
{
	public class ReflectionTestSuiteBuilder : ITestSuiteBuilder
	{
		private Type[] _classes;

		public ReflectionTestSuiteBuilder(Type clazz) : this(new Type[] { clazz })
		{
		}

		public ReflectionTestSuiteBuilder(Type[] classes)
		{
			if (null == classes)
			{
				throw new ArgumentException("classes");
			}
			_classes = classes;
		}

		public virtual TestSuite Build()
		{
			return (1 == _classes.Length) ? FromClass(_classes[0]) : FromClasses(_classes);
		}

		protected virtual TestSuite FromClasses(Type[] classes)
		{
			ArrayList suites = new ArrayList(classes.Length);
			for (int i = 0; i < classes.Length; i++)
			{
				TestSuite suite = FromClass(classes[i]);
				if (suite.GetTests().Length > 0)
				{
					suites.Add(suite);
				}
			}
			return new TestSuite(ToTestArray(suites));
		}

		protected virtual TestSuite FromClass(Type clazz)
		{
			try
			{
				return SuiteFor(clazz);
			}
			catch (Exception e)
			{
				return new TestSuite(new FailingTest(clazz.FullName, e));
			}
		}

		private TestSuite SuiteFor(Type clazz)
		{
			if (!IsApplicable(clazz))
			{
				TestPlatform.EmitWarning("DISABLED: " + clazz.FullName);
				return new TestSuite(new ITest[0]);
			}
			if (typeof(ITestSuiteBuilder).IsAssignableFrom(clazz))
			{
				return ((ITestSuiteBuilder)NewInstance(clazz)).Build();
			}
			if (typeof(ITest).IsAssignableFrom(clazz))
			{
				return new TestSuite(clazz.FullName, new ITest[] { (ITest)NewInstance(clazz) });
			}
			if (!(typeof(ITestCase).IsAssignableFrom(clazz)))
			{
				throw new ArgumentException(string.Empty + clazz + " is not marked as " + typeof(
					ITestCase));
			}
			return FromMethods(clazz);
		}

		protected virtual bool IsApplicable(Type clazz)
		{
			return clazz != null;
		}

		// just removing the 'parameter not used' warning
		private TestSuite FromMethods(Type clazz)
		{
			ArrayList tests = new ArrayList();
			MethodInfo[] methods = clazz.GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				object instance = NewInstance(clazz);
				MethodInfo method = methods[i];
				if (!IsTestMethod(method))
				{
					EmitWarningOnIgnoredTestMethod(instance, method);
					continue;
				}
				tests.Add(CreateTest(instance, method));
			}
			return new TestSuite(clazz.FullName, ToTestArray(tests));
		}

		private void EmitWarningOnIgnoredTestMethod(object subject, MethodInfo method)
		{
			if (!StartsWithIgnoreCase(method.Name, "_test"))
			{
				return;
			}
			TestPlatform.EmitWarning("IGNORED: " + CreateTest(subject, method).GetLabel());
		}

		protected virtual bool IsTestMethod(MethodInfo method)
		{
			return HasTestPrefix(method) && TestPlatform.IsPublic(method) && !TestPlatform.IsStatic
				(method) && !TestPlatform.HasParameters(method);
		}

		private bool HasTestPrefix(MethodInfo method)
		{
			return StartsWithIgnoreCase(method.Name, "test");
		}

		protected virtual bool StartsWithIgnoreCase(string s, string prefix)
		{
			return s.ToUpper().StartsWith(prefix.ToUpper());
		}

		private static ITest[] ToTestArray(ArrayList tests)
		{
			ITest[] array = new ITest[tests.Count];
			tests.CopyTo(array);
			return array;
		}

		protected virtual object NewInstance(Type clazz)
		{
			try
			{
				return System.Activator.CreateInstance(clazz);
			}
			catch (Exception e)
			{
				throw new TestException(e);
			}
		}

		protected virtual ITest CreateTest(object instance, MethodInfo method)
		{
			return new TestMethod(instance, method);
		}
	}
}
