namespace Db4oUnit
{
	public class ReflectionTestSuiteBuilder : Db4oUnit.ITestSuiteBuilder
	{
		private System.Type[] _classes;

		public ReflectionTestSuiteBuilder(System.Type clazz)
		{
			if (null == clazz)
			{
				throw new System.ArgumentException("clazz");
			}
			_classes = new System.Type[] { clazz };
		}

		public ReflectionTestSuiteBuilder(System.Type[] classes)
		{
			if (null == classes)
			{
				throw new System.ArgumentException("classes");
			}
			_classes = classes;
		}

		public virtual Db4oUnit.TestSuite Build()
		{
			return (1 == _classes.Length) ? FromClass(_classes[0]) : FromClasses(_classes);
		}

		protected virtual Db4oUnit.TestSuite FromClasses(System.Type[] classes)
		{
			System.Collections.ArrayList suites = new System.Collections.ArrayList(classes.Length
				);
			for (int i = 0; i < classes.Length; i++)
			{
				Db4oUnit.TestSuite suite = FromClass(classes[i]);
				if (suite.GetTests().Length > 0)
				{
					suites.Add(suite);
				}
			}
			return new Db4oUnit.TestSuite(ToTestArray(suites));
		}

		protected virtual Db4oUnit.TestSuite FromClass(System.Type clazz)
		{
			if (!IsApplicable(clazz))
			{
				Db4oUnit.TestPlatform.EmitWarning("DISABLED: " + clazz.FullName);
				return new Db4oUnit.TestSuite(new Db4oUnit.ITest[0]);
			}
			if (typeof(Db4oUnit.ITestSuiteBuilder).IsAssignableFrom(clazz))
			{
				return ((Db4oUnit.ITestSuiteBuilder)NewInstance(clazz)).Build();
			}
			if (typeof(Db4oUnit.ITest).IsAssignableFrom(clazz))
			{
				return new Db4oUnit.TestSuite(clazz.FullName, new Db4oUnit.ITest[] { (Db4oUnit.ITest
					)NewInstance(clazz) });
			}
			if (!(typeof(Db4oUnit.ITestCase).IsAssignableFrom(clazz)))
			{
				throw new System.ArgumentException(string.Empty + clazz + " is not marked as " + 
					typeof(Db4oUnit.ITestCase));
			}
			return FromMethods(clazz);
		}

		protected virtual bool IsApplicable(System.Type clazz)
		{
			return clazz != null;
		}

		private Db4oUnit.TestSuite FromMethods(System.Type clazz)
		{
			System.Collections.ArrayList tests = new System.Collections.ArrayList();
			System.Reflection.MethodInfo[] methods = clazz.GetMethods();
			for (int i = 0; i < methods.Length; i++)
			{
				object instance = NewInstance(clazz);
				System.Reflection.MethodInfo method = methods[i];
				if (!IsTestMethod(method))
				{
					EmitWarningOnIgnoredTestMethod(instance, method);
					continue;
				}
				tests.Add(CreateTest(instance, method));
			}
			return new Db4oUnit.TestSuite(clazz.FullName, ToTestArray(tests));
		}

		private void EmitWarningOnIgnoredTestMethod(object subject, System.Reflection.MethodInfo
			 method)
		{
			if (!StartsWithIgnoreCase(method.Name, "_test"))
			{
				return;
			}
			Db4oUnit.TestPlatform.EmitWarning("IGNORED: " + CreateTest(subject, method).GetLabel
				());
		}

		protected virtual bool IsTestMethod(System.Reflection.MethodInfo method)
		{
			return HasTestPrefix(method) && Db4oUnit.TestPlatform.IsPublic(method) && !Db4oUnit.TestPlatform
				.IsStatic(method) && !Db4oUnit.TestPlatform.HasParameters(method);
		}

		private bool HasTestPrefix(System.Reflection.MethodInfo method)
		{
			return StartsWithIgnoreCase(method.Name, "test");
		}

		protected virtual bool StartsWithIgnoreCase(string s, string prefix)
		{
			return s.ToUpper().StartsWith(prefix.ToUpper());
		}

		private static Db4oUnit.ITest[] ToTestArray(System.Collections.ArrayList tests)
		{
			Db4oUnit.ITest[] array = new Db4oUnit.ITest[tests.Count];
			tests.CopyTo(array);
			return array;
		}

		protected virtual object NewInstance(System.Type clazz)
		{
			try
			{
				return System.Activator.CreateInstance(clazz);
			}
			catch (System.Exception e)
			{
				throw new Db4oUnit.TestException(e);
			}
		}

		protected virtual Db4oUnit.ITest CreateTest(object instance, System.Reflection.MethodInfo
			 method)
		{
			return new Db4oUnit.TestMethod(instance, method);
		}
	}
}
