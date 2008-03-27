/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class DatabaseReadonlyExceptionTestCase : AbstractDb4oTestCase, IOptOutTA
	{
		public static void Main(string[] args)
		{
			new DatabaseReadonlyExceptionTestCase().RunAll();
		}

		public virtual void TestRollback()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_20(this));
		}

		private sealed class _ICodeBlock_20 : ICodeBlock
		{
			public _ICodeBlock_20(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Rollback();
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestCommit()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_29(this));
		}

		private sealed class _ICodeBlock_29 : ICodeBlock
		{
			public _ICodeBlock_29(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestSet()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_38(this));
		}

		private sealed class _ICodeBlock_38 : ICodeBlock
		{
			public _ICodeBlock_38(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Store(new Item());
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestDelete()
		{
			ConfigReadOnly();
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_47(this));
		}

		private sealed class _ICodeBlock_47 : ICodeBlock
		{
			public _ICodeBlock_47(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Delete(null);
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestNewFile()
		{
			Assert.Expect(typeof(DatabaseReadOnlyException), new _ICodeBlock_55(this));
		}

		private sealed class _ICodeBlock_55 : ICodeBlock
		{
			public _ICodeBlock_55(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Fixture().Close();
				this._enclosing.Fixture().Clean();
				this._enclosing.Fixture().Config().ReadOnly(true);
				this._enclosing.Fixture().Open(this.GetType());
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestReserveStorage()
		{
			ConfigReadOnly();
			Type exceptionType = IsClientServer() && !IsMTOC() ? typeof(NotSupportedException
				) : typeof(DatabaseReadOnlyException);
			Assert.Expect(exceptionType, new _ICodeBlock_69(this));
		}

		private sealed class _ICodeBlock_69 : ICodeBlock
		{
			public _ICodeBlock_69(DatabaseReadonlyExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Configure().ReserveStorageSpace(1);
			}

			private readonly DatabaseReadonlyExceptionTestCase _enclosing;
		}

		public virtual void TestStoredClasses()
		{
			ConfigReadOnly();
			Db().StoredClasses();
		}

		private void ConfigReadOnly()
		{
			Db().Configure().ReadOnly(true);
		}
	}
}
