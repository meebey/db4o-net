namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class CanUpdateFalseRefreshTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Item
		{
			public int _id;

			public string _name;

			public Item(int id, string name)
			{
				_id = id;
				_name = name;
			}

			public virtual bool ObjectCanUpdate(Db4objects.Db4o.IObjectContainer container)
			{
				return false;
			}
		}

		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Assorted.CanUpdateFalseRefreshTestCase.Item
				(1, "one"));
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Common.Assorted.CanUpdateFalseRefreshTestCase.Item item = (
				Db4objects.Db4o.Tests.Common.Assorted.CanUpdateFalseRefreshTestCase.Item)RetrieveOnlyInstance
				(typeof(Db4objects.Db4o.Tests.Common.Assorted.CanUpdateFalseRefreshTestCase.Item)
				);
			item._name = "two";
			Db().Set(item);
			Db4oUnit.Assert.AreEqual("two", item._name);
			Db().Refresh(item, 2);
			Db4oUnit.Assert.AreEqual("one", item._name);
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.CanUpdateFalseRefreshTestCase().RunSoloAndClientServer
				();
		}
	}
}
