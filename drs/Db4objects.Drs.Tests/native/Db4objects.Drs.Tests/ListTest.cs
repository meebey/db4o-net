/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to Versant, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
using System.Collections;
using Db4oUnit;

namespace Db4objects.Drs.Tests
{
    public class ListTest : DrsTestCase
    {
        public virtual void Test()
        {
            ActualTest();
        }

        protected virtual void ActualTest()
        {
            StoreListToProviderA();
            ReplicateAllToProviderBFirstTime();
            ModifyInProviderB();
            ReplicateAllStep2();
            AddElementInProviderA();
            ReplicateHolderStep3();
        }

        private void StoreListToProviderA()
        {
            ListHolder lh = CreateHolder();
            ListContent lc1 = new ListContent("c1");
            ListContent lc2 = new ListContent("c2");
            lh.Add(lc1);
            lh.Add(lc2);
            A().Provider().StoreNew(lh);
            A().Provider().Commit();
            EnsureContent(A(), new string[] {"h1"}, new string[] {"c1", "c2"});
        }

        protected virtual ListHolder CreateHolder()
        {
            ListHolder lh = new ListHolder("h1");
            lh.SetList(new ArrayList());
            return lh;
        }

        private void ReplicateAllToProviderBFirstTime()
        {
            ReplicateAll(A().Provider(), B().Provider());
            EnsureContent(A(), new string[] {"h1"}, new string[] {"c1", "c2"});
            EnsureContent(B(), new string[] {"h1"}, new string[] {"c1", "c2"});
        }

        private void ModifyInProviderB()
        {
            ListHolder lh = (ListHolder) GetOneInstance
                                             (B(), typeof (ListHolder));
            lh.SetName("h2");
            IEnumerator itor = lh.GetList().GetEnumerator();
            itor.MoveNext();
            ListContent lc1 = (ListContent) itor.Current;
            itor.MoveNext();
            ListContent lc2 = (ListContent) itor.Current;
            lc1.SetName("co1");
            lc2.SetName("co2");
            B().Provider().Update(lc1);
            B().Provider().Update(lc2);
            B().Provider().Update(lh.GetList());
            B().Provider().Update(lh);
            B().Provider().Commit();
            EnsureContent(B(), new string[] {"h2"}, new string[] {"co1", "co2"});
        }

        private void ReplicateAllStep2()
        {
            ReplicateAll(B().Provider(), A().Provider());
            EnsureContent(B(), new string[] {"h2"}, new string[] {"co1", "co2"});
            EnsureContent(A(), new string[] {"h2"}, new string[] {"co1", "co2"});
        }

        private void AddElementInProviderA()
        {
            ListHolder lh = (ListHolder) GetOneInstance
                                             (A(), typeof (ListHolder));
            lh.SetName("h3");
            ListContent lc3 = new ListContent("co3");
            A().Provider().StoreNew(lc3);
            lh.GetList().Add(lc3);
            A().Provider().Update(lh.GetList());
            A().Provider().Update(lh);
            A().Provider().Commit();
            EnsureContent(A(), new string[] {"h3"}, new string[] {"co1", "co2", "co3"});
        }

        private void ReplicateHolderStep3()
        {
            ReplicateClass(A().Provider(), B().Provider(), typeof (ListHolder)
                );
            EnsureContent(A(), new string[] {"h3"}, new string[] {"co1", "co2", "co3"});
            EnsureContent(B(), new string[] {"h3"}, new string[] {"co1", "co2", "co3"});
        }

        private void EnsureContent(IDrsProviderFixture fixture, string[] holderNames, string[] contentNames)
        {
            int holderCount = holderNames.Length;
            EnsureInstanceCount(fixture, typeof (ListHolder), holderCount);
            int i = 0;
            IEnumerator objectSet = fixture.Provider().GetStoredObjects(typeof (ListHolder)).GetEnumerator();
            while (objectSet.MoveNext())
            {
                ListHolder lh = (ListHolder) objectSet.Current;
                Assert.AreEqual(holderNames[i], lh.GetName());
                IEnumerator itor = lh.GetList().GetEnumerator();
                int idx = 0;
                while (itor.MoveNext())
                {
                    Assert.AreEqual(contentNames[idx], ((ListContent) itor.Current).GetName());
                    idx++;
                }
            }
        }
    }
}