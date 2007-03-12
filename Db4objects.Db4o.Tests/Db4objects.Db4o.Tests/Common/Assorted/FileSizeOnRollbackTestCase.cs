namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class FileSizeOnRollbackTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
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
			Db4objects.Db4o.Internal.LocalObjectContainer localContainer = Fixture().FileSession
				();
			Db4objects.Db4o.Internal.IoAdaptedObjectContainer container = (Db4objects.Db4o.Internal.IoAdaptedObjectContainer
				)localContainer;
			container.SyncFiles();
			long length = new Sharpen.IO.File(container.FileName()).Length();
			return (int)length;
		}
	}
}
