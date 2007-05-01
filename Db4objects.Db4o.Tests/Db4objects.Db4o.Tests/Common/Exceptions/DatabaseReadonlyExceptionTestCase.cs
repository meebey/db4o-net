using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class DatabaseReadonlyExceptionTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new DatabaseReadonlyExceptionTestCase().RunAll();
		}

		public virtual void TestRollback()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _AnonymousInnerClass18(this)
				);
		}

		private sealed class _AnonymousInnerClass18 : ICodeBlock
		{
			public _AnonymousInnerClass18(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Rollback();
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestCommit()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _AnonymousInnerClass27(this)
				);
		}

		private sealed class _AnonymousInnerClass27 : ICodeBlock
		{
			public _AnonymousInnerClass27(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestSet()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _AnonymousInnerClass36(this)
				);
		}

		private sealed class _AnonymousInnerClass36 : ICodeBlock
		{
			public _AnonymousInnerClass36(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Set(new Item());
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestDelete()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _AnonymousInnerClass45(this)
				);
		}

		private sealed class _AnonymousInnerClass45 : ICodeBlock
		{
			public _AnonymousInnerClass45(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Delete(null);
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestNewFile()
		{
			Assert.Expect(typeof(DatabaseReadOnlyException), new _AnonymousInnerClass53(this)
				);
		}

		private sealed class _AnonymousInnerClass53 : ICodeBlock
		{
			public _AnonymousInnerClass53(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Fixture().Close();
				this._enclosing.Fixture().Clean();
				this._enclosing.Fixture().Config().ReadOnly(true);
				this._enclosing.Fixture().Open();
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestReserveStorage()
		{
			ConfigReadOnly();
			Type exceptionType = IsClientServer() ? typeof(NotSupportedException) : typeof(DatabaseReadOnlyException);
			Assert.Expect(exceptionType, new _AnonymousInnerClass67(this));
		}

		private sealed class _AnonymousInnerClass67 : ICodeBlock
		{
			public _AnonymousInnerClass67(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Configure().ReserveStorageSpace(1);
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		private void ConfigReadOnly()
		{
			Db().Configure().ReadOnly(true);
		}
	}
}
