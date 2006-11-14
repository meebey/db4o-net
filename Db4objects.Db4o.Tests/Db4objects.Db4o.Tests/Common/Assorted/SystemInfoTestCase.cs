namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class SystemInfoTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Item
		{
		}

		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.SystemInfoTestCase().RunSolo();
		}

		protected override void Db4oCustomTearDown()
		{
			Db4objects.Db4o.Db4oFactory.Configure().Freespace().UseRamSystem();
		}

		public virtual void TestDefaultFreespaceInfo()
		{
			AssertFreespaceInfo(Db().SystemInfo());
		}

		public virtual void TestIndexBasedFreespaceInfo()
		{
			Db4objects.Db4o.Db4oFactory.Configure().Freespace().UseIndexSystem();
			Reopen();
			AssertFreespaceInfo(Db().SystemInfo());
		}

		private void AssertFreespaceInfo(Db4objects.Db4o.Ext.ISystemInfo info)
		{
			Db4oUnit.Assert.IsNotNull(info);
			Db4objects.Db4o.Tests.Common.Assorted.SystemInfoTestCase.Item item = new Db4objects.Db4o.Tests.Common.Assorted.SystemInfoTestCase.Item
				();
			Db().Set(item);
			Db().Commit();
			Db().Delete(item);
			Db().Commit();
			Db4oUnit.Assert.IsTrue(info.FreespaceEntryCount() > 0);
			Db4oUnit.Assert.IsTrue(info.FreespaceSize() > 0);
		}

		public virtual void TestTotalSize()
		{
			if (Fixture() is Db4oUnit.Extensions.Fixtures.AbstractFileBasedDb4oFixture)
			{
				Db4oUnit.Extensions.Fixtures.AbstractFileBasedDb4oFixture fixture = (Db4oUnit.Extensions.Fixtures.AbstractFileBasedDb4oFixture
					)Fixture();
				Sharpen.IO.File f = new Sharpen.IO.File(fixture.GetAbsolutePath());
				long expectedSize = f.Length();
				long actual = Db().SystemInfo().TotalSize();
				Db4oUnit.Assert.AreEqual(expectedSize, actual);
			}
		}
	}
}
