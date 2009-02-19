using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI2.Assorted
{
    public class NullableDateTimeTestCase : AbstractDb4oTestCase
    {
        class Item
        {
            public DateTime? _typedDateTime;

            public object _untypedDateTime;

            public DateTime?[] _typedArray;

            public object _untypedArray;


            public Item(DateTime? value)
            {
                _typedDateTime = value;
                _untypedDateTime = value;
                _typedArray = new DateTime?[] {value};
                _untypedArray = new DateTime?[] {value};

            }
        }

        protected override void Store()
        {
            Item item = new Item(null);
            Store(item);
        }

        public void TestRetrievedIsNull()
        {
            Item item = RetrieveOnlyItem();
            AssertItemDateTime(item, null);
        }

        private Item RetrieveOnlyItem()
        {
            return (Item) RetrieveOnlyInstance(typeof (Item));
        }

        public void TestUpdate()
        {
            DateTime updatedDateTime = new DateTime(2009, 2, 18);
            Item item = RetrieveOnlyItem();
            UpdateDateTime(item, updatedDateTime);
            StoreCommitRefresh(item);
            AssertItemDateTime(item, updatedDateTime);
            UpdateDateTime(item, null);
            StoreCommitRefresh(item);
            AssertItemDateTime(item, null);
        }

        private void UpdateDateTime(Item item, DateTime? updatedDateTime)
        {
            item._typedDateTime = updatedDateTime;
            item._untypedDateTime = updatedDateTime;
            item._typedArray[0] = updatedDateTime;
            ((DateTime?[]) item._untypedArray)[0] = updatedDateTime;
        }

        private void StoreCommitRefresh(Item item)
        {
            Store(item);
            Db().Commit();
            Db().Refresh(item, int.MaxValue);
        }

        private void AssertItemDateTime(Item item, DateTime? expectedDateTime)
        {
            Assert.AreEqual(expectedDateTime, item._typedDateTime);
            Assert.AreEqual(expectedDateTime, item._untypedDateTime);
            Assert.AreEqual(expectedDateTime, item._typedArray[0]);
            Assert.AreEqual(expectedDateTime, ((DateTime?[])item._untypedArray)[0]);
        }

    }
}
