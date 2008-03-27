/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using System.Reflection;
using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit
{
	public class ReflectionTestSuiteBuilder : ITestSuiteBuilder
	{
		public static object GetTestSubject(ITest test)
		{
			return ((TestMethod)Undecorate(test)).GetSubject();
		}

		private static ITest Undecorate(ITest test)
		{
			while (test is ITestDecoration)
			{
				test = ((ITestDecoration)test).Test();
			}
			return test;
		}

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

		public virtual IEnumerator GetEnumerator()
		{
			return Iterators.Flatten(Iterators.Map(_classes, new _IFunction4_35(this)));
		}

		private sealed class _IFunction4_35 : IFunction4
		{
			public _IFunction4_35(ReflectionTestSuiteBuilder _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Apply(object arg)
			{
				return this._enclosing.FromClass((Type)arg);
			}

			private readonly ReflectionTestSuiteBuilder _enclosing;
		}

		protected virtual IEnumerator FromClass(Type clazz)
		{
			try
			{
				return SuiteFor(clazz);
			}
			catch (Exception e)
			{
				return Iterators.Cons(new FailingTest(clazz.FullName, e)).GetEnumerator();
			}
		}

		private IEnumerator SuiteFor(Type clazz)
		{
			if (!IsApplicable(clazz))
			{
				TestPlatform.EmitWarning("DISABLED: " + clazz.FullName);
				return Iterators.EmptyIterator;
			}
			if (typeof(ITestSuiteBuilder).IsAssignableFrom(clazz))
			{
				return ((ITestSuiteBuilder)NewInstance(clazz)).GetEnumerator();
			}
			if (typeof(ITest).IsAssignableFrom(clazz))
			{
				return Iterators.IterateSingle(NewInstance(clazz));
			}
			ValidateTestClass(clazz);
			return FromMethods(clazz);
		}

		private void ValidateTestClass(Type clazz)
		{
			if (!(typeof(ITestCase).IsAssignableFrom(clazz)))
			{
				throw new ArgumentException(string.Empty + clazz + " is not marked as " + typeof(
					ITestCase));
			}
		}

		protected virtual bool IsApplicable(Type clazz)
		{
			return clazz != null;
		}

		// just removing the 'parameter not used' warning
		private IEnumerator FromMethods(Type clazz)
		{
			return Iterators.Map(clazz.GetMethods(), new _IFunction4_77(this, clazz));
		}

		private sealed class _IFunction4_77 : IFunction4
		{
			public _IFunction4_77(ReflectionTestSuiteBuilder _enclosing, Type clazz)
			{
				this._enclosing = _enclosing;
				this.clazz = clazz;
			}

			public object Apply(object arg)
			{
				MethodInfo method = (MethodInfo)arg;
				if (!this._enclosing.IsTestMethod(method))
				{
					this._enclosing.EmitWarningOnIgnoredTestMethod(clazz, method);
					return Iterators.Skip;
				}
				return this._enclosing.FromMethod(clazz, method);
			}

			private readonly ReflectionTestSuiteBuilder _enclosing;

			private readonly Type clazz;
		}

		private void EmitWarningOnIgnoredTestMethod(Type clazz, MethodInfo method)
		{
			if (!StartsWithIgnoreCase(method.Name, "_test"))
			{
				return;
			}
			TestPlatform.EmitWarning("IGNORED: " + CreateTest(NewInstance(clazz), method).GetLabel
				());
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

		protected virtual ITest FromMethod(Type clazz, MethodInfo method)
		{
			return new DeferredTest(new _ITestFactory_124(this, clazz, method));
		}

		private sealed class _ITestFactory_124 : ITestFactory
		{
			public _ITestFactory_124(ReflectionTestSuiteBuilder _enclosing, Type clazz, MethodInfo
				 method)
			{
				this._enclosing = _enclosing;
				this.clazz = clazz;
				this.method = method;
			}

			public ITest NewInstance()
			{
				return this._enclosing.CreateTest(this._enclosing.NewInstance(clazz), method);
			}

			private readonly ReflectionTestSuiteBuilder _enclosing;

			private readonly Type clazz;

			private readonly MethodInfo method;
		}
	}
}
