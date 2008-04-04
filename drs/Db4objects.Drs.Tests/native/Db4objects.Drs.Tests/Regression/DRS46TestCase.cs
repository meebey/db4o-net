using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Drs.Inside;
using Db4objects.Db4o;
using Db4oUnit;

namespace Db4objects.Drs.Tests.Regression
{
    class DRS46TestCase : Db4objects.Drs.Tests.DrsTestCase
    {
        public class Container
        {
            public string _name;
            public IEnumerable<string> _items;

            public Container(string name, IEnumerable<string> items)
            {
                _name = name;
                _items = items;
            }
        }

		public Container Item()
        {
			return new Container("Item One", new List<string>(GenerateStrings(10)));
        }

        public void Test()
        {
            StoreToProviderA();
            ReplicateAllToProviderB();
			EnsureContent(B().Provider());
        }

        private void ReplicateAllToProviderB()
        {
            ReplicateAll(A().Provider(), B().Provider());
        }

        private void StoreToProviderA()
        {
            ITestableReplicationProviderInside providerA = A().Provider();
            providerA.StoreNew(Item());
            providerA.Commit();
        }

        private void EnsureContent(ITestableReplicationProviderInside provider)
        {
			Container replicated = QueryItem(provider);
			Container expected = Item();
			Assert.AreNotSame(expected, replicated);
            Assert.AreEqual(expected._name, replicated._name);
			Iterator4Assert.AreEqual(expected._items.GetEnumerator(), replicated._items.GetEnumerator());
        }

		private static Container QueryItem(ITestableReplicationProviderInside provider)
    	{
			IObjectSet result = provider.GetStoredObjects(typeof(Container));
    		Assert.AreEqual(1, result.Count);
			return (Container)result.Next();
    	}

		private IEnumerable<string> GenerateStrings(int count)
		{
			if (count < 0) throw new ArgumentOutOfRangeException("count");
			for (int i = 0; i < count; i++)
			{
				yield  return "string " + i;
			}
		}
    }
}
