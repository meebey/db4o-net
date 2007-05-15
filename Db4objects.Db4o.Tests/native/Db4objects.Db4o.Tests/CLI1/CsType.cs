/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1
{
    public class CsType : AbstractDb4oTestCase
    {
        Type myType;
        Type stringType;

        override protected void Store()
        {
            myType = this.GetType();
            stringType = typeof(String);
            Store(this);
        }

        public void Test()
        {
            Assert.AreEqual(this.GetType(), myType);
            Assert.AreEqual(typeof(String), stringType);
        }
    }
}
