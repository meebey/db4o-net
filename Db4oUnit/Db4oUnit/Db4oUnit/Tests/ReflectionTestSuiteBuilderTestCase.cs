/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class ReflectionTestSuiteBuilderTestCase : ITestCase
	{
		private sealed class ExcludingReflectionTestSuiteBuilder : ReflectionTestSuiteBuilder
		{
			public ExcludingReflectionTestSuiteBuilder(Type[] classes) : base(classes)
			{
			}

			protected override bool IsApplicable(Type clazz)
			{
				return clazz != typeof(ReflectionTestSuiteBuilderTestCase.NotAccepted);
			}
		}

		public class NonTestFixture
		{
		}

		public virtual void TestUnmarkedTestFixture()
		{
			ReflectionTestSuiteBuilder builder = new ReflectionTestSuiteBuilder(typeof(ReflectionTestSuiteBuilderTestCase.NonTestFixture
				));
			AssertFailingTestCase(typeof(ArgumentException), builder);
		}

		public class Accepted : ITestCase
		{
			public virtual void Test()
			{
			}
		}

		public class NotAccepted : ITestCase
		{
			public virtual void Test()
			{
			}
		}

		public virtual void TestNotAcceptedFixture()
		{
			ReflectionTestSuiteBuilder builder = new ReflectionTestSuiteBuilderTestCase.ExcludingReflectionTestSuiteBuilder
				(new Type[] { typeof(ReflectionTestSuiteBuilderTestCase.Accepted), typeof(ReflectionTestSuiteBuilderTestCase.NotAccepted
				) });
			Assert.AreEqual(1, builder.Build().GetTests().Length);
		}

		public class ConstructorThrows : ITestCase
		{
			public static readonly Exception Error = new Exception("no way");

			public ConstructorThrows()
			{
				throw Error;
			}

			public virtual void Test1()
			{
			}

			public virtual void Test2()
			{
			}
		}

		public virtual void TestConstructorFailuresAppearAsFailedTestCases()
		{
			ReflectionTestSuiteBuilder builder = new ReflectionTestSuiteBuilder(typeof(ReflectionTestSuiteBuilderTestCase.ConstructorThrows
				));
			Exception cause = AssertFailingTestCase(typeof(TestException), builder);
			Assert.AreSame(ReflectionTestSuiteBuilderTestCase.ConstructorThrows.Error, ((TestException
				)cause).GetReason());
		}

		private Exception AssertFailingTestCase(Type expectedError, ReflectionTestSuiteBuilder
			 builder)
		{
			TestSuite suite = builder.Build();
			Assert.AreEqual(1, suite.GetTests().Length);
			FailingTest test = (FailingTest)suite.GetTests()[0];
			Assert.AreSame(expectedError, test.Error().GetType());
			return test.Error();
		}
	}
}
