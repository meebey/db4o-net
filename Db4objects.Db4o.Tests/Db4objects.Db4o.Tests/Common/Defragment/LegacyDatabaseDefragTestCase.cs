/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System.IO;
using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Defragment;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Defragment;
using Db4objects.Db4o.Tests.Common.Migration;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	/// <summary>test case for COR-785</summary>
	public class LegacyDatabaseDefragTestCase : ITestCase
	{
		private const int ITEM_COUNT = 50;

		public sealed class Item
		{
			public int value;

			public Item()
			{
			}

			public Item(int value)
			{
				this.value = value;
			}
		}

		public virtual void Test()
		{
			string dbFile = GetTempFile();
			CreateLegacyDatabase(dbFile);
			Defrag(dbFile);
			AssertContents(dbFile);
		}

		private void AssertContents(string dbFile)
		{
			IObjectContainer container = Db4oFactory.OpenFile(dbFile);
			try
			{
				IObjectSet found = QueryItems(container);
				for (int i = 1; i < ITEM_COUNT; i += 2)
				{
					Assert.IsTrue(found.HasNext());
					Assert.AreEqual(i, ((LegacyDatabaseDefragTestCase.Item)found.Next()).value);
				}
			}
			finally
			{
				container.Close();
			}
		}

		private IObjectSet QueryItems(IObjectContainer container)
		{
			IQuery q = container.Query();
			q.Constrain(typeof(LegacyDatabaseDefragTestCase.Item));
			q.Descend("value").OrderAscending();
			IObjectSet found = q.Execute();
			return found;
		}

		public virtual void CreateDatabase(string fname)
		{
			IObjectContainer container = Db4oFactory.OpenFile(fname);
			try
			{
				FragmentDatabase(container);
			}
			finally
			{
				container.Close();
			}
		}

		private void FragmentDatabase(IObjectContainer container)
		{
			LegacyDatabaseDefragTestCase.Item[] items = CreateItems();
			for (int i = 0; i < items.Length; ++i)
			{
				container.Set(items[i]);
			}
			for (int i = 0; i < items.Length; i += 2)
			{
				container.Delete(items[i]);
			}
		}

		private LegacyDatabaseDefragTestCase.Item[] CreateItems()
		{
			LegacyDatabaseDefragTestCase.Item[] items = new LegacyDatabaseDefragTestCase.Item
				[LegacyDatabaseDefragTestCase.ITEM_COUNT];
			for (int i = 0; i < items.Length; ++i)
			{
				items[i] = new LegacyDatabaseDefragTestCase.Item(i);
			}
			return items;
		}

		private string GetTempFile()
		{
			return Path.GetTempFileName();
		}

		private void Defrag(string dbFile)
		{
			DefragmentConfig config = new DefragmentConfig(dbFile);
			config.UpgradeFile(dbFile + ".upgraded");
			Db4objects.Db4o.Defragment.Defragment.Defrag(config);
		}

		private void CreateLegacyDatabase(string dbFile)
		{
			Db4oLibrary library = new Db4oLibrarian(new Db4oLibraryEnvironmentProvider(PathProvider
				.TestCasePath())).ForVersion("6.1");
			library.environment.InvokeInstanceMethod(GetType(), "createDatabase", new object[
				] { dbFile });
		}
	}
}
