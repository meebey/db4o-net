/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

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
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_18(this));
		}

		private sealed class _ICodeBlock_18 : ICodeBlock
		{
			public _ICodeBlock_18(DatabaseReadonlyExceptionTestCase _enclosing)
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
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_27(this));
		}

		private sealed class _ICodeBlock_27 : ICodeBlock
		{
			public _ICodeBlock_27(DatabaseReadonlyExceptionTestCase _enclosing)
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
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_36(this));
		}

		private sealed class _ICodeBlock_36 : ICodeBlock
		{
			public _ICodeBlock_36(DatabaseReadonlyExceptionTestCase _enclosing)
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
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_45(this));
		}

		private sealed class _ICodeBlock_45 : ICodeBlock
		{
			public _ICodeBlock_45(DatabaseReadonlyExceptionTestCase _enclosing)
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
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_53(this));
		}

		private sealed class _ICodeBlock_53 : ICodeBlock
		{
			public _ICodeBlock_53(DatabaseReadonlyExceptionTestCase _enclosing)
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
			Assert.Expect(exceptionType, new _ICodeBlock_67(this));
		}

		private sealed class _ICodeBlock_67 : ICodeBlock
		{
			public _ICodeBlock_67(DatabaseReadonlyExceptionTestCase _enclosing)
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
