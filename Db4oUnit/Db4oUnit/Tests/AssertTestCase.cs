using Db4oUnit;

namespace Db4oUnit.Tests
{
	public class AssertTestCase : ITestCase
	{
		public virtual void TestAreEqual()
		{
			Assert.AreEqual(true, true);
			Assert.AreEqual(42, 42);
			Assert.AreEqual(42, 42);
			Assert.AreEqual(null, null);
			ExpectFailure(new _AnonymousInnerClass14(this));
			ExpectFailure(new _AnonymousInnerClass19(this));
			ExpectFailure(new _AnonymousInnerClass24(this));
			ExpectFailure(new _AnonymousInnerClass29(this));
		}

		private sealed class _AnonymousInnerClass14 : ICodeBlock
		{
			public _AnonymousInnerClass14(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Assert.AreEqual(true, false);
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass19 : ICodeBlock
		{
			public _AnonymousInnerClass19(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Assert.AreEqual(42, 43);
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass24 : ICodeBlock
		{
			public _AnonymousInnerClass24(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Assert.AreEqual(new object(), new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass29 : ICodeBlock
		{
			public _AnonymousInnerClass29(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Assert.AreEqual(null, new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		public virtual void TestAreSame()
		{
			ExpectFailure(new _AnonymousInnerClass37(this));
			Assert.AreSame(this, this);
		}

		private sealed class _AnonymousInnerClass37 : ICodeBlock
		{
			public _AnonymousInnerClass37(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Assert.AreSame(new object(), new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		private void ExpectFailure(ICodeBlock block)
		{
			Assert.Expect(typeof(AssertionException), block);
		}
	}
}
