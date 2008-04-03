using System;
using System.Collections.Generic;
using Db4objects.Drs.Inside;
using Db4objects.Db4o;
using Db4oUnit;

namespace Db4objects.Drs.Tests.Regression
{
    class DRS46TestCase : Db4objects.Drs.Tests.DrsTestCase
    {
        public class GenericItem
        {
            public string _string;
            public List<string> _stringList;

            public GenericItem(string str, List<string> stringList)
            {
                _string = str;
                _stringList = stringList;
            }
        }

        GenericItem _item;

        public GenericItem Item()
        {
            if (_item == null)
            {
                List<string> list = new List<string>();
                for (int i = 0; i < 10; i++)
                {
                    list.Add("string " + i);
                }
                _item = new GenericItem("Item One", list);
            }
            return _item;
        }

        public void Test()
        {
            StoreToProviderA();
            ReplicateAllToProviderB();
        }

        private void ReplicateAllToProviderB()
        {
            ReplicateAll(A().Provider(), B().Provider());
            EnsureContent(A().Provider());
            EnsureContent(B().Provider());
        }

        private void StoreToProviderA()
        {
            ITestableReplicationProviderInside providerA = A().Provider();
            providerA.StoreNew(Item());
            providerA.Commit();
            EnsureContent(providerA);
        }

        private void EnsureContent(ITestableReplicationProviderInside provider)
        {
            IObjectSet result = provider.GetStoredObjects(typeof(GenericItem));
            Assert.AreEqual(1, result.Count);
            GenericItem item = (GenericItem) result.Next();
            GenericItem expected = Item();
            Assert.AreEqual(expected._string, item._string);

            IObjectSet set = provider.GetStoredObjects(typeof(List<string>));
            Console.WriteLine(set.Size());

            Assert.AreEqual(expected._stringList.Count, item._stringList.Count);
            for (int i = 0; i < expected._stringList.Count; i++)
            {
                Assert.AreEqual(expected._stringList[i], item._stringList[i]);
            }
        }
    }
}
