using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
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
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass17(this));
		}

		private sealed class _AnonymousInnerClass17 : ICodeBlock
		{
			public _AnonymousInnerClass17(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Rollback();
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestCommit()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass26(this));
		}

		private sealed class _AnonymousInnerClass26 : ICodeBlock
		{
			public _AnonymousInnerClass26(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestSet()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass35(this));
		}

		private sealed class _AnonymousInnerClass35 : ICodeBlock
		{
			public _AnonymousInnerClass35(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Set(new Item());
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestDelete()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass44(this));
		}

		private sealed class _AnonymousInnerClass44 : ICodeBlock
		{
			public _AnonymousInnerClass44(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Delete(null);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestQueryClass()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass53(this));
		}

		private sealed class _AnonymousInnerClass53 : ICodeBlock
		{
			public _AnonymousInnerClass53(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Query(this.GetType());
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestQuery()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass62(this));
		}

		private sealed class _AnonymousInnerClass62 : ICodeBlock
		{
			public _AnonymousInnerClass62(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Query();
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestDeactivate()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass71(this));
		}

		private sealed class _AnonymousInnerClass71 : ICodeBlock
		{
			public _AnonymousInnerClass71(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Deactivate(null, 1);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestActivate()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass80(this));
		}

		private sealed class _AnonymousInnerClass80 : ICodeBlock
		{
			public _AnonymousInnerClass80(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Activate(null, 1);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}

		public virtual void TestGet()
		{
			Db().Close();
			Assert.Expect(typeof(DatabaseClosedException), new _AnonymousInnerClass89(this));
		}

		private sealed class _AnonymousInnerClass89 : ICodeBlock
		{
			public _AnonymousInnerClass89(DatabaseClosedExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Get(null);
			}

			private readonly DatabaseClosedExceptionTestCase _enclosing;
		}
	}
}
