namespace Db4objects.Db4o.Tests.CLI2.Regression
{
#if NET_2_0
    using System.Collections.Generic;
    using Db4oUnit;
    using Db4oUnit.Extensions;

    class COR242TestCase : AbstractDb4oTestCase
    {
        public class Item
        {
            public IList<string> items;

            public Item(IList<string> items_)
            {
                items = items_;
            }
        }

        protected override void Store()
        {
            Store(new Item(new string[] {"foo", "bar"}));
        }

        public void Test()
        {
            Item item = (Item) RetrieveOnlyInstance(typeof(Item));
            Assert.IsNotNull(item.items);
            Assert.IsInstanceOf(typeof(string[]), item.items);
            ArrayAssert.AreEqual(new string[] {"foo", "bar"}, (string[])item.items);
        }
    }
#endif
}
