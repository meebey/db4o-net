namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class FileSizeOnRollbackTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
		, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.FileSizeOnRollbackTestCase().RunSolo();
		}

		public class Item
		{
			public int _int;
		}

		public virtual void TestFileSizeDoesNotIncrease()
		{
			StoreSomeItems();
			ProduceSomeFreeSpace();
			int originalFileSize = FileSize();
			for (int i = 0; i < 100; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Assorted.FileSizeOnRollbackTestCase.Item()
					);
				Db().Rollback();
				Db4oUnit.Assert.AreEqual(originalFileSize, FileSize());
			}
		}

		private void StoreSomeItems()
		{
			for (int i = 0; i < 3; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Assorted.FileSizeOnRollbackTestCase.Item()
					);
			}
			Db().Commit();
		}

		private void ProduceSomeFreeSpace()
		{
			Db4objects.Db4o.IObjectSet objectSet = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Assorted.FileSizeOnRollbackTestCase.Item)
				).Execute();
			while (objectSet.HasNext())
			{
				Db().Delete(objectSet.Next());
			}
			Db().Commit();
		}

		private int FileSize()
		{
			Db4oUnit.Extensions.Fixtures.AbstractFileBasedDb4oFixture fixture = (Db4oUnit.Extensions.Fixtures.AbstractFileBasedDb4oFixture
				)Fixture();
			Db4objects.Db4o.Internal.IoAdaptedObjectContainer container = (Db4objects.Db4o.Internal.IoAdaptedObjectContainer
				)Fixture().Db();
			container.SyncFiles();
			long length = new Sharpen.IO.File(fixture.GetAbsolutePath()).Length();
			return (int)length;
		}
	}
}
