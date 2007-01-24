namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class FieldIndexProcessorTestCase : Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexProcessorTestCaseBase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexProcessorTestCase().RunSolo
				();
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			base.Configure(config);
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Fieldindex.NonIndexedFieldIndexItem)
				, "indexed");
		}

		protected override void Store()
		{
			StoreItems(new int[] { 3, 4, 7, 9 });
			StoreComplexItems(new int[] { 3, 4, 7, 9 }, new int[] { 2, 2, 8, 8 });
		}

		public virtual void TestIdentity()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			query.Descend("foo").Constrain(3);
			Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem item = (Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem
				)query.Execute().Next();
			query = CreateComplexItemQuery();
			query.Descend("child").Constrain(item).Identity();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, new int[] { 4 }, query);
		}

		public virtual void TestSingleIndexNotSmaller()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			query.Descend("foo").Constrain(5).Smaller().Not();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 7, 9 }, query);
		}

		public virtual void TestSingleIndexNotGreater()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			query.Descend("foo").Constrain(4).Greater().Not();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 3, 4 }, query);
		}

		public virtual void TestSingleIndexSmallerOrEqual()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			query.Descend("foo").Constrain(7).Smaller().Equal();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 3, 4, 7 }, query);
		}

		public virtual void TestSingleIndexGreaterOrEqual()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			query.Descend("foo").Constrain(7).Greater().Equal();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 7, 9 }, query);
		}

		public virtual void TestSingleIndexRange()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			query.Descend("foo").Constrain(3).Greater();
			query.Descend("foo").Constrain(9).Smaller();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 4, 7 }, query);
		}

		public virtual void TestSingleIndexAndRange()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(3).Greater(
				);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(9).Smaller(
				);
			c1.And(c2);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 4, 7 }, query);
		}

		public virtual void TestSingleIndexOr()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(4).Smaller(
				);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(7).Greater(
				);
			c1.Or(c2);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 3, 9 }, query);
		}

		public virtual void TestExplicitAndOverOr()
		{
			AssertAndOverOrQuery(true);
		}

		public virtual void TestImplicitAndOverOr()
		{
			AssertAndOverOrQuery(false);
		}

		private void AssertAndOverOrQuery(bool explicitAnd)
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(3);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(9);
			Db4objects.Db4o.Query.IConstraint c3 = query.Descend("foo").Constrain(3);
			Db4objects.Db4o.Query.IConstraint c4 = query.Descend("foo").Constrain(7);
			Db4objects.Db4o.Query.IConstraint cc1 = c1.Or(c2);
			Db4objects.Db4o.Query.IConstraint cc2 = c3.Or(c4);
			if (explicitAnd)
			{
				cc1.And(cc2);
			}
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 3 }, query);
		}

		public virtual void TestSingleIndexOrRange()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(1).Greater(
				);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(4).Smaller(
				);
			Db4objects.Db4o.Query.IConstraint c3 = query.Descend("foo").Constrain(4).Greater(
				);
			Db4objects.Db4o.Query.IConstraint c4 = query.Descend("foo").Constrain(10).Smaller
				();
			Db4objects.Db4o.Query.IConstraint cc1 = c1.And(c2);
			Db4objects.Db4o.Query.IConstraint cc2 = c3.And(c4);
			cc1.Or(cc2);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 3, 7, 9 }, query);
		}

		public virtual void TestImplicitAndOnOrs()
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(4).Smaller(
				);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(3).Greater(
				);
			Db4objects.Db4o.Query.IConstraint c3 = query.Descend("foo").Constrain(4).Greater(
				);
			c1.Or(c2);
			c1.Or(c3);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { 3, 4, 7, 9 }, query);
		}

		public virtual void TestTwoLevelDescendOr()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("child").Descend("foo").Constrain
				(4).Smaller();
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("child").Descend("foo").Constrain
				(4).Greater();
			c1.Or(c2);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, new int[] { 4, 9 }, query);
		}

		public virtual void _testOrOnDifferentFields()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(3);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("bar").Constrain(8);
			c1.Or(c2);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, new int[] { 3, 7, 9 }, query);
		}

		public virtual void TestCantOptimizeOrInvolvingNonIndexedField()
		{
			Db4objects.Db4o.Query.IQuery query = CreateQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.NonIndexedFieldIndexItem)
				);
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("indexed").Constrain(1);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(2);
			c1.Or(c2);
			AssertCantOptimize(query);
		}

		public virtual void TestCantOptimizeDifferentLevels()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("child").Descend("foo").Constrain
				(4).Smaller();
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(7).Greater(
				);
			c1.Or(c2);
			AssertCantOptimize(query);
		}

		public virtual void TestCantOptimizeJoinOnNonIndexedFields()
		{
			Db4objects.Db4o.Query.IQuery query = CreateQuery(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.NonIndexedFieldIndexItem)
				);
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(1);
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(2);
			c1.Or(c2);
			AssertCantOptimize(query);
		}

		private void AssertCantOptimize(Db4objects.Db4o.Query.IQuery query)
		{
			Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult result = ExecuteProcessor
				(query);
			Db4oUnit.Assert.AreSame(Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
				.NO_INDEX_FOUND, result);
		}

		public virtual void TestIndexSelection()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			query.Descend("bar").Constrain(2);
			query.Descend("foo").Constrain(3);
			AssertBestIndex("foo", query);
			query = CreateComplexItemQuery();
			query.Descend("foo").Constrain(3);
			query.Descend("bar").Constrain(2);
			AssertBestIndex("foo", query);
		}

		private void AssertBestIndex(string expectedFieldIndex, Db4objects.Db4o.Query.IQuery
			 query)
		{
			Db4objects.Db4o.Inside.Fieldindex.IIndexedNode node = SelectBestIndex(query);
			AssertComplexItemIndex(expectedFieldIndex, node);
		}

		public virtual void TestDoubleDescendingOnQuery()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			query.Descend("child").Descend("foo").Constrain(3);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, new int[] { 4 }, query);
		}

		public virtual void TestTripleDescendingOnQuery()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			query.Descend("child").Descend("child").Descend("foo").Constrain(3);
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.ComplexFieldIndexItem)
				, new int[] { 7 }, query);
		}

		public virtual void TestMultiTransactionSmallerWithCommit()
		{
			Db4objects.Db4o.Transaction transaction = NewTransaction();
			FillTransactionWith(transaction, 0);
			int[] expectedZeros = NewBTreeNodeSizedArray(0);
			AssertSmaller(transaction, expectedZeros, 3);
			transaction.Commit();
			FillTransactionWith(transaction, 5);
			AssertSmaller(Db4objects.Db4o.Tests.Common.Foundation.IntArrays4.Concat(expectedZeros
				, new int[] { 3, 4 }), 7);
		}

		public virtual void TestMultiTransactionWithRollback()
		{
			Db4objects.Db4o.Transaction transaction = NewTransaction();
			FillTransactionWith(transaction, 0);
			int[] expectedZeros = NewBTreeNodeSizedArray(0);
			AssertSmaller(transaction, expectedZeros, 3);
			transaction.Rollback();
			AssertSmaller(transaction, new int[0], 3);
			FillTransactionWith(transaction, 5);
			AssertSmaller(new int[] { 3, 4 }, 7);
		}

		public virtual void TestMultiTransactionSmaller()
		{
			Db4objects.Db4o.Transaction transaction = NewTransaction();
			FillTransactionWith(transaction, 0);
			int[] expected = NewBTreeNodeSizedArray(0);
			AssertSmaller(transaction, expected, 3);
			FillTransactionWith(transaction, 5);
			AssertSmaller(new int[] { 3, 4 }, 7);
		}

		public virtual void TestMultiTransactionGreater()
		{
			FillTransactionWith(SystemTrans(), 10);
			FillTransactionWith(SystemTrans(), 5);
			AssertGreater(new int[] { 4, 7, 9 }, 3);
			RemoveFromTransaction(SystemTrans(), 5);
			AssertGreater(new int[] { 4, 7, 9 }, 3);
			RemoveFromTransaction(SystemTrans(), 10);
			AssertGreater(new int[] { 4, 7, 9 }, 3);
		}

		public virtual void TestSingleIndexEquals()
		{
			int expectedBar = 3;
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, new int[] { expectedBar }, CreateQuery(expectedBar));
		}

		public virtual void TestSingleIndexSmaller()
		{
			AssertSmaller(new int[] { 3, 4 }, 7);
		}

		public virtual void TestSingleIndexGreater()
		{
			AssertGreater(new int[] { 4, 7, 9 }, 3);
		}

		private void AssertGreater(int[] expectedFoos, int greaterThan)
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery();
			query.Descend("foo").Constrain(greaterThan).Greater();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, expectedFoos, query);
		}

		private void AssertExpectedFoos(System.Type itemClass, int[] expectedFoos, Db4objects.Db4o.Query.IQuery
			 query)
		{
			Db4objects.Db4o.Transaction trans = TransactionFromQuery(query);
			int[] expectedIds = MapToObjectIds(CreateQuery(trans, itemClass), expectedFoos);
			AssertExpectedIDs(expectedIds, query);
		}

		private void AssertExpectedIDs(int[] expectedIds, Db4objects.Db4o.Query.IQuery query
			)
		{
			Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult result = ExecuteProcessor
				(query);
			if (expectedIds.Length == 0)
			{
				Db4oUnit.Assert.AreSame(Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult
					.FOUND_INDEX_BUT_NO_MATCH, result);
				return;
			}
			AssertTreeInt(expectedIds, result.ToTreeInt());
		}

		private Db4objects.Db4o.Inside.Fieldindex.FieldIndexProcessorResult ExecuteProcessor
			(Db4objects.Db4o.Query.IQuery query)
		{
			return CreateProcessor(query).Run();
		}

		private Db4objects.Db4o.Transaction TransactionFromQuery(Db4objects.Db4o.Query.IQuery
			 query)
		{
			return ((Db4objects.Db4o.QQuery)query).GetTransaction();
		}

		private Db4objects.Db4o.Inside.Btree.BTree Btree()
		{
			return FieldIndexBTree(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, "foo");
		}

		private void Store(Db4objects.Db4o.Transaction trans, Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem
			 item)
		{
			Stream().Set(trans, item);
		}

		private void FillTransactionWith(Db4objects.Db4o.Transaction trans, int bar)
		{
			for (int i = 0; i < Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.FillSize(Btree
				()); ++i)
			{
				Store(trans, new Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem(bar));
			}
		}

		private int[] NewBTreeNodeSizedArray(int value)
		{
			Db4objects.Db4o.Inside.Btree.BTree btree = Btree();
			return Db4objects.Db4o.Tests.Common.Btree.BTreeAssert.NewBTreeNodeSizedArray(btree
				, value);
		}

		private void RemoveFromTransaction(Db4objects.Db4o.Transaction trans, int foo)
		{
			Db4objects.Db4o.IObjectSet found = CreateItemQuery(trans).Execute();
			while (found.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem item = (Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem
					)found.Next();
				if (item.foo == foo)
				{
					Stream().Delete(trans, item);
				}
			}
		}

		private void AssertSmaller(int[] expectedFoos, int smallerThan)
		{
			AssertSmaller(Trans(), expectedFoos, smallerThan);
		}

		private void AssertSmaller(Db4objects.Db4o.Transaction transaction, int[] expectedFoos
			, int smallerThan)
		{
			Db4objects.Db4o.Query.IQuery query = CreateItemQuery(transaction);
			query.Descend("foo").Constrain(smallerThan).Smaller();
			AssertExpectedFoos(typeof(Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexItem)
				, expectedFoos, query);
		}
	}
}
