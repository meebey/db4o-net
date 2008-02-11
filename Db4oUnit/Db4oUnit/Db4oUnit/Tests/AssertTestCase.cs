/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
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
			ExpectFailure(new _ICodeBlock_14(this));
			ExpectFailure(new _ICodeBlock_19(this));
			ExpectFailure(new _ICodeBlock_24(this));
			ExpectFailure(new _ICodeBlock_29(this));
		}

		private sealed class _ICodeBlock_14 : ICodeBlock
		{
			public _ICodeBlock_14(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Assert.AreEqual(true, false);
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _ICodeBlock_19 : ICodeBlock
		{
			public _ICodeBlock_19(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Assert.AreEqual(42, 43);
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _ICodeBlock_24 : ICodeBlock
		{
			public _ICodeBlock_24(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Assert.AreEqual(new object(), new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		private sealed class _ICodeBlock_29 : ICodeBlock
		{
			public _ICodeBlock_29(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				Assert.AreEqual(null, new object());
			}

			private readonly AssertTestCase _enclosing;
		}

		public virtual void TestAreSame()
		{
			ExpectFailure(new _ICodeBlock_37(this));
			Assert.AreSame(this, this);
		}

		private sealed class _ICodeBlock_37 : ICodeBlock
		{
			public _ICodeBlock_37(AssertTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
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
