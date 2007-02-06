namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ReAddCascadedDeleteTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase().RunClientServer
				();
		}

		public class Item
		{
			public string _name;

			public Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item _member;

			public Item()
			{
			}

			public Item(string name)
			{
				_name = name;
			}

			public Item(string name, Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item
				 member)
			{
				_name = name;
				_member = member;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item)
				).CascadeOnDelete(true);
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item
				("parent", new Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item
				("child")));
		}

		public virtual void TestDeletingAndReaddingMember()
		{
			DeleteParentAndReAddChild();
			Reopen();
			Db4oUnit.Assert.IsNotNull(Query("child"));
			Db4oUnit.Assert.IsNull(Query("parent"));
		}

		private void DeleteParentAndReAddChild()
		{
			Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item i = Query(
				"parent");
			Db().Delete(i);
			Db().Set(i._member);
			Db().Commit();
		}

		private Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item Query
			(string name)
		{
			Db4objects.Db4o.IObjectSet objectSet = Db().Get(new Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item
				(name));
			if (!objectSet.HasNext())
			{
				return null;
			}
			return (Db4objects.Db4o.Tests.Common.Assorted.ReAddCascadedDeleteTestCase.Item)objectSet
				.Next();
		}
	}
}
