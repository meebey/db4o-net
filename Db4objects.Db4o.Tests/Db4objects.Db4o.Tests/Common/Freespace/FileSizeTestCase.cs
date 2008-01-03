/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Tests.Common.Freespace;
using Sharpen.Lang;

namespace Db4objects.Db4o.Tests.Common.Freespace
{
	public class FileSizeTestCase : FreespaceManagerTestCaseBase, IOptOutDefragSolo
	{
		private const int Iterations = 100;

		public static void Main(string[] args)
		{
			new FileSizeTestCase().RunEmbeddedClientServer();
		}

		public virtual void TestConsistentSizeOnRollback()
		{
			StoreSomeItems();
			ProduceSomeFreeSpace();
			AssertConsistentSize(new _IRunnable_21(this));
		}

		private sealed class _IRunnable_21 : IRunnable
		{
			public _IRunnable_21(FileSizeTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Store(new FreespaceManagerTestCaseBase.Item());
				this._enclosing.Db().Rollback();
			}

			private readonly FileSizeTestCase _enclosing;
		}

		public virtual void TestConsistentSizeOnCommit()
		{
			StoreSomeItems();
			Db().Commit();
			AssertConsistentSize(new _IRunnable_32(this));
		}

		private sealed class _IRunnable_32 : IRunnable
		{
			public _IRunnable_32(FileSizeTestCase _enclosing)
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
			FreespaceManagerTestCaseBase.Item item = new FreespaceManagerTestCaseBase.Item();
			Store(item);
			Db().Commit();
			AssertConsistentSize(new _IRunnable_45(this, item));
		}

		private sealed class _IRunnable_45 : IRunnable
		{
			public _IRunnable_45(FileSizeTestCase _enclosing, FreespaceManagerTestCaseBase.Item
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

			private readonly FreespaceManagerTestCaseBase.Item item;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestConsistentSizeOnReopen()
		{
			Db().Commit();
			Reopen();
			AssertConsistentSize(new _IRunnable_56(this));
		}

		private sealed class _IRunnable_56 : IRunnable
		{
			public _IRunnable_56(FileSizeTestCase _enclosing)
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

		/// <exception cref="Exception"></exception>
		public virtual void TestConsistentSizeOnUpdateAndReopen()
		{
			ProduceSomeFreeSpace();
			Store(new FreespaceManagerTestCaseBase.Item());
			Db().Commit();
			AssertConsistentSize(new _IRunnable_71(this));
		}

		private sealed class _IRunnable_71 : IRunnable
		{
			public _IRunnable_71(FileSizeTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.Store(this._enclosing.RetrieveOnlyInstance(typeof(FreespaceManagerTestCaseBase.Item
					)));
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
			int originalFileSize = DatabaseFileSize();
			for (int i = 0; i < Iterations; i++)
			{
				runnable.Run();
			}
			Assert.AreEqual(originalFileSize, DatabaseFileSize());
		}
	}
}
