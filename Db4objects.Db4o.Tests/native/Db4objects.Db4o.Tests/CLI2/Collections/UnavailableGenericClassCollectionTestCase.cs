/* Copyright (C) 2009   Versant Inc.   http://www.db4o.com */
#if NET_3_5 && ! CF

using System;
using System.Collections.Generic;
using System.Linq;
using Db4objects.Db4o.Tests.Common.Assorted;
using Db4oUnit;

namespace Db4objects.Db4o.Tests.CLI2.Collections
{
    public class UnavailableGenericClassCollectionTestCase : UnavailableClassTestCaseBase
    {


        public class ListHolder
        {
            private LinkedList<ListItem> _list = new LinkedList<ListItem>();

            public LinkedList<ListItem> List()
            {
                return _list;
            }

        }
        
        public class ListItem
        {
            private string _name;

            public ListItem(string name)
            {
                _name = name;
            }

            public string Name
            {
                get { return _name; }
            }
        }

        protected override void Store()
        {
            ListItem listItem = new ListItem("name");
            ListHolder listHolder = new ListHolder();
            listHolder.List().AddFirst(listItem);
            Store(listHolder);
        }

        public void Test()
        {
            var originalName = "UnavailableGenericClassCollectionTestCase";
            var modifiedName = "FooBar";

            ReopenHidingClasses(new Type[]{typeof(ListHolder), typeof(ListItem)});

            RenameClasses(originalName, modifiedName);

            ReopenHidingClasses(new Type[] { typeof(ListHolder), typeof(ListItem) });

            RenameClasses(modifiedName, originalName);

            ReopenHidingClasses(new Type[] { });

            ListHolder listHolder = (ListHolder)RetrieveOnlyInstance(typeof(ListHolder));

            var item = listHolder.List().First();

            Assert.AreEqual("name", item.Name);
        }

        private void RenameClasses(string originalName, string modifiedName)
        {
            var classes = from storedClass in Db().StoredClasses()
                          let name = storedClass.GetName()
                          where name != null && name.Contains(originalName)
                          select new { StoredClass = storedClass, Name = name };
            foreach(var clazz in classes)
            {
                var newName = clazz.Name.Replace(originalName, modifiedName);
                clazz.StoredClass.Rename(newName);
            }
        }
    }
}
#endif