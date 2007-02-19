
namespace Db4objects.Db4o.Tests.CLI2.Regression
{
#if (NET_2_0 || CF_2_0)
	using System.Collections.Generic;

	using Db4objects.Db4o.Config;

	using Db4oUnit;
	using Db4oUnit.Extensions;

	class COR195TestCase : AbstractDb4oTestCase
	{
		public class Item
		{
			private int i;

			public Item(int i)
			{
				this.i = i;
			}

			public int I
			{
				get { return i; }
				set { i = value; }
			}
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(Item)).ObjectField("i").Indexed(true);
		}

		protected override void Store()
		{
			for (int i = 0; i < 1000; i++) Store(new Item(i));
		}

		public void TestNativeQueryOnIndex()
		{
			IList<Item> list = Db().Query<Item>(delegate(Item i) { return i.I > 100 && i.I <= 200; });
			Assert.AreEqual(100, list.Count);
			for (int i = 0; i < list.Count; i++)
			{
				Assert.AreEqual(i + 101, list[i].I);
			}
		}
	}
#endif
}
