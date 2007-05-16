/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;
using Db4objects.Db4o.Query;
using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.CLI1.Aliases
{
    public class BaseAliasesTestCase : AbstractDb4oTestCase
    {
        protected void AssertAliasedData(IObjectContainer container)
        {
            AssertAliasedData(container.Get(GetAliasedDataType()));

            AssertAliasedData(QueryAliasedData(container));
        }

        private IObjectSet QueryAliasedData(IObjectContainer container)
        {
            IQuery query = container.Query();
            query.Constrain(GetAliasedDataType());
            return query.Execute();
        }

        private void AssertAliasedData(IObjectSet os)
        {
            Assert.AreEqual(2, os.Size());
            AssertContains(os, CreateAliasedData("Homer Simpson"));
            AssertContains(os, CreateAliasedData("John Cleese"));
        }

        protected virtual Type GetAliasedDataType()
        {
            return typeof(Person2);
        }

        protected object CreateAliasedData(string name)
        {
            return Activator.CreateInstance(GetAliasedDataType(), new object[] { name });
        }

        public static void AssertContains(IObjectSet actual, object expected)
        {
            actual.Reset();
            while (actual.HasNext())
            {
                object next = actual.Next();
                if (CFHelper.AreEqual(next, expected)) return;
            }
            Assert.Fail("Expected item: " + expected);
        }

    }
}
