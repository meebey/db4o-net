namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadedDeleteUpdate : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class ParentItem
		{
			public object child;
		}

		public class ChildItem
		{
			public object parent1;

			public object parent2;
		}

		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate().RunClientServer(
				);
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem)
				).CascadeOnDelete(true);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem parentItem1
				 = new Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem();
			Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem parentItem2
				 = new Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem();
			Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ChildItem child = new 
				Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ChildItem();
			child.parent1 = parentItem1;
			child.parent2 = parentItem2;
			parentItem1.child = child;
			parentItem2.child = child;
			Db().Set(parentItem1);
		}

		public virtual void TestAllObjectStored()
		{
			AssertAllObjectStored();
		}

		public virtual void TestUpdate()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem)
				);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				Db().Set(objectSet.Next());
			}
			Db().Commit();
			AssertAllObjectStored();
		}

		private void AssertAllObjectStored()
		{
			Reopen();
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem)
				);
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem parentItem = 
					(Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.ParentItem)objectSet
					.Next();
				Db().Refresh(parentItem, 3);
				Db4oUnit.Assert.IsNotNull(parentItem.child);
			}
		}
	}
}
