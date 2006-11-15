namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class MultiDeleteTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutDefragSolo
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase().RunSoloAndClientServer
				();
		}

		public class Item
		{
			public Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item child;

			public string name;

			public object forLong;

			public long myLong;

			public object[] untypedArr;

			public long[] typedArr;

			public virtual void SetMembers()
			{
				forLong = System.Convert.ToInt64(100);
				myLong = System.Convert.ToInt64(100);
				untypedArr = new object[] { System.Convert.ToInt64(10), "hi", new Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item
					() };
				typedArr = new long[] { System.Convert.ToInt64(3), System.Convert.ToInt64(7), System.Convert.ToInt64
					(9) };
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			Db4objects.Db4o.Config.IObjectClass itemClass = config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item)
				);
			itemClass.CascadeOnDelete(true);
			itemClass.CascadeOnUpdate(true);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item md = new Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item
				();
			md.name = "killmefirst";
			md.SetMembers();
			md.child = new Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item();
			md.child.SetMembers();
			Db().Set(md);
		}

		public virtual void TestDeleteCanBeCalledTwice()
		{
			Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item item = ItemByName(
				"killmefirst");
			Db4oUnit.Assert.IsNotNull(item);
			long id = Db().GetID(item);
			Db().Delete(item);
			Db4oUnit.Assert.AreSame(item, ItemById(id));
			Db().Delete(item);
			Db4oUnit.Assert.AreSame(item, ItemById(id));
		}

		private Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item ItemByName
			(string name)
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item)
				);
			q.Descend("name").Constrain(name);
			return (Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item)q.Execute(
				).Next();
		}

		private Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item ItemById(long
			 id)
		{
			return (Db4objects.Db4o.Tests.Common.Assorted.MultiDeleteTestCase.Item)Db().GetByID
				(id);
		}
	}
}
