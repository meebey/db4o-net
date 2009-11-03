/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Backup;

namespace Db4objects.Db4o.Tests.Common.Backup
{
	public abstract class MemoryBackupTestCaseBase : ITestCase
	{
		public class Item
		{
			public int _id;

			public Item(int id)
			{
				_id = id;
			}
		}

		private static readonly string DbPath = "database";

		private const int NumItems = 10;

		private static readonly string BackupPath = "backup";

		/// <exception cref="System.Exception"></exception>
		public virtual void TestMemoryBackup()
		{
			LocalObjectContainer origDb = (LocalObjectContainer)Db4oEmbedded.OpenFile(Config(
				OrigStorage()), DbPath);
			Store(origDb);
			Backup(origDb, BackupPath);
			origDb.Close();
			IObjectContainer backupDb = Db4oEmbedded.OpenFile(Config(BackupStorage()), BackupPath
				);
			IObjectSet result = backupDb.Query(typeof(MemoryBackupTestCaseBase.Item));
			Assert.AreEqual(NumItems, result.Count);
			backupDb.Close();
			BackupStorage().Delete(BackupPath);
		}

		protected abstract void Backup(LocalObjectContainer origDb, string backupPath);

		protected abstract IStorage BackupStorage();

		protected abstract IStorage OrigStorage();

		private void Store(LocalObjectContainer origDb)
		{
			for (int itemId = 0; itemId < NumItems; itemId++)
			{
				origDb.Store(new MemoryBackupTestCaseBase.Item(itemId));
			}
			origDb.Commit();
		}

		private IEmbeddedConfiguration Config(IStorage storage)
		{
			IEmbeddedConfiguration origConfig = Db4oEmbedded.NewConfiguration();
			origConfig.File.Storage = storage;
			return origConfig;
		}
	}
}
