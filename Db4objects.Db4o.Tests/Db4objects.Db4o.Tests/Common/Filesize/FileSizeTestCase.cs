/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Filesize;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Filesize
{
	public class FileSizeTestCase : FileSizeTestCaseBase
	{
		private const int ITERATIONS = 100;

		public static void Main(string[] args)
		{
			new FileSizeTestCase().RunEmbeddedClientServer();
		}

		public virtual void TestConsistentSizeOnRollback()
		{
			StoreSomeItems();
			ProduceSomeFreeSpace();
			AssertConsistentSize(new _AnonymousInnerClass19(this));
		}

		private sealed class _AnonymousInnerClass19 : IRunnable
		{
			public _AnonymousInnerClass19(FileSizeTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Store(new FileSizeTestCaseBase.Item());
				this._enclosing.Db().Rollback();
			}

			private readonly FileSizeTestCase _enclosing;
		}

		public virtual void TestConsistentSizeOnCommit()
		{
			StoreSomeItems();
			Db().Commit();
			AssertConsistentSize(new _AnonymousInnerClass30(this));
		}

		private sealed class _AnonymousInnerClass30 : IRunnable
		{
			public _AnonymousInnerClass30(FileSizeTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Db().Commit();
			}

			private readonly FileSizeTestCase _enclosing;
		}

		public virtual void TestConsistentSizeOnUpdate()
		{
			StoreSomeItems();
			ProduceSomeFreeSpace();
			FileSizeTestCaseBase.Item item = new FileSizeTestCaseBase.Item();
			Store(item);
			Db().Commit();
			AssertConsistentSize(new _AnonymousInnerClass43(this, item));
		}

		private sealed class _AnonymousInnerClass43 : IRunnable
		{
			public _AnonymousInnerClass43(FileSizeTestCase _enclosing, FileSizeTestCaseBase.Item
				 item)
			{
				this._enclosing = _enclosing;
				this.item = item;
			}

			public void Run()
			{
				this._enclosing.Store(item);
				this._enclosing.Db().Commit();
			}

			private readonly FileSizeTestCase _enclosing;

			private readonly FileSizeTestCaseBase.Item item;
		}

		public virtual void TestConsistentSizeOnReopen()
		{
			Db().Commit();
			Reopen();
			AssertConsistentSize(new _AnonymousInnerClass54(this));
		}

		private sealed class _AnonymousInnerClass54 : IRunnable
		{
			public _AnonymousInnerClass54(FileSizeTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				try
				{
					this._enclosing.Reopen();
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}

			private readonly FileSizeTestCase _enclosing;
		}

		public virtual void TestConsistentSizeOnUpdateAndReopen()
		{
			ProduceSomeFreeSpace();
			Store(new FileSizeTestCaseBase.Item());
			Db().Commit();
			AssertConsistentSize(new _AnonymousInnerClass69(this));
		}

		private sealed class _AnonymousInnerClass69 : IRunnable
		{
			public _AnonymousInnerClass69(FileSizeTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Store(this._enclosing.RetrieveOnlyInstance(typeof(FileSizeTestCaseBase.Item)
					));
				this._enclosing.Db().Commit();
				try
				{
					this._enclosing.Reopen();
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
				}
			}

			private readonly FileSizeTestCase _enclosing;
		}

		public virtual void AssertConsistentSize(IRunnable runnable)
		{
			for (int i = 0; i < 10; i++)
			{
				runnable.Run();
			}
			int originalFileSize = FileSize();
			for (int i = 0; i < ITERATIONS; i++)
			{
				runnable.Run();
			}
			Assert.AreEqual(originalFileSize, FileSize());
			Sharpen.Runtime.Out.WriteLine(FileSize());
		}
	}
}
