using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class FileSizeOnRollbackTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new FileSizeOnRollbackTestCase().RunSolo();
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
				Store(new FileSizeOnRollbackTestCase.Item());
				Db().Rollback();
				Assert.AreEqual(originalFileSize, FileSize());
			}
		}

		private void StoreSomeItems()
		{
			for (int i = 0; i < 3; i++)
			{
				Store(new FileSizeOnRollbackTestCase.Item());
			}
			Db().Commit();
		}

		private void ProduceSomeFreeSpace()
		{
			IObjectSet objectSet = NewQuery(typeof(FileSizeOnRollbackTestCase.Item)).Execute(
				);
			while (objectSet.HasNext())
			{
				Db().Delete(objectSet.Next());
			}
			Db().Commit();
		}

		private int FileSize()
		{
			LocalObjectContainer localContainer = Fixture().FileSession();
			IoAdaptedObjectContainer container = (IoAdaptedObjectContainer)localContainer;
			container.SyncFiles();
			long length = new Sharpen.IO.File(container.FileName()).Length();
			return (int)length;
		}
	}
}
