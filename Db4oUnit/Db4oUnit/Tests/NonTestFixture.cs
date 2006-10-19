namespace Db4oUnit.Tests
{
	public class ReflectionTestSuiteBuilderTestCase : Db4oUnit.ITestCase
	{
		public class NonTestFixture
		{
		}

		public virtual void TestUnmarkedTestFixture()
		{
			Db4oUnit.ReflectionTestSuiteBuilder builder = new Db4oUnit.ReflectionTestSuiteBuilder
				(typeof(Db4oUnit.Tests.ReflectionTestSuiteBuilderTestCase.NonTestFixture));
			Db4oUnit.Assert.Expect(typeof(System.ArgumentException), new _AnonymousInnerClass17
				(this, builder));
		}

		private sealed class _AnonymousInnerClass17 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass17(ReflectionTestSuiteBuilderTestCase _enclosing, Db4oUnit.ReflectionTestSuiteBuilder
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
	}
}
