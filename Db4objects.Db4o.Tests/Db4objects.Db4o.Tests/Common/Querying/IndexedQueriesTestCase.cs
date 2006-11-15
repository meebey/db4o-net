namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class IndexedQueriesTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] arguments)
		{
			new Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase().RunSolo();
		}

		public class IndexedQueriesItem
		{
			public string _name;

			public int _int;

			public int _integer;

			public IndexedQueriesItem()
			{
			}

			public IndexedQueriesItem(string name)
			{
				_name = name;
			}

			public IndexedQueriesItem(int int_)
			{
				_int = int_;
				_integer = int_;
			}
		}

		protected override void Configure(Db4objects.Db4o.Config.IConfiguration config)
		{
			IndexField(config, "_name");
			IndexField(config, "_int");
			IndexField(config, "_integer");
		}

		private void IndexField(Db4objects.Db4o.Config.IConfiguration config, string fieldName
			)
		{
			IndexField(config, typeof(Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem)
				, fieldName);
		}

		protected override void Store()
		{
			string[] strings = new string[] { "a", "c", "b", "f", "e" };
			for (int i = 0; i < strings.Length; i++)
			{
				Db().Set(new Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
					(strings[i]));
			}
			int[] ints = new int[] { 1, 5, 7, 3, 2, 3 };
			for (int i = 0; i < ints.Length; i++)
			{
				Db().Set(new Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
					(ints[i]));
			}
		}

		public virtual void TestIntQuery()
		{
			AssertInts(5);
		}

		public virtual void TestStringQuery()
		{
			AssertNullNameCount(6);
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
				("d"));
			AssertQuery(1, "b");
			UpdateB();
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
				("z"));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
				("y"));
			Reopen();
			AssertQuery(1, "b");
			AssertInts(8);
		}

		private void AssertIntegers()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Descend("_integer").Constrain(4).Greater().Equal();
			AssertIntsFound(new int[] { 5, 7 }, q);
			q = NewQuery();
			q.Descend("_integer").Constrain(4).Smaller();
			AssertIntsFound(new int[] { 1, 2, 3, 3 }, q);
		}

		private void AssertInts(int expectedZeroSize)
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Descend("_int").Constrain(0);
			int zeroSize = q.Execute().Size();
			Db4oUnit.Assert.AreEqual(expectedZeroSize, zeroSize);
			q = NewQuery();
			q.Descend("_int").Constrain(4).Greater().Equal();
			AssertIntsFound(new int[] { 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(4).Greater();
			AssertIntsFound(new int[] { 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(3).Greater();
			AssertIntsFound(new int[] { 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(3).Greater().Equal();
			AssertIntsFound(new int[] { 3, 3, 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(2).Greater().Equal();
			AssertIntsFound(new int[] { 2, 3, 3, 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(2).Greater();
			AssertIntsFound(new int[] { 3, 3, 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(1).Greater().Equal();
			AssertIntsFound(new int[] { 1, 2, 3, 3, 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(1).Greater();
			AssertIntsFound(new int[] { 2, 3, 3, 5, 7 }, q);
			q = NewQuery();
			q.Descend("_int").Constrain(4).Smaller();
			AssertIntsFound(new int[] { 1, 2, 3, 3 }, expectedZeroSize, q);
			q = NewQuery();
			q.Descend("_int").Constrain(4).Smaller().Equal();
			AssertIntsFound(new int[] { 1, 2, 3, 3 }, expectedZeroSize, q);
			q = NewQuery();
			q.Descend("_int").Constrain(3).Smaller();
			AssertIntsFound(new int[] { 1, 2 }, expectedZeroSize, q);
			q = NewQuery();
			q.Descend("_int").Constrain(3).Smaller().Equal();
			AssertIntsFound(new int[] { 1, 2, 3, 3 }, expectedZeroSize, q);
			q = NewQuery();
			q.Descend("_int").Constrain(2).Smaller().Equal();
			AssertIntsFound(new int[] { 1, 2 }, expectedZeroSize, q);
			q = NewQuery();
			q.Descend("_int").Constrain(2).Smaller();
			AssertIntsFound(new int[] { 1 }, expectedZeroSize, q);
			q = NewQuery();
			q.Descend("_int").Constrain(1).Smaller().Equal();
			AssertIntsFound(new int[] { 1 }, expectedZeroSize, q);
			q = NewQuery();
			q.Descend("_int").Constrain(1).Smaller();
			AssertIntsFound(new int[] {  }, expectedZeroSize, q);
		}

		private void AssertIntsFound(int[] ints, int zeroSize, Db4objects.Db4o.Query.IQuery
			 q)
		{
			Db4objects.Db4o.IObjectSet res = q.Execute();
			Db4oUnit.Assert.AreEqual((ints.Length + zeroSize), res.Size());
			while (res.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem ci
					 = (Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
					)res.Next();
				for (int i = 0; i < ints.Length; i++)
				{
					if (ints[i] == ci._int)
					{
						ints[i] = 0;
						break;
					}
				}
			}
			for (int i = 0; i < ints.Length; i++)
			{
				Db4oUnit.Assert.AreEqual(0, ints[i]);
			}
		}

		private void AssertIntsFound(int[] ints, Db4objects.Db4o.Query.IQuery q)
		{
			AssertIntsFound(ints, 0, q);
		}

		private void AssertQuery(int count, string @string)
		{
			Db4objects.Db4o.IObjectSet res = QueryForName(@string);
			Db4oUnit.Assert.AreEqual(count, res.Size());
			Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem item
				 = (Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
				)res.Next();
			Db4oUnit.Assert.AreEqual("b", item._name);
		}

		private void AssertNullNameCount(int count)
		{
			Db4objects.Db4o.IObjectSet res = QueryForName(null);
			Db4oUnit.Assert.AreEqual(count, res.Size());
			while (res.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem ci
					 = (Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
					)res.Next();
				Db4oUnit.Assert.IsNull(ci._name);
			}
		}

		private void UpdateB()
		{
			Db4objects.Db4o.IObjectSet res = QueryForName("b");
			Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem ci
				 = (Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem
				)res.Next();
			ci._name = "j";
			Db().Set(ci);
			res = QueryForName("b");
			Db4oUnit.Assert.AreEqual(0, res.Size());
			res = QueryForName("j");
			Db4oUnit.Assert.AreEqual(1, res.Size());
			ci._name = "b";
			Db().Set(ci);
			AssertQuery(1, "b");
		}

		private Db4objects.Db4o.IObjectSet QueryForName(string n)
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Descend("_name").Constrain(n);
			return q.Execute();
		}

		protected override Db4objects.Db4o.Query.IQuery NewQuery()
		{
			Db4objects.Db4o.Query.IQuery q = base.NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Querying.IndexedQueriesTestCase.IndexedQueriesItem)
				);
			return q;
		}
	}
}
