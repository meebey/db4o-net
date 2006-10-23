namespace Db4oUnit.Tests
{
	public class ReflectionTestSuiteBuilderTestCase : Db4oUnit.ITestCase
	{
		private sealed class ExcludingReflectionTestSuiteBuilder : Db4oUnit.ReflectionTestSuiteBuilder
		{
			public ExcludingReflectionTestSuiteBuilder(System.Type[] classes) : base(classes)
			{
			}

			protected override bool IsApplicable(System.Type clazz)
			{
				return clazz != typeof(Db4oUnit.Tests.ReflectionTestSuiteBuilderTestCase.NotAccepted);
			}
		}

		public class NonTestFixture
		{
		}

		public virtual void TestUnmarkedTestFixture()
		{
			Db4oUnit.ReflectionTestSuiteBuilder builder = new Db4oUnit.ReflectionTestSuiteBuilder
				(typeof(Db4oUnit.Tests.ReflectionTestSuiteBuilderTestCase.NonTestFixture));
			Db4oUnit.Assert.Expect(typeof(System.ArgumentException), new _AnonymousInnerClass28
				(this, builder));
		}

		private sealed class _AnonymousInnerClass28 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass28(ReflectionTestSuiteBuilderTestCase _enclosing, Db4oUnit.ReflectionTestSuiteBuilder
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

			private readonly Db4oUnit.ReflectionTestSuiteBuilder builder;
		}

		public class Accepted : Db4oUnit.ITestCase
		{
			public virtual void Test()
			{
			}
		}

		public class NotAccepted : Db4oUnit.ITestCase
		{
			public virtual void Test()
			{
			}
		}

		public virtual void TestNotAcceptedFixture()
		{
			Db4oUnit.ReflectionTestSuiteBuilder builder = new Db4oUnit.Tests.ReflectionTestSuiteBuilderTestCase.ExcludingReflectionTestSuiteBuilder
				(new System.Type[] { typeof(Db4oUnit.Tests.ReflectionTestSuiteBuilderTestCase.Accepted)
				, typeof(Db4oUnit.Tests.ReflectionTestSuiteBuilderTestCase.NotAccepted) });
			Db4oUnit.Assert.AreEqual(1, builder.Build().GetTests().Length);
		}
	}
}
