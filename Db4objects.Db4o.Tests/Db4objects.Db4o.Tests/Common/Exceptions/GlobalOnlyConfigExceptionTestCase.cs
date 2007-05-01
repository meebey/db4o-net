using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class GlobalOnlyConfigExceptionTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new GlobalOnlyConfigExceptionTestCase().RunAll();
		}

		public virtual void TestBlockSize()
		{
			Assert.Expect(typeof(ArgumentException), new _AnonymousInnerClass16(this));
			Assert.Expect(typeof(ArgumentException), new _AnonymousInnerClass22(this));
			Assert.Expect(typeof(GlobalOnlyConfigException), new _AnonymousInnerClass28(this)
				);
		}

		private sealed class _AnonymousInnerClass16 : ICodeBlock
		{
			public _AnonymousInnerClass16(GlobalOnlyConfigExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Configure().BlockSize(-1);
			}

			private readonly GlobalOnlyConfigExceptionTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass22 : ICodeBlock
		{
			public _AnonymousInnerClass22(GlobalOnlyConfigExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Configure().BlockSize(128);
			}

			private readonly GlobalOnlyConfigExceptionTestCase _enclosing;
		}

		private sealed class _AnonymousInnerClass28 : ICodeBlock
		{
			public _AnonymousInnerClass28(GlobalOnlyConfigExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Configure().BlockSize(12);
			}

			private readonly GlobalOnlyConfigExceptionTestCase _enclosing;
		}
	}
}
