/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class BackupDb4oIOExceptionTestCase : Db4oIOExceptionTestCaseBase
	{
		public static void Main(string[] args)
		{
			new BackupDb4oIOExceptionTestCase().RunAll();
		}

		private static readonly string BACKUP_FILE = "backup.db4o";

		protected override void Db4oSetupBeforeStore()
		{
			base.Db4oSetupBeforeStore();
			File4.Delete(BACKUP_FILE);
		}

		protected override void Db4oCustomTearDown()
		{
			base.Db4oCustomTearDown();
			File4.Delete(BACKUP_FILE);
		}

		public virtual void TestBackup()
		{
			Assert.Expect(typeof(Db4oIOException), new _AnonymousInnerClass28(this));
		}

		private sealed class _AnonymousInnerClass28 : ICodeBlock
		{
			public _AnonymousInnerClass28(BackupDb4oIOExceptionTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				ExceptionIOAdapter.exception = true;
				this._enclosing.Db().Backup(BackupDb4oIOExceptionTestCase.BACKUP_FILE);
			}

			private readonly BackupDb4oIOExceptionTestCase _enclosing;
		}
	}
}
