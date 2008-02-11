/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class DatabaseClosedExceptionTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new DatabaseClosedExceptionTestCase().RunAll();
		}

		public virtual void TestRollback()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_18(this));
		}

		private sealed class _ICodeBlock_18 : ICodeBlock
		{
			public _ICodeBlock_18(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Rollback();
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestCommit()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_27(this));
		}

		private sealed class _ICodeBlock_27 : ICodeBlock
		{
			public _ICodeBlock_27(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestSet()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_36(this));
		}

		private sealed class _ICodeBlock_36 : ICodeBlock
		{
			public _ICodeBlock_36(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Store(new Item());
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestDelete()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_45(this));
		}

		private sealed class _ICodeBlock_45 : ICodeBlock
		{
			public _ICodeBlock_45(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Delete(null);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestQueryClass()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_54(this));
		}

		private sealed class _ICodeBlock_54 : ICodeBlock
		{
			public _ICodeBlock_54(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Query(this.GetType());
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestQuery()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_63(this));
		}

		private sealed class _ICodeBlock_63 : ICodeBlock
		{
			public _ICodeBlock_63(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Query();
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestDeactivate()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_72(this));
		}

		private sealed class _ICodeBlock_72 : ICodeBlock
		{
			public _ICodeBlock_72(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Deactivate(null, 1);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestActivate()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_81(this));
		}

		private sealed class _ICodeBlock_81 : ICodeBlock
		{
			public _ICodeBlock_81(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().Activate(null, 1);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestGet()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _ICodeBlock_90(this));
		}

		private sealed class _ICodeBlock_90 : ICodeBlock
		{
			public _ICodeBlock_90(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			/// <exception cref="Exception"></exception>
			public void Run()
			{
				this._enclosing.Db().QueryByExample(null);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}
	}
}
