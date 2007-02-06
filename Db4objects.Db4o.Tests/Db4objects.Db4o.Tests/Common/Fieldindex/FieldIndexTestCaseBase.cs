namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public abstract class FieldIndexTestCaseBase : Db4oUnit.Extensions.AbstractDb4oTestCase
		, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public FieldIndexTestCaseBase() : base()
		{
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, "foo");
		}

		protected abstract override void Store();

		protected virtual void StoreItems(int[] foos)
		{
			for (int i = 0; i < foos.Length; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem(foos[i]));
			}
		}

		protected virtual Db4objects.Db4o.Query.IQuery CreateQuery(int id)
		{
			Db4objects.Db4o.Query.IQuery q = CreateItemQuery();
			q.Descend("foo").Constrain(id);
			return q;
		}

		protected virtual Db4objects.Db4o.Query.IQuery CreateItemQuery()
		{
			return CreateQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				);
		}

		protected virtual Db4objects.Db4o.Query.IQuery CreateQuery(System.Type clazz)
		{
			return CreateQuery(Trans(), clazz);
		}

		protected virtual Db4objects.Db4o.Query.IQuery CreateQuery(Db4objects.Db4o.Internal.Transaction
			 trans, System.Type clazz)
		{
			Db4objects.Db4o.Query.IQuery q = CreateQuery(trans);
			q.Constrain(clazz);
			return q;
		}

		protected virtual Db4objects.Db4o.Query.IQuery CreateItemQuery(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			Db4objects.Db4o.Query.IQuery q = CreateQuery(trans);
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem));
			return q;
		}

		private Db4objects.Db4o.Query.IQuery CreateQuery(Db4objects.Db4o.Internal.Transaction
			 trans)
		{
			return Stream().Query(trans);
		}
	}
}
