/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class UShortHandlerTestCase : TypeHandlerTestCaseBase
    {
		protected override void Configure(IConfiguration config)
		{
			config.ExceptionsOnNotStorable(false);
		}

        public virtual void TestReadWrite()
        {
            MockWriteContext writeContext = new MockWriteContext(Db());
            ushort expected = 0x1122;
            UShortHandler().Write(writeContext, expected);
            MockReadContext readContext = new MockReadContext(writeContext);
            ushort ushortValue = (ushort)UShortHandler().Read(readContext);
            Assert.AreEqual(expected, ushortValue);
        }

        public virtual void TestStoreObject()
        {
            ULongHandlerTestCase.Item storedItem = new ULongHandlerTestCase.Item(0x1122, 0x8877);
            DoTestStoreObject(storedItem);
        }

        private Db4objects.Db4o.Internal.Handlers.UShortHandler UShortHandler()
        {
            return new Db4objects.Db4o.Internal.Handlers.UShortHandler();
        }

        public class Item
        {
            public ushort _ushort;

            public UInt16 _ushortWrapper;

            public Item(ushort u, UInt16 wrapper)
            {
                _ushort = u;
                _ushortWrapper = wrapper;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                {
                    return true;
                }
                if (!(obj is UShortHandlerTestCase.Item))
                {
                    return false;
                }
                UShortHandlerTestCase.Item other = (UShortHandlerTestCase.Item)obj;
                return (other._ushort == this._ushort) && this._ushortWrapper.Equals(other._ushortWrapper
                    );
            }

            public override string ToString()
            {
                return "[" + _ushort + "," + _ushortWrapper + "]";
            }
        }

    }
}