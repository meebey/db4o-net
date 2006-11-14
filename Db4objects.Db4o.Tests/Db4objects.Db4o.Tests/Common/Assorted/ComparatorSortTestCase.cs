namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ComparatorSortTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase().RunSoloAndClientServer
				();
		}

		public class Item
		{
			public int _id;

			public string _name;

			public Item(int id, string name)
			{
				_id = id;
				_name = name;
			}
		}

		[System.Serializable]
		public class AscendingIdComparator : Db4objects.Db4o.Query.IQueryComparator
		{
			public virtual int Compare(object first, object second)
			{
				return ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)first)
					._id - ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)second
					)._id;
			}
		}

		protected override void Store()
		{
			for (int i = 0; i < 4; i++)
			{
				Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item item = new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item
					(i, (3 - i).ToString());
				Store(item);
			}
		}

		public virtual void TestByIdAscending()
		{
			AssertIdOrder(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.AscendingIdComparator
				(), new int[] { 0, 1, 2, 3 });
		}

		private void AssertIdOrder(Db4objects.Db4o.Query.IQueryComparator comparator, int[]
			 ids)
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)
				);
			AssertIdOrder(query, comparator, ids);
		}

		private void AssertIdOrder(Db4objects.Db4o.Query.IQuery query, Db4objects.Db4o.Query.IQueryComparator
			 comparator, int[] ids)
		{
			query.SortBy(comparator);
			Db4objects.Db4o.IObjectSet result = query.Execute();
			AssertIdOrder(result, ids);
		}

		private void AssertIdOrder(Db4objects.Db4o.IObjectSet result, int[] ids)
		{
			Db4oUnit.Assert.AreEqual(ids.Length, result.Size());
			for (int idx = 0; idx < ids.Length; idx++)
			{
				Db4oUnit.Assert.AreEqual(ids[idx], ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item
					)result.Next())._id);
			}
		}
	}
}
