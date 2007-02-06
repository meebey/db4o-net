namespace Db4objects.Db4o.Tests.Common.Classindex
{
	public class ClassIndexTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase, Db4oUnit.Extensions.Fixtures.IOptOutCS
	{
		public class Item
		{
			public string name;

			public Item(string _name)
			{
				this.name = _name;
			}
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Classindex.ClassIndexTestCase().RunSolo();
		}

		public virtual void TestDelete()
		{
			Db4objects.Db4o.Tests.Common.Classindex.ClassIndexTestCase.Item item = new Db4objects.Db4o.Tests.Common.Classindex.ClassIndexTestCase.Item
				("test");
			Store(item);
			int id = (int)Db().GetID(item);
			AssertID(id);
			Reopen();
			item = (Db4objects.Db4o.Tests.Common.Classindex.ClassIndexTestCase.Item)Db().Get(
				item).Next();
			id = (int)Db().GetID(item);
			AssertID(id);
			Db().Delete(item);
			Db().Commit();
			AssertEmpty();
			Reopen();
			AssertEmpty();
		}

		private void AssertID(int id)
		{
			AssertIndex(new object[] { id });
		}

		private void AssertEmpty()
		{
			AssertIndex(new object[] {  });
		}

		private void AssertIndex(object[] expected)
		{
			Db4objects.Db4o.Internal.ClassMetadata clazz = Stream().GetYapClass(Reflector().ForClass
				(typeof(Db4objects.Db4o.Tests.Common.Classindex.ClassIndexTestCase.Item)));
			Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor visitor = new Db4objects.Db4o.Tests.Common.Btree.ExpectingVisitor
				(expected);
			Db4objects.Db4o.Internal.Classindex.IClassIndexStrategy index = clazz.Index();
			index.TraverseAll(Trans(), visitor);
			visitor.AssertExpectations();
		}
	}
}
