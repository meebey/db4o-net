namespace Db4objects.Db4o.Tests.Common.Querying
{
	/// <exclude></exclude>
	public class OrderedQueryTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase().RunSolo();
		}

		public sealed class Item
		{
			public int value;

			public Item(int value)
			{
				this.value = value;
			}
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase.Item(1));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase.Item(3));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase.Item(2));
		}

		public virtual void TestOrderAscending()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase.Item)
				);
			query.Descend("value").OrderAscending();
			AssertQuery(new int[] { 1, 2, 3 }, query.Execute());
		}

		public virtual void TestOrderDescending()
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase.Item)
				);
			query.Descend("value").OrderDescending();
			AssertQuery(new int[] { 3, 2, 1 }, query.Execute());
		}

		private void AssertQuery(int[] expected, Db4objects.Db4o.IObjectSet actual)
		{
			for (int i = 0; i < expected.Length; i++)
			{
				Db4oUnit.Assert.IsTrue(actual.HasNext());
				Db4oUnit.Assert.AreEqual(expected[i], ((Db4objects.Db4o.Tests.Common.Querying.OrderedQueryTestCase.Item
					)actual.Next()).value);
			}
			Db4oUnit.Assert.IsFalse(actual.HasNext());
		}
	}
}
