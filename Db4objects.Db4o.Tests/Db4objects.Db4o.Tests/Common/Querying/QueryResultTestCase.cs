namespace Db4objects.Db4o.Tests.Common.Querying
{
	public abstract class QueryResultTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
		, Db4oUnit.Extensions.Fixtures.IOptOutDefragSolo
	{
		private static readonly int[] VALUES = new int[] { 1, 5, 6, 7, 9 };

		private readonly int[] ids = new int[VALUES.Length];

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item)
				, "foo");
		}

		public virtual void TestClassQuery()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			Db4objects.Db4o.Inside.Query.IQueryResult queryResult = ExecuteQuery(query);
			AssertIDs(queryResult, ids);
		}

		public virtual void TestIndexedFieldQuery()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("foo").Constrain(6).Smaller();
			Db4objects.Db4o.Inside.Query.IQueryResult queryResult = ExecuteQuery(query);
			AssertIDs(queryResult, new int[] { ids[0], ids[1] });
		}

		public virtual void TestNonIndexedFieldQuery()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("bar").Constrain(6).Smaller();
			Db4objects.Db4o.Inside.Query.IQueryResult queryResult = ExecuteQuery(query);
			AssertIDs(queryResult, new int[] { ids[0], ids[1] });
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult ExecuteQuery(Db4objects.Db4o.Query.IQuery
			 query)
		{
			Db4objects.Db4o.Inside.Query.IQueryResult queryResult = NewQueryResult();
			queryResult.LoadFromQuery((Db4objects.Db4o.QQuery)query);
			return queryResult;
		}

		private void AssertIDs(Db4objects.Db4o.Inside.Query.IQueryResult queryResult, int[]
			 expectedIDs)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.ToObjectArray(expectedIDs));
			Db4objects.Db4o.Foundation.IIntIterator4 i = queryResult.IterateIDs();
			while (i.MoveNext())
			{
				expectingVisitor.Visit(i.CurrentInt());
			}
			expectingVisitor.AssertExpectations();
		}

		protected virtual Db4objects.Db4o.Query.IQuery NewItemQuery()
		{
			return NewQuery(typeof(Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item)
				);
		}

		protected override void Store()
		{
			StoreItems(VALUES);
		}

		protected virtual void StoreItems(int[] foos)
		{
			for (int i = 0; i < foos.Length; i++)
			{
				Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item item = new Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item
					(foos[i]);
				Store(item);
				ids[i] = (int)Db().GetID(item);
			}
		}

		public class Item
		{
			public int foo;

			public int bar;

			public Item()
			{
			}

			public Item(int foo_)
			{
				foo = foo_;
				bar = foo;
			}
		}

		protected abstract Db4objects.Db4o.Inside.Query.IQueryResult NewQueryResult();
	}
}
