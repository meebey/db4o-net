namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class RollbackTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Item
		{
			public string _string;

			public Item()
			{
			}

			public Item(string str)
			{
				_string = str;
			}
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.RollbackTestCase().RunClientServer();
		}

		public virtual void TestNotIsStoredOnRollback()
		{
			Db4objects.Db4o.Tests.Common.Assorted.RollbackTestCase.Item item = new Db4objects.Db4o.Tests.Common.Assorted.RollbackTestCase.Item
				();
			Store(item);
			Db().Rollback();
			Db4oUnit.Assert.IsFalse(Db().IsStored(item));
		}
	}
}
