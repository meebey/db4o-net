/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class SystemInfoTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
		}

		public static void Main(string[] arguments)
		{
			new SystemInfoTestCase().RunSolo();
		}

		/// <exception cref="Exception"></exception>
		protected override void Db4oTearDownBeforeClean()
		{
			Db4oFactory.Configure().Freespace().UseRamSystem();
		}

		public virtual void TestDefaultFreespaceInfo()
		{
			AssertFreespaceInfo(FileSession().SystemInfo());
		}

		private void AssertFreespaceInfo(ISystemInfo info)
		{
			Assert.IsNotNull(info);
			SystemInfoTestCase.Item item = new SystemInfoTestCase.Item();
			Db().Store(item);
			Db().Commit();
			Db().Delete(item);
			Db().Commit();
			Assert.IsTrue(info.FreespaceEntryCount() > 0);
			Assert.IsTrue(info.FreespaceSize() > 0);
		}

		public virtual void TestTotalSize()
		{
			if (Fixture() is AbstractFileBasedDb4oFixture)
			{
				// assuming YapFile only
				AbstractFileBasedDb4oFixture fixture = (AbstractFileBasedDb4oFixture)Fixture();
				Sharpen.IO.File f = new Sharpen.IO.File(fixture.GetAbsolutePath());
				long expectedSize = f.Length();
				long actual = Db().SystemInfo().TotalSize();
				Assert.AreEqual(expectedSize, actual);
			}
		}
	}
}
