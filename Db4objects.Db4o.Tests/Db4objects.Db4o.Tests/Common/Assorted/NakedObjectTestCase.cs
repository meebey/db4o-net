namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class NakedObjectTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Item
		{
			public object field = new object();
		}

		public virtual void TestStoreNakedObjects()
		{
			try
			{
				Db().Set(new Db4objects.Db4o.Tests.Common.Assorted.NakedObjectTestCase.Item());
				Db4oUnit.Assert.Fail("Naked objects can't be stored");
			}
			catch (Db4objects.Db4o.Ext.ObjectNotStorableException)
			{
			}
		}
	}
}
