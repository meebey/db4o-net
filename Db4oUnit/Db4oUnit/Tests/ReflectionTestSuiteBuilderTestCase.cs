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
			ReflectionTestSuiteBuilder builder = new ReflectionTestSuiteBuilder(typeof(ReflectionTestSuiteBuilderTestCase.NonTestFixture)
				);
			Assert.Expect(typeof(ArgumentException), new _AnonymousInnerClass28(this, builder
				));
		}

		private sealed class _AnonymousInnerClass28 : ICodeBlock
		{
			public _AnonymousInnerClass28(ReflectionTestSuiteBuilderTestCase _enclosing, ReflectionTestSuiteBuilder
				 builder)
			{
				this._enclosing = _enclosing;
				this.builder = builder;
			}

			public void Run()
			{
				builder.Build();
			}

			private readonly ReflectionTestSuiteBuilderTestCase _enclosing;

			private readonly ReflectionTestSuiteBuilder builder;
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
				(new Type[] { typeof(ReflectionTestSuiteBuilderTestCase.Accepted), typeof(ReflectionTestSuiteBuilderTestCase.NotAccepted)
				 });
			Assert.AreEqual(1, builder.Build().GetTests().Length);
		}
	}
}
