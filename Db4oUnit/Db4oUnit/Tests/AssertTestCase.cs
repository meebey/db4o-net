namespace Db4oUnit.Tests
{
	public class AssertTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestAreEqual()
		{
			Db4oUnit.Assert.AreEqual(true, true);
			Db4oUnit.Assert.AreEqual(42, 42);
			Db4oUnit.Assert.AreEqual(42, 42);
			Db4oUnit.Assert.AreEqual(null, null);
			ExpectFailure(new _AnonymousInnerClass14(this));
			ExpectFailure(new _AnonymousInnerClass19(this));
			ExpectFailure(new _AnonymousInnerClass24(this));
			ExpectFailure(new _AnonymousInnerClass29(this));
		}

		private sealed class _AnonymousInnerClass14 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass14(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oUnit.Assert.AreEqual(true, false);
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass19 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass19(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oUnit.Assert.AreEqual(42, 43);
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass24 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass24(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oUnit.Assert.AreEqual(new object(), new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass29 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass29(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oUnit.Assert.AreEqual(null, new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		public virtual void TestAreSame()
		{
			ExpectFailure(new _AnonymousInnerClass37(this));
			Db4oUnit.Assert.AreSame(this, this);
		}

		private sealed class _AnonymousInnerClass37 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass37(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				Db4oUnit.Assert.AreSame(new object(), new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		private void ExpectFailure(Db4oUnit.ICodeBlock block)
		{
			Db4oUnit.Assert.Expect(typeof(Db4oUnit.AssertionException), block);
		}
	}
}
