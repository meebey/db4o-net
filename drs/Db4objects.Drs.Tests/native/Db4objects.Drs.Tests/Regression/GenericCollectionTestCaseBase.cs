using System;
using System.Collections.Generic;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Inside;
using Db4objects.Db4o;
using Db4oUnit;

namespace Db4objects.Drs.Tests.Regression
{
    abstract class GenericCollectionTestCaseBase : DrsTestCase
    {
        public void Test()
        {
            StoreToProviderA();
            ReplicateAllToProviderB();
            EnsureContent(B().Provider());
        }

        private void StoreToProviderA()
        {
            ITestableReplicationProviderInside providerA = A().Provider();
            providerA.StoreNew(CreateItem());
            providerA.Commit();
        }
        
        private void ReplicateAllToProviderB()
        {
            ReplicateAll(A().Provider(), B().Provider());
        }

        public object QueryItem(ITestableReplicationProviderInside provider, Type type)
        {
            IObjectSet result = provider.GetStoredObjects(type);
            Assert.AreEqual(1, result.Count);
            return result.Next();
        }

        public abstract object CreateItem();

        public abstract void EnsureContent(ITestableReplicationProviderInside provider);
    }
}
