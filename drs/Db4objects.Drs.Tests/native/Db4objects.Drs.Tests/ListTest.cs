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
namespace Db4objects.Drs.Tests
{
	public class ListTest : Db4objects.Drs.Tests.DrsTestCase
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
			Db4objects.Drs.Tests.ListHolder lh = CreateHolder();
			Db4objects.Drs.Tests.ListContent lc1 = new Db4objects.Drs.Tests.ListContent("c1");
			Db4objects.Drs.Tests.ListContent lc2 = new Db4objects.Drs.Tests.ListContent("c2");
			lh.Add(lc1);
			lh.Add(lc2);
			A().Provider().StoreNew(lh);
			A().Provider().Commit();
			EnsureContent(A(), new string[] { "h1" }, new string[] { "c1", "c2" });
		}

		protected virtual Db4objects.Drs.Tests.ListHolder CreateHolder()
		{
			Db4objects.Drs.Tests.ListHolder lh = new Db4objects.Drs.Tests.ListHolder("h1");
			lh.SetList(new System.Collections.ArrayList());
			return lh;
		}

		private void ReplicateAllToProviderBFirstTime()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureContent(A(), new string[] { "h1" }, new string[] { "c1", "c2" });
			EnsureContent(B(), new string[] { "h1" }, new string[] { "c1", "c2" });
		}

		private void ModifyInProviderB()
		{
			Db4objects.Drs.Tests.ListHolder lh = (Db4objects.Drs.Tests.ListHolder)GetOneInstance
				(B(), typeof(Db4objects.Drs.Tests.ListHolder));
			lh.SetName("h2");
			System.Collections.IEnumerator itor = lh.GetList().GetEnumerator();
            itor.MoveNext();
			Db4objects.Drs.Tests.ListContent lc1 = (Db4objects.Drs.Tests.ListContent)itor.Current;
            itor.MoveNext();
			Db4objects.Drs.Tests.ListContent lc2 = (Db4objects.Drs.Tests.ListContent)itor.Current;
			lc1.SetName("co1");
			lc2.SetName("co2");
			B().Provider().Update(lc1);
			B().Provider().Update(lc2);
			B().Provider().Update(lh.GetList());
			B().Provider().Update(lh);
			B().Provider().Commit();
			EnsureContent(B(), new string[] { "h2" }, new string[] { "co1", "co2" });
		}

		private void ReplicateAllStep2()
		{
			ReplicateAll(B().Provider(), A().Provider());
			EnsureContent(B(), new string[] { "h2" }, new string[] { "co1", "co2" });
			EnsureContent(A(), new string[] { "h2" }, new string[] { "co1", "co2" });
		}

		private void AddElementInProviderA()
		{
			Db4objects.Drs.Tests.ListHolder lh = (Db4objects.Drs.Tests.ListHolder)GetOneInstance
				(A(), typeof(Db4objects.Drs.Tests.ListHolder));
			lh.SetName("h3");
			Db4objects.Drs.Tests.ListContent lc3 = new Db4objects.Drs.Tests.ListContent("co3");
			A().Provider().StoreNew(lc3);
			lh.GetList().Add(lc3);
			A().Provider().Update(lh.GetList());
			A().Provider().Update(lh);
			A().Provider().Commit();
			EnsureContent(A(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
		}

		private void ReplicateHolderStep3()
		{
			ReplicateClass(A().Provider(), B().Provider(), typeof(Db4objects.Drs.Tests.ListHolder)
				);
			EnsureContent(A(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
			EnsureContent(B(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
		}

		private void EnsureContent(Db4objects.Drs.Tests.IDrsFixture fixture, string[] holderNames
			, string[] contentNames)
		{
			int holderCount = holderNames.Length;
			EnsureInstanceCount(fixture, typeof(Db4objects.Drs.Tests.ListHolder), holderCount);
			int i = 0;
			System.Collections.IEnumerator objectSet = fixture.Provider().GetStoredObjects(typeof(Db4objects.Drs.Tests.ListHolder)
				).GetEnumerator();
			while (objectSet.MoveNext())
			{
				Db4objects.Drs.Tests.ListHolder lh = (Db4objects.Drs.Tests.ListHolder)objectSet.Current;
				Db4oUnit.Assert.AreEqual(holderNames[i], lh.GetName());
				System.Collections.IEnumerator itor = lh.GetList().GetEnumerator();
				int idx = 0;
				while (itor.MoveNext())
				{
					Db4oUnit.Assert.AreEqual(contentNames[idx], ((Db4objects.Drs.Tests.ListContent)itor
						.Current).GetName());
					idx++;
				}
			}
		}
	}
}
