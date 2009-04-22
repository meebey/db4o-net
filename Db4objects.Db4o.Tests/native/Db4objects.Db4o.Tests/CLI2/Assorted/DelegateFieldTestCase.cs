using System;
using System.ComponentModel;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
	class DelegateFieldTestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			public PropertyChangedEventHandler changed;
		}

		public class Holder
		{
			public Item item;

			public Holder(Item item)
			{
				this.item = item;
			}
		}

		protected override void Store()
		{
			Item item = new Item();
			item.changed += delegate { };
			Store(new Holder(item));
		}

		public void Test()
		{
			Assert.IsNull(RetrieveOnlyInstance<Holder>().item.changed);
		}
	}
}
