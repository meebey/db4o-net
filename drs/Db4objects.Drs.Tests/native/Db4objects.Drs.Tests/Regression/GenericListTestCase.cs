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
		public class RootContainer : Container
		{
			public RootContainer(string name, IEnumerable items) : base(name, items)
			{
			}
		}

		public override object CreateItem()
        {
            return new RootContainer("ROOT", (IEnumerable) SubjectFixtureProvider.Value());
        }

        public override void EnsureContent(ITestableReplicationProviderInside provider)
        {	
            Container replicated = (Container) QueryItem(provider, typeof(RootContainer));
            Container expected = (Container) CreateItem();
			Assert.AreSame(expected.GetType(), replicated.GetType());
            Assert.AreNotSame(expected, replicated);
            Assert.AreEqual(expected._name, replicated._name);
            Iterator4Assert.AreEqual(expected._items.GetEnumerator(), replicated._items.GetEnumerator());
        }
    }
}
