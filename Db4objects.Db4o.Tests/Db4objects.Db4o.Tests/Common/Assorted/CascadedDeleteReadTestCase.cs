namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class CascadedDeleteReadTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Item
		{
			public Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item _child1;

			public Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item _child2;

			public string _name;

			public Item()
			{
			}

			public Item(string name)
			{
				_name = name;
			}

			public Item(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item
				 child1, Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item child2
				, string name)
			{
				_child1 = child1;
				_child2 = child2;
				_name = name;
			}
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase().RunSoloAndClientServer
				();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item)
				).ObjectField("_child1").CascadeOnDelete(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item)
				).ObjectField("_child2").CascadeOnDelete(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item)
				).ObjectField("_child1").CascadeOnUpdate(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item)
				).ObjectField("_child2").CascadeOnUpdate(true);
		}

		protected override void Store()
		{
			Store(new Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item(new 
				Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item("1"), null
				, "parent"));
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item item = ParentItem
				();
			item._child2 = item._child1;
			item._child1 = null;
			Store(item);
			Db().Delete(item);
			AssertItemCount(0);
		}

		private Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item ParentItem
			()
		{
			Db4objects.Db4o.Query.IQuery q = Db().Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item)
				);
			q.Descend("_name").Constrain("parent");
			return (Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item)q.Execute
				().Next();
		}

		private void AssertItemCount(int count)
		{
			Db4objects.Db4o.Query.IQuery q = Db().Query();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Assorted.CascadedDeleteReadTestCase.Item)
				);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			Db4oUnit.Assert.AreEqual(count, objectSet.Size());
		}
	}
}
