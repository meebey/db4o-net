/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4oUnit;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.CLI1.Handlers
{
    class SByteHandlerTestCase : TypeHandlerTestCaseBase
    {
		protected override void Configure(IConfiguration config)
		{
			config.ExceptionsOnNotStorable(false);
		}

		public virtual void TestReadWrite()
        {
            MockWriteContext writeContext = new MockWriteContext(Db());
            sbyte expected = 0x11;
            SByteHandler().Write(writeContext, expected);
            MockReadContext readContext = new MockReadContext(writeContext);
            sbyte sbyteValue = (sbyte)SByteHandler().Read(readContext);
            Assert.AreEqual(expected, sbyteValue);
        }

        public virtual void TestStoreObject()
        {
            SByteHandlerTestCase.Item storedItem = new SByteHandlerTestCase.Item(0x11, 0x22);
            DoTestStoreObject(storedItem);
        }

        private Db4objects.Db4o.Internal.Handlers.SByteHandler SByteHandler()
        {
            return new Db4objects.Db4o.Internal.Handlers.SByteHandler();
        }

        public class Item
        {
            public sbyte _sbyte;

            public SByte _sbyteWrapper;

            public Item(sbyte s, SByte wrapper)
            {
                _sbyte = s;
                _sbyteWrapper = wrapper;
            }

            public override bool Equals(object obj)
            {
                if (obj == this)
                {
                    return true;
                }
                if (!(obj is SByteHandlerTestCase.Item))
                {
                    return false;
                }
                SByteHandlerTestCase.Item other = (SByteHandlerTestCase.Item)obj;
                return (other._sbyte == this._sbyte) && this._sbyteWrapper.Equals(other._sbyteWrapper
                    );
            }

            public override string ToString()
            {
                return "[" + _sbyte + "," + _sbyteWrapper + "]";
            }
        }

    }
}
