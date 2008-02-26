/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class UIntHandlerTestCase : TypeHandlerTestCaseBase
    {
        public virtual void TestReadWrite()
        {
            MockWriteContext writeContext = new MockWriteContext(Db());
            uint expected = 0x11223344;
            UIntHandler().Write(writeContext, expected);
            MockReadContext readContext = new MockReadContext(writeContext);
            uint uintValue = (uint)UIntHandler().Read(readContext);
            Assert.AreEqual(expected, uintValue);
        }

        public virtual void TestStoreObject()
        {
            UIntHandlerTestCase.Item storedItem = new UIntHandlerTestCase.Item((uint)0x11223344, (uint)0x55667788);
            DoTestStoreObject(storedItem);
        }

        private Db4objects.Db4o.Internal.Handlers.UIntHandler UIntHandler()
        {
            return new Db4objects.Db4o.Internal.Handlers.UIntHandler();
        }

        public class Item
        {
            public uint _uint;

            public UInt32 _uintWrapper;

            public Item(uint u, UInt32 wrapper)
            {
                _uint = u;
                _uintWrapper = wrapper;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                {
                    return true;
                }
                if (!(obj is UIntHandlerTestCase.Item))
                {
                    return false;
                }
                UIntHandlerTestCase.Item other = (UIntHandlerTestCase.Item)obj;
                return (other._uint == this._uint) && this._uintWrapper.Equals(other._uintWrapper
                    );
            }

            public override string ToString()
            {
                return "[" + _uint + "," + _uintWrapper + "]";
            }
        }

    }
}
