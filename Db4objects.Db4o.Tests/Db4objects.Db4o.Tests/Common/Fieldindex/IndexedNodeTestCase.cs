namespace Db4objects.Db4o.Tests.Common.Fieldindex
{
	public class IndexedNodeTestCase : Db4objects.Db4o.Tests.Common.Fieldindex.FieldIndexProcessorTestCaseBase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Fieldindex.IndexedNodeTestCase().RunSolo();
		}

		protected override void Store()
		{
			StoreItems(new int[] { 3, 4, 7, 9 });
			StoreComplexItems(new int[] { 3, 4, 7, 9 }, new int[] { 2, 2, 8, 8 });
		}

		public virtual void TestTwoLevelDescendOr()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("child").Descend("foo").Constrain
				(4).Smaller();
			Db4objects.Db4o.Query.IConstraint c2 = query.Descend("child").Descend("foo").Constrain
				(4).Greater();
			c1.Or(c2);
			AssertSingleOrNode(query);
		}

		public virtual void TestMultipleOrs()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			Db4objects.Db4o.Query.IConstraint c1 = query.Descend("foo").Constrain(4).Smaller(
				);
			for (int i = 0; i < 5; i++)
			{
				Db4objects.Db4o.Query.IConstraint c2 = query.Descend("foo").Constrain(4).Greater(
					);
				c1 = c1.Or(c2);
			}
			AssertSingleOrNode(query);
		}

		public virtual void TestDoubleDescendingOnIndexedNodes()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			query.Descend("child").Descend("foo").Constrain(3);
			query.Descend("bar").Constrain(2);
			Db4objects.Db4o.Inside.Fieldindex.IIndexedNode index = SelectBestIndex(query);
			AssertComplexItemIndex("foo", index);
			Db4oUnit.Assert.IsFalse(index.IsResolved());
			Db4objects.Db4o.Inside.Fieldindex.IIndexedNode result = index.Resolve();
			Db4oUnit.Assert.IsNotNull(result);
			AssertComplexItemIndex("child", result);
			Db4oUnit.Assert.IsTrue(result.IsResolved());
			Db4oUnit.Assert.IsNull(result.Resolve());
			AssertComplexItems(new int[] { 4 }, result.ToTreeInt());
		}

		public virtual void TestTripleDescendingOnQuery()
		{
			Db4objects.Db4o.Query.IQuery query = CreateComplexItemQuery();
			query.Descend("child").Descend("child").Descend("foo").Constrain(3);
			Db4objects.Db4o.Inside.Fieldindex.IIndexedNode index = SelectBestIndex(query);
			AssertComplexItemIndex("foo", index);
			Db4oUnit.Assert.IsFalse(index.IsResolved());
			Db4objects.Db4o.Inside.Fieldindex.IIndexedNode result = index.Resolve();
			Db4oUnit.Assert.IsNotNull(result);
			AssertComplexItemIndex("child", result);
			Db4oUnit.Assert.IsFalse(result.IsResolved());
			result = result.Resolve();
			Db4oUnit.Assert.IsNotNull(result);
			AssertComplexItemIndex("child", result);
			AssertComplexItems(new int[] { 7 }, result.ToTreeInt());
		}

		private void AssertComplexItems(int[] expectedFoos, Db4objects.Db4o.TreeInt found
			)
		{
			Db4oUnit.Assert.IsNotNull(found);
			AssertTreeInt(MapToObjectIds(CreateComplexItemQuery(), expectedFoos), found);
		}

		private void AssertSingleOrNode(Db4objects.Db4o.Query.IQuery query)
		{
			System.Collections.IEnumerator nodes = CreateProcessor(query).CollectIndexedNodes
				();
			Db4oUnit.Assert.IsTrue(nodes.MoveNext());
			Db4objects.Db4o.Inside.Fieldindex.OrIndexedLeaf node = (Db4objects.Db4o.Inside.Fieldindex.OrIndexedLeaf
				)nodes.Current;
			Db4oUnit.Assert.IsNotNull(node);
			Db4oUnit.Assert.IsFalse(nodes.MoveNext());
		}
	}
}
