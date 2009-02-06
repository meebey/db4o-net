using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
    public class EnumTestCase : AbstractDb4oTestCase
    {

        enum MyEnum { A, B, C, D, F, INCOMPLETE }; 

        class Item { 

            public MyEnum _enum; 

         } 

       protected override void Store()
       {
           Item item = new Item();
           item._enum = MyEnum.C;
           Store(item);
       }

       public void TestPeekPersisted()
       {
           Item item = (Item) RetrieveOnlyInstance(typeof (Item));
           Item peeked = (Item) Db().PeekPersisted(item, int.MaxValue, true);
           Assert.AreSame(item._enum, peeked._enum);
       }
    } 

}
