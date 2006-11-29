using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
    public class NonSerializedAttributeTestCase : AbstractDb4oTestCase
    {
        public class Item
        {
            [NonSerialized]
            public int NonSerializedValue;

            [Transient]
            public int TransientValue;
            
            public int Value;
            
            public Item()
            {   
            }
            
            public Item(int value_)
            {
                Value = value_;
                NonSerializedValue = value_;
                TransientValue = value_;
            }
        }
        
        public class DerivedItem : Item
        {
            public DerivedItem()
            {   
            }
            
            public DerivedItem(int value_) : base(value_)
            {
            }
        }

        protected override void Store()
        {
            Store(new Item(42));
            Store(new DerivedItem(42));
        }
        
        public void Test()
        {
            IObjectSet found = NewQuery(typeof(Item)).Execute();
            Assert.AreEqual(2, found.Count);
            foreach (Item item in found)
            {
                Assert.AreEqual(0, item.NonSerializedValue);
                Assert.AreEqual(0, item.TransientValue);
                Assert.AreEqual(42, item.Value);
            }
        }
    }
}
