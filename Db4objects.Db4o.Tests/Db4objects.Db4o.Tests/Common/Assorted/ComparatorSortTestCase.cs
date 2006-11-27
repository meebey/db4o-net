namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class ComparatorSortTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
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

		[System.Serializable]
		public class DescendingIdComparator : Db4objects.Db4o.Query.IQueryComparator
		{
			public virtual int Compare(object first, object second)
			{
				return ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)second
					)._id - ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)first
					)._id;
			}
		}

		[System.Serializable]
		public class OddEvenIdComparator : Db4objects.Db4o.Query.IQueryComparator
		{
			public virtual int Compare(object first, object second)
			{
				int idA = ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)first
					)._id;
				int idB = ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)second
					)._id;
				int modA = idA % 2;
				int modB = idB % 2;
				if (modA != modB)
				{
					return modA - modB;
				}
				return idA - idB;
			}
		}

		[System.Serializable]
		public class AscendingNameComparator : Db4objects.Db4o.Query.IQueryComparator
		{
			public virtual int Compare(object first, object second)
			{
				return ((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)first)
					._name.CompareTo(((Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item
					)second)._name);
			}
		}

		[System.Serializable]
		public class SmallerThanThreePredicate : Db4objects.Db4o.Query.Predicate
		{
			public virtual bool Match(Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item
				 candidate)
			{
				return candidate._id < 3;
			}
		}

		public class Item
		{
			public int _id;

			public string _name;

			public Item() : this(0, null)
			{
			}

			public Item(int id, string name)
			{
				this._id = id;
				this._name = name;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			config.ExceptionsOnNotStorable(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < 4; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item(i, (3
					 - i).ToString()));
			}
		}

		public virtual void TestByIdAscending()
		{
			AssertIdOrder(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.AscendingIdComparator
				(), new int[] { 0, 1, 2, 3 });
		}

		public virtual void TestByIdAscendingConstrained()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("_id").Constrain(3).Smaller();
			AssertIdOrder(query, new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.AscendingIdComparator
				(), new int[] { 0, 1, 2 });
		}

		public virtual void TestByIdAscendingNQ()
		{
			Db4objects.Db4o.IObjectSet result = Db().Query(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.SmallerThanThreePredicate
				(), new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.AscendingIdComparator
				());
			AssertIdOrder(result, new int[] { 0, 1, 2 });
		}

		public virtual void TestByIdDescending()
		{
			AssertIdOrder(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.DescendingIdComparator
				(), new int[] { 3, 2, 1, 0 });
		}

		public virtual void TestByIdDescendingConstrained()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("_id").Constrain(3).Smaller();
			AssertIdOrder(query, new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.DescendingIdComparator
				(), new int[] { 2, 1, 0 });
		}

		public virtual void TestByIdDescendingNQ()
		{
			Db4objects.Db4o.IObjectSet result = Db().Query(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.SmallerThanThreePredicate
				(), new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.DescendingIdComparator
				());
			AssertIdOrder(result, new int[] { 2, 1, 0 });
		}

		public virtual void TestByIdOddEven()
		{
			AssertIdOrder(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.OddEvenIdComparator
				(), new int[] { 0, 2, 1, 3 });
		}

		public virtual void TestByIdOddEvenConstrained()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("_id").Constrain(3).Smaller();
			AssertIdOrder(query, new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.OddEvenIdComparator
				(), new int[] { 0, 2, 1 });
		}

		public virtual void TestByIdOddEvenNQ()
		{
			Db4objects.Db4o.IObjectSet result = Db().Query(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.SmallerThanThreePredicate
				(), new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.OddEvenIdComparator
				());
			AssertIdOrder(result, new int[] { 0, 2, 1 });
		}

		public virtual void TestByNameAscending()
		{
			AssertIdOrder(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.AscendingNameComparator
				(), new int[] { 3, 2, 1, 0 });
		}

		public virtual void TestByNameAscendingConstrained()
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			query.Descend("_id").Constrain(3).Smaller();
			AssertIdOrder(query, new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.AscendingNameComparator
				(), new int[] { 2, 1, 0 });
		}

		public virtual void TestByNameAscendingNQ()
		{
			Db4objects.Db4o.IObjectSet result = Db().Query(new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.SmallerThanThreePredicate
				(), new Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.AscendingNameComparator
				());
			AssertIdOrder(result, new int[] { 2, 1, 0 });
		}

		private void AssertIdOrder(Db4objects.Db4o.Query.IQueryComparator comparator, int[]
			 ids)
		{
			Db4objects.Db4o.Query.IQuery query = NewItemQuery();
			AssertIdOrder(query, comparator, ids);
		}

		private Db4objects.Db4o.Query.IQuery NewItemQuery()
		{
			return NewQuery(typeof(Db4objects.Db4o.Tests.Common.Assorted.ComparatorSortTestCase.Item)
				);
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
