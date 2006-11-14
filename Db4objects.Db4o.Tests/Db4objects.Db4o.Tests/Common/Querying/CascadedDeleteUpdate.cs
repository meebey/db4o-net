namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class CascadedDeleteUpdate : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class CduHelper
		{
			internal object parent1;

			internal object parent2;
		}

		public object child;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ObjectClass(this).CascadeOnDelete(true);
			config.ObjectClass(typeof(Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.CduHelper)
				).CascadeOnDelete(true);
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate cdu1 = new Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate
				();
			Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate cdu2 = new Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate
				();
			Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.CduHelper helper = new 
				Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate.CduHelper();
			helper.parent1 = cdu1;
			helper.parent2 = cdu2;
			cdu1.child = helper;
			cdu2.child = helper;
			Db().Set(cdu1);
			Db().Set(cdu2);
			Db().Set(cdu1);
			Db().Set(cdu2);
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(this.GetType());
			Db4objects.Db4o.IObjectSet objectSet = q.Execute();
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate cdu = (Db4objects.Db4o.Tests.Common.Querying.CascadedDeleteUpdate
					)objectSet.Next();
				Db4oUnit.Assert.IsNotNull(cdu.child);
			}
		}
	}
}
