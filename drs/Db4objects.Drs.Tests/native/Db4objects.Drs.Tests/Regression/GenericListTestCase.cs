using System;
using System.Collections.Generic;
using Db4objects.Drs.Inside;
using Db4oUnit;

namespace Db4objects.Drs.Tests.Regression
{
    class GenericListTestCase : GenericCollectionTestCaseBase
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

        public override object CreateItem()
        {
            return new Container("Item One", new List<string>(GenerateStrings(10)));
        }

        public override void EnsureContent(ITestableReplicationProviderInside provider)
        {
            Container replicated = (Container) QueryItem(provider, typeof(Container));
            Container expected = (Container) CreateItem();
            Assert.AreNotSame(expected, replicated);
            Assert.AreEqual(expected._name, replicated._name);
            Iterator4Assert.AreEqual(expected._items.GetEnumerator(), replicated._items.GetEnumerator());
        }

        private IEnumerable<string> GenerateStrings(int count)
        {
            if (count < 0) throw new ArgumentOutOfRangeException("count");
            for (int i = 0; i < count; i++)
            {
                yield return "string " + i;
            }
        }
    }
}
