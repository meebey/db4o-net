using System;
using Db4oUnit;

namespace Db4objects.Drs.Tests.Regression
{
	class DelegateTestCase : DrsTestCase
	{
		public class Item
		{
			public event EventHandler Foo;

			public string Value;

			public Item(string value)
			{
				Value = value;
			}

			public int HandlerCount
			{
				get
				{
					if (Foo == null) return 0;
					return Foo.GetInvocationList().Length;
				}
			}
		}

		public void Test()
		{
			Item item = new Item("the item");
			item.Foo += delegate { };

			A().Provider().StoreNew(item);
			A().Provider().Commit();

			ReplicateAll(A().Provider(), B().Provider());

			Item replicated = (Item)B().Provider().GetStoredObjects(typeof(Item))[0];
			Assert.IsNotNull(replicated);
			Assert.AreEqual(item.Value, replicated.Value);
			Assert.AreEqual(0, replicated.HandlerCount);
		
		}
	}
}
