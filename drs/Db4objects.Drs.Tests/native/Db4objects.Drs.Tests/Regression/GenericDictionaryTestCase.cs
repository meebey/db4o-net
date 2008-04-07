using System;
using System.Collections.Generic;
using System.Collections;
using Db4objects.Drs.Inside;
using Db4oUnit;

namespace Db4objects.Drs.Tests.Regression
{
    class GenericDictionaryTestCase : GenericCollectionTestCaseBase
    {
        public class Container
        {
            public string _name;
            public IDictionary<string, int> _items;

            public Container(string name, IDictionary<string, int> items)
            {
                _name = name;
                _items = items;
            }
        }

        public override object CreateItem()
        {
            Dictionary<string, int> dictionary = new Dictionary<string, int>();
            for (int i = 0; i < 10; i++)
            {
                dictionary.Add("Key " + i, i);
            }
            Container container = new Container("Dictionary Item", dictionary);
            return container;
        }

        public override void EnsureContent(ITestableReplicationProviderInside provider)
        {
            Container replicated = (Container)QueryItem(provider, typeof(Container));
            Container expected = (Container)CreateItem();
            Assert.AreNotSame(expected, replicated);
            Assert.AreEqual(expected._name, replicated._name);
            Iterator4Assert.AreEqual(expected._items.GetEnumerator(), replicated._items.GetEnumerator());
        }
    }
}
