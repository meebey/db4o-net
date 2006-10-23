namespace Db4objects.Db4o.Tests.Common.Querying
{
	/// <exclude></exclude>
	public class ObjectSetTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Querying.AllTests().RunSoloAndClientServer();
		}

		public class Item
		{
			public string name;

			public Item()
			{
			}

			public Item(string name)
			{
				this.name = name;
			}
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item("foo"));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item("bar"));
			Db().Set(new Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item("baz"));
		}

		public virtual void TestObjectsCantBeSeenAfterDelete()
		{
			Db4objects.Db4o.Transaction trans1 = NewTransaction();
			Db4objects.Db4o.Transaction trans2 = NewTransaction();
			Db4objects.Db4o.IObjectSet os = QueryItems(trans1);
			DeleteItemAndCommit(trans2, "foo");
			AssertItems(new string[] { "bar", "baz" }, os);
		}

		private void AssertItems(string[] expectedNames, Db4objects.Db4o.IObjectSet actual
			)
		{
			for (int i = 0; i < expectedNames.Length; i++)
			{
				Db4oUnit.Assert.IsTrue(actual.HasNext());
				Db4oUnit.Assert.AreEqual(expectedNames[i], ((Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item
					)actual.Next()).name);
			}
			Db4oUnit.Assert.IsFalse(actual.HasNext());
		}

		private void DeleteItemAndCommit(Db4objects.Db4o.Transaction trans, string name)
		{
			Stream().Delete(trans, QueryItem(trans, name));
			trans.Commit();
		}

		private Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item QueryItem(Db4objects.Db4o.Transaction
			 trans, string name)
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(trans, typeof(Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item)
				);
			q.Descend("name").Constrain(name);
			return (Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item)q.Execute().
				Next();
		}

		private Db4objects.Db4o.IObjectSet QueryItems(Db4objects.Db4o.Transaction trans)
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(trans, typeof(Db4objects.Db4o.Tests.Common.Querying.ObjectSetTestCase.Item)
				);
			q.Descend("name").OrderAscending();
			return q.Execute();
		}
	}
}
