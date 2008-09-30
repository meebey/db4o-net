/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
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
