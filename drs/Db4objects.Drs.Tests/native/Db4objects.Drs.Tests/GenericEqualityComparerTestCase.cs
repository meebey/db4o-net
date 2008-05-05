using System;
using System.Collections.Generic;
using System.Text;
using Db4objects.Db4o;
using Db4oUnit;

namespace Db4objects.Drs.Tests
{
	class GenericEqualityComparerTestCase : DrsTestCase
	{
		public class Item
		{
			public IEqualityComparer<string> comparer;

			public Item(IEqualityComparer<string> comparer)
			{
				this.comparer = comparer;
			}
		}

		public void Test()
		{
			A().Provider().StoreNew(new Item(EqualityComparer<string>.Default));
			A().Provider().Commit();

			ReplicateAll(A().Provider(), B().Provider());

			IObjectSet found = B().Provider().GetStoredObjects(typeof(Item));
			Assert.AreEqual(1, found.Count);

			Item item = (Item) found[0];
			Assert.IsNotNull(item.comparer);
		}
	}
}
