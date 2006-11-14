namespace Db4objects.Db4o.Tests.Common.Querying
{
	public abstract class QueryResultTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
		, Db4oUnit.Extensions.Fixtures.IOptOutCS, Db4oUnit.Extensions.Fixtures.IOptOutDefragSolo
	{
		private static readonly int[] VALUES = new int[] { 1, 5, 6, 7, 9 };

		private readonly int[] itemIds = new int[VALUES.Length];

		private int idForGetAll;

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item)
				, "foo");
		}

		public virtual void TestClassQuery()
		{
			AssertIDs(ClassOnlyQuery(), itemIds);
		}

		public virtual void TestGetAll()
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = NewQueryResult();
			queryResult.LoadFromClassIndexes(Stream().ClassCollection().Iterator());
			int[] ids = Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.Concat(itemIds, new 
				int[] { idForGetAll });
			AssertIDs(queryResult, ids, true);
		}

		public virtual void TestIndexedFieldQuery()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("foo").Constrain(6).Smaller();
			Db4objects.Db4o.Inside.Query.IQueryResult queryResult = ExecuteQuery(query);
			AssertIDs(queryResult, new int[] { itemIds[0], itemIds[1] });
		}

		public virtual void TestNonIndexedFieldQuery()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("bar").Constrain(6).Smaller();
			Db4objects.Db4o.Inside.Query.IQueryResult queryResult = ExecuteQuery(query);
			AssertIDs(queryResult, new int[] { itemIds[0], itemIds[1] });
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult ClassOnlyQuery()
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = NewQueryResult();
			queryResult.LoadFromClassIndex(YapClass());
			return queryResult;
		}

		private Db4objects.Db4o.YapClass YapClass()
		{
			return Stream().GetYapClass(Reflector().ForClass(typeof(Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item)
				), false);
		}

		private Db4objects.Db4o.Inside.Query.IQueryResult ExecuteQuery(Db4objects.Db4o.Query.IQuery
			 query)
		{
			Db4objects.Db4o.Inside.Query.AbstractQueryResult queryResult = NewQueryResult();
			queryResult.LoadFromQuery((Db4objects.Db4o.QQuery)query);
			return queryResult;
		}

		private void AssertIDs(Db4objects.Db4o.Inside.Query.IQueryResult queryResult, int[]
			 expectedIDs)
		{
			AssertIDs(queryResult, expectedIDs, false);
		}

		private void AssertIDs(Db4objects.Db4o.Inside.Query.IQueryResult queryResult, int[]
			 expectedIDs, bool ignoreUnexpected)
		{
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor expectingVisitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.ToObjectArray(expectedIDs), 
				false, ignoreUnexpected);
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
			Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.ItemForGetAll ifga = new 
				Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.ItemForGetAll();
			Store(ifga);
			idForGetAll = (int)Db().GetID(ifga);
		}

		protected virtual void StoreItems(int[] foos)
		{
			for (int i = 0; i < foos.Length; i++)
			{
				Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item item = new Db4objects.Db4o.Tests.Common.Querying.QueryResultTestCase.Item
					(foos[i]);
				Store(item);
				itemIds[i] = (int)Db().GetID(item);
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

		public class ItemForGetAll
		{
		}

		protected abstract Db4objects.Db4o.Inside.Query.AbstractQueryResult NewQueryResult
			();
	}
}
