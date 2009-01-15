using System;
using Db4oUnit;
using Db4objects.Db4o.Config;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
    public class CsSystemArrayTestCase:AbstractDb4oTestCase
    {
        public class Item
        {
            public System.Array _intArray;
        }

        static int[] INT_DATA = new int[] { 0, 1, 2, 3 };

        protected override void Store()
        {
            Item item = new Item();
            item._intArray = INT_DATA;
            Store(item);
        }

        public void Test()
        {
            Item item = (Item) RetrieveOnlyInstance(typeof (Item));
            int[] boxedIntArray = (int[])item._intArray;
            ArrayAssert.AreEqual(INT_DATA, boxedIntArray);
        }

    }
}
