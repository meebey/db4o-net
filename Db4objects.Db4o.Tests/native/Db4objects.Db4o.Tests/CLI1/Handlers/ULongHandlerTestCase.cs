/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class ULongHandlerTestCase : TypeHandlerTestCaseBase
    {
        public virtual void TestReadWrite()
        {
            MockWriteContext writeContext = new MockWriteContext(Db());
            ulong expected = 0x1122334455667788;
            ULongHandler().Write(writeContext, expected);
            MockReadContext readContext = new MockReadContext(writeContext);
            ulong ulongValue = (ulong)ULongHandler().Read(readContext);
            Assert.AreEqual(expected, ulongValue);
        }

        public virtual void TestStoreObject()
        {
            ULongHandlerTestCase.Item storedItem = new ULongHandlerTestCase.Item(0x1122334455667788, 0x8877665544332211);
            DoTestStoreObject(storedItem);
        }

        private Db4objects.Db4o.Internal.Handlers.ULongHandler ULongHandler()
        {
            return new Db4objects.Db4o.Internal.Handlers.ULongHandler();
        }

        public class Item
        {
            public ulong _ulong;

            public UInt64 _ulongWrapper;

            public Item(ulong u, UInt64 wrapper)
            {
                _ulong = u;
                _ulongWrapper = wrapper;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                {
                    return true;
                }
                if (!(obj is ULongHandlerTestCase.Item))
                {
                    return false;
                }
                ULongHandlerTestCase.Item other = (ULongHandlerTestCase.Item)obj;
                return (other._ulong == this._ulong) && this._ulongWrapper.Equals(other._ulongWrapper
                    );
            }

            public override string ToString()
            {
                return "[" + _ulong + "," + _ulongWrapper + "]";
            }
        }

    }
}