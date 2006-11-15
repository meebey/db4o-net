namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class DescendToNullFieldTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static int COUNT = 2;

		public class ParentItem
		{
			public string _name;

			public Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ChildItem
				 one;

			public Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ChildItem
				 two;

			public ParentItem(string name, Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ChildItem
				 child1, Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ChildItem
				 child2)
			{
				_name = name;
				one = child1;
				two = child2;
			}
		}

		public class ChildItem
		{
			public string _name;

			public ChildItem(string name)
			{
				_name = name;
			}
		}

		protected override void Store()
		{
			for (int i = 0; i < COUNT; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ParentItem
					("one", new Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ChildItem
					("one"), null));
			}
			for (int i = 0; i < COUNT; i++)
			{
				Store(new Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ParentItem
					("two", null, new Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ChildItem
					("two")));
			}
		}

		public virtual void Test()
		{
			AssertResults("one");
			AssertResults("two");
		}

		private void AssertResults(string name)
		{
			Db4objects.Db4o.Query.IQuery query = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ParentItem)
				);
			query.Descend(name).Descend("_name").Constrain(name);
			Db4objects.Db4o.IObjectSet objectSet = query.Execute();
			Db4oUnit.Assert.AreEqual(COUNT, objectSet.Size());
			while (objectSet.HasNext())
			{
				Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ParentItem parentItem
					 = (Db4objects.Db4o.Tests.Common.Assorted.DescendToNullFieldTestCase.ParentItem)
					objectSet.Next();
				Db4oUnit.Assert.AreEqual(name, parentItem._name);
			}
		}
	}
}
