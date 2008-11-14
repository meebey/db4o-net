/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class DecimalHandlerTestCase : TypeHandlerTestCaseBase
    {
		protected override void Configure(IConfiguration config)
		{
			config.ExceptionsOnNotStorable(false);
		}

		public virtual void TestReadWrite()
        {
            MockWriteContext writeContext = new MockWriteContext(Db());
            decimal expected = Decimal.MaxValue;
            DecimalHandler().Write(writeContext, expected);
            MockReadContext readContext = new MockReadContext(writeContext);
            decimal decimalValue = (decimal)DecimalHandler().Read(readContext);
            Assert.AreEqual(expected, decimalValue);
        }

        public virtual void TestStoreObject()
        {
            DecimalHandlerTestCase.Item storedItem = new DecimalHandlerTestCase.Item(Decimal.MaxValue, Decimal.MinValue);
            DoTestStoreObject(storedItem);
        }

        private Db4objects.Db4o.Internal.Handlers.DecimalHandler DecimalHandler()
        {
            return new Db4objects.Db4o.Internal.Handlers.DecimalHandler();
        }

        public class Item
        {
            public decimal _decimal;

            public Decimal _decimalWrapper;

            public Item(decimal d, Decimal wrapper)
            {
                _decimal = d;
                _decimalWrapper = wrapper;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                {
                    return true;
                }
                if (!(obj is DecimalHandlerTestCase.Item))
                {
                    return false;
                }
                DecimalHandlerTestCase.Item other = (DecimalHandlerTestCase.Item)obj;
                return (other._decimal == this._decimal) && this._decimalWrapper.Equals(other._decimalWrapper
                    );
            }

            public override string ToString()
            {
                return "[" + _decimal + "," + _decimalWrapper + "]";
            }
        }

    }
}
