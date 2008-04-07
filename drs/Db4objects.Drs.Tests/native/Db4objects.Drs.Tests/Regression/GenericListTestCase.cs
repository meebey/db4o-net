using System;
using System.Collections;
using System.Collections.Generic;
using Db4objects.Drs.Inside;
using Db4oUnit;
using Db4oUnit.Fixtures;

namespace Db4objects.Drs.Tests.Regression
{
	class GenericListTestCase : GenericCollectionTestCaseBase
    {
        public class Container
        {
            public string _name;
            public IEnumerable _items;

            public Container(string name, IEnumerable items)
            {
                _name = name;
                _items = items;
            }
        }

        public override object CreateItem()
        {
            return new Container("Item One", (IEnumerable) SubjectFixtureProvider.Value());
        }

        public override void EnsureContent(ITestableReplicationProviderInside provider)
        {
            Container replicated = (Container) QueryItem(provider, typeof(Container));
            Container expected = (Container) CreateItem();
            Assert.AreNotSame(expected, replicated);
            Assert.AreEqual(expected._name, replicated._name);
            Iterator4Assert.AreEqual(expected._items.GetEnumerator(), replicated._items.GetEnumerator());
        }
    }
}
