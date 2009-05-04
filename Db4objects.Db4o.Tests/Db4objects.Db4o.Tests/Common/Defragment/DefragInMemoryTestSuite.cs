/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public class DefragInMemoryTestSuite : FixtureBasedTestSuite
	{
		private class StorageSpec : ILabeled
		{
			private readonly string _label;

			private readonly IStorage _storage;

			public readonly string _path;

			public StorageSpec(string label, IStorage storage, string path)
			{
				_label = label;
				_storage = storage;
				_path = path;
			}

			public virtual IStorage Storage(IStorage storage)
			{
				return _storage == null ? storage : _storage;
			}

			public virtual string Label()
			{
				return _label;
			}
		}

		private static readonly FixtureVariable StorageSpecFixture = new FixtureVariable(
			);

		public override IFixtureProvider[] FixtureProviders()
		{
			string tempFilePath = Path.GetTempFileName();
			File4.Delete(tempFilePath);
			return new IFixtureProvider[] { new SimpleFixtureProvider(StorageSpecFixture, new 
				DefragInMemoryTestSuite.StorageSpec[] { new DefragInMemoryTestSuite.StorageSpec(
				"memory", null, "backup"), new DefragInMemoryTestSuite.StorageSpec("file", new FileStorage
				(), tempFilePath) }) };
		}

		public override Type[] TestUnits()
		{
			return new Type[] { typeof(DefragInMemoryTestSuite.DefragInMemoryTestUnit) };
		}

		public class DefragInMemoryTestUnit : ITestLifeCycle
		{
			public class Item
			{
				public int _id;

				public Item(int id)
				{
					_id = id;
				}
			}

			[System.Serializable]
			public class EvenIdItemsPredicate : Predicate
			{
				public virtual bool Match(DefragInMemoryTestSuite.DefragInMemoryTestUnit.Item item
					)
				{
					return (item._id % 2) == 0;
				}
			}

			private const int NumItems = 100;

			protected static readonly string Uri = "database";

			private MemoryStorage _memoryStorage;

			/// <exception cref="System.Exception"></exception>
			public virtual void TestInMemoryDefragment()
			{
				Store();
				Defrag();
				Assert.IsSmaller(BackupLength(), _memoryStorage.Bin(Uri).Length());
				Retrieve();
			}

			private long BackupLength()
			{
				IBin backupBin = BackupStorage().Open(new BinConfiguration(BackupPath(), true, 0, 
					true));
				long backupLength = backupBin.Length();
				backupBin.Close();
				return backupLength;
			}

			private DefragmentConfig DefragmentConfig(MemoryStorage storage)
			{
				DefragmentConfig defragConfig = new DefragmentConfig(Uri, BackupPath(), new TreeIDMapping
					());
				defragConfig.Db4oConfig(Config(storage));
				defragConfig.BackupStorage(BackupStorage());
				return defragConfig;
			}

			private string BackupPath()
			{
				return ((DefragInMemoryTestSuite.StorageSpec)StorageSpecFixture.Value)._path;
			}

			private IStorage BackupStorage()
			{
				return ((DefragInMemoryTestSuite.StorageSpec)StorageSpecFixture.Value).Storage(_memoryStorage
					);
			}

			private IEmbeddedConfiguration Config(IStorage storage)
			{
				IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
				config.Common.ReflectWith(Platform4.ReflectorForType(typeof(DefragInMemoryTestSuite.DefragInMemoryTestUnit.Item
					)));
				config.File.Storage = storage;
				return config;
			}

			/// <exception cref="System.IO.IOException"></exception>
			private void Defrag()
			{
				DefragmentConfig defragConfig = DefragmentConfig(_memoryStorage);
				Db4objects.Db4o.Defragment.Defragment.Defrag(defragConfig);
			}

			private void Store()
			{
				IObjectContainer db = Db4oEmbedded.OpenFile(Config(_memoryStorage), Uri);
				for (int itemId = 0; itemId < NumItems; itemId++)
				{
					db.Store(new DefragInMemoryTestSuite.DefragInMemoryTestUnit.Item(itemId));
				}
				db.Commit();
				IObjectSet result = db.Query(new DefragInMemoryTestSuite.DefragInMemoryTestUnit.EvenIdItemsPredicate
					());
				while (result.HasNext())
				{
					db.Delete(((DefragInMemoryTestSuite.DefragInMemoryTestUnit.Item)result.Next()));
				}
				db.Close();
			}

			private void Retrieve()
			{
				IObjectContainer db = Db4oEmbedded.OpenFile(Config(_memoryStorage), Uri);
				IObjectSet result = db.Query(typeof(DefragInMemoryTestSuite.DefragInMemoryTestUnit.Item
					));
				Assert.AreEqual(NumItems / 2, result.Count);
				while (result.HasNext())
				{
					Assert.IsTrue((((DefragInMemoryTestSuite.DefragInMemoryTestUnit.Item)result.Next(
						))._id % 2) == 1);
				}
				db.Close();
			}

			/// <exception cref="System.Exception"></exception>
			public virtual void SetUp()
			{
				_memoryStorage = new MemoryStorage();
			}

			/// <exception cref="System.Exception"></exception>
			public virtual void TearDown()
			{
				BackupStorage().Delete(BackupPath());
			}
		}
	}
}
