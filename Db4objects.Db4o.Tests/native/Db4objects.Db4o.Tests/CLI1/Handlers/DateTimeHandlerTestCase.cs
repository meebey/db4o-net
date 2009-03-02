/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class DateTimeHandlerTestCase : TypeHandlerTestCaseBase
    {
		protected override void Configure(IConfiguration config)
		{
			config.ExceptionsOnNotStorable(false);
		}

		public virtual void TestReadWrite()
        {
            MockWriteContext writeContext = new MockWriteContext(Db());
            DateTime expected = new DateTime();
            DateTimeHandler().Write(writeContext, expected);
            MockReadContext readContext = new MockReadContext(writeContext);
            DateTime actual = (DateTime)DateTimeHandler().Read(readContext);
            Assert.AreEqual(expected, actual);
        }

        public virtual void TestStoreObject()
        {
            DateTimeHandlerTestCase.Item storedItem = new DateTimeHandlerTestCase.Item(new DateTime());
            DoTestStoreObject(storedItem);
        }

        public void TestKind()
        {
            DateTimeKind kind = DateTimeKind.Utc;
            DateTime storedDateTime = DateTime.SpecifyKind(new DateTime(), kind);
            DateTimeHandlerTestCase.Item storedItem = new DateTimeHandlerTestCase.Item(storedDateTime);
            Store(storedItem);
            Reopen();
            Item retrievedItem = (Item) RetrieveOnlyInstance(typeof(Item));
            Assert.AreEqual(kind, retrievedItem._dateTime.Kind);
        }

        private Db4objects.Db4o.Internal.Handlers.DateTimeHandler DateTimeHandler()
        {
            return new Db4objects.Db4o.Internal.Handlers.DateTimeHandler();
        }

        public class Item
        {
            public DateTime _dateTime;

            public Item(DateTime wrapper)
            {
                _dateTime = wrapper;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                {
                    return true;
                }
                if (!(obj is DateTimeHandlerTestCase.Item))
                {
                    return false;
                }
                DateTimeHandlerTestCase.Item other = (DateTimeHandlerTestCase.Item)obj;
                return (other._dateTime == _dateTime);
            }

            public override string ToString()
            {
                return "[" + _dateTime + "]";
            }
        }

    }
}
