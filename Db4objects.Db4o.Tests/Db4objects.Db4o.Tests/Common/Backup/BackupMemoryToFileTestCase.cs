/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Backup;

namespace Db4objects.Db4o.Tests.Common.Backup
{
	public class BackupMemoryToFileTestCase : MemoryBackupTestCaseBase
	{
		protected override void Backup(LocalObjectContainer origDb, string backupPath)
		{
			origDb.Backup(BackupStorage(), backupPath);
		}

		protected override IStorage BackupStorage()
		{
			return new FileStorage();
		}

		protected override IStorage OrigStorage()
		{
			return new MemoryStorage();
		}
	}
}
