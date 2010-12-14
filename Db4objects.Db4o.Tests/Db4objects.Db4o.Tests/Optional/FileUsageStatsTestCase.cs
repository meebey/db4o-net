/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System.Collections;
using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Filestats;
using Db4objects.Db4o.Foundation.IO;
using Db4objects.Db4o.Tests.Common.Api;
using Db4objects.Db4o.Tests.Optional;

namespace Db4objects.Db4o.Tests.Optional
{
	public class FileUsageStatsTestCase : TestWithTempFile
	{
		public class Child
		{
		}

		public class Item
		{
			public int _id;

			public string _name;

			public int[] _arr;

			public IList _list;

			public Item(int id, string name, IList list)
			{
				_id = id;
				_name = name;
				_arr = new int[] { id };
				_list = list;
			}
		}

		/// <exception cref="System.Exception"></exception>
		public virtual void TestFileStats()
		{
			CreateDatabase(new ArrayList());
			AssertFileStats();
			Defrag();
			AssertFileStats();
		}

		private void AssertFileStats()
		{
			FileUsageStats stats = FileUsageStatsCollector.RunStats(TempFile(), true);
			Assert.AreEqual(stats.FileSize(), stats.TotalUsage());
		}

		/// <exception cref="System.IO.IOException"></exception>
		private void Defrag()
		{
			string backupPath = Path.GetTempFileName();
			DefragmentConfig config = new DefragmentConfig(TempFile(), backupPath);
			config.ForceBackupDelete(true);
			Db4objects.Db4o.Defragment.Defragment.Defrag(config);
			File4.Delete(backupPath);
		}

		private void CreateDatabase(IList gaps)
		{
			File4.Delete(TempFile());
			IEmbeddedConfiguration config = Db4oEmbedded.NewConfiguration();
			config.Common.ObjectClass(typeof(FileUsageStatsTestCase.Item)).ObjectField("_id")
				.Indexed(true);
			config.Common.ObjectClass(typeof(FileUsageStatsTestCase.Item)).ObjectField("_name"
				).Indexed(true);
			IEmbeddedObjectContainer db = Db4oEmbedded.OpenFile(config, TempFile());
			IList list = new ArrayList();
			list.Add(new FileUsageStatsTestCase.Child());
			FileUsageStatsTestCase.Item item = new FileUsageStatsTestCase.Item(0, "#0", list);
			db.Store(item);
			db.Commit();
			db.Close();
		}
	}
}
