/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

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
namespace Db4objects.Drs.Test
{
	public class SimpleArrayTest : Db4objects.Drs.Test.DrsTestCase
	{
		public virtual void Test()
		{
			StoreListToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyInProviderB();
			ReplicateAllStep2();
			AddElementInProviderA();
			ReplicateHolderStep3();
		}

		protected override void Clean()
		{
			Delete(new System.Type[] { typeof(Db4objects.Drs.Test.SimpleArrayHolder), typeof(Db4objects.Drs.Test.SimpleArrayContent)
				 });
		}

		private void StoreListToProviderA()
		{
			Db4objects.Drs.Test.SimpleArrayHolder sah = new Db4objects.Drs.Test.SimpleArrayHolder
				("h1");
			Db4objects.Drs.Test.SimpleArrayContent sac1 = new Db4objects.Drs.Test.SimpleArrayContent
				("c1");
			Db4objects.Drs.Test.SimpleArrayContent sac2 = new Db4objects.Drs.Test.SimpleArrayContent
				("c2");
			sah.Add(sac1);
			sah.Add(sac2);
			A().Provider().StoreNew(sah);
			A().Provider().Commit();
			EnsureContent(A(), new string[] { "h1" }, new string[] { "c1", "c2" });
		}

		private void ReplicateAllToProviderBFirstTime()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureContent(A(), new string[] { "h1" }, new string[] { "c1", "c2" });
			EnsureContent(B(), new string[] { "h1" }, new string[] { "c1", "c2" });
		}

		private void ModifyInProviderB()
		{
			Db4objects.Drs.Test.SimpleArrayHolder sah = (Db4objects.Drs.Test.SimpleArrayHolder
				)GetOneInstance(B(), typeof(Db4objects.Drs.Test.SimpleArrayHolder));
			sah.SetName("h2");
			Db4objects.Drs.Test.SimpleArrayContent sac1 = sah.GetArr()[0];
			Db4objects.Drs.Test.SimpleArrayContent sac2 = sah.GetArr()[1];
			sac1.SetName("co1");
			sac2.SetName("co2");
			B().Provider().Update(sac1);
			B().Provider().Update(sac2);
			B().Provider().Update(sah);
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
			Db4objects.Drs.Test.SimpleArrayHolder sah = (Db4objects.Drs.Test.SimpleArrayHolder
				)GetOneInstance(A(), typeof(Db4objects.Drs.Test.SimpleArrayHolder));
			sah.SetName("h3");
			Db4objects.Drs.Test.SimpleArrayContent lc3 = new Db4objects.Drs.Test.SimpleArrayContent
				("co3");
			A().Provider().StoreNew(lc3);
			sah.Add(lc3);
			A().Provider().Update(sah);
			A().Provider().Commit();
			EnsureContent(A(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
		}

		private void ReplicateHolderStep3()
		{
			ReplicateClass(A().Provider(), B().Provider(), typeof(Db4objects.Drs.Test.SimpleArrayHolder)
				);
			EnsureContent(A(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
			EnsureContent(B(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
		}

		private void EnsureContent(Db4objects.Drs.Test.IDrsFixture fixture, string[] holderNames
			, string[] contentNames)
		{
			int holderCount = holderNames.Length;
			int contentCount = contentNames.Length;
			EnsureInstanceCount(fixture, typeof(Db4objects.Drs.Test.SimpleArrayHolder), holderCount
				);
			EnsureInstanceCount(fixture, typeof(Db4objects.Drs.Test.SimpleArrayContent), contentCount
				);
			int i = 0;
			Db4objects.Db4o.IObjectSet objectSet = fixture.Provider().GetStoredObjects(typeof(Db4objects.Drs.Test.SimpleArrayHolder)
				);
			System.Collections.IEnumerator iterator = objectSet.GetEnumerator();
			while (iterator.MoveNext())
			{
				Db4objects.Drs.Test.SimpleArrayHolder lh = (Db4objects.Drs.Test.SimpleArrayHolder
					)iterator.Current;
				Db4oUnit.Assert.AreEqual(holderNames[i], lh.GetName());
				Db4objects.Drs.Test.SimpleArrayContent[] sacs = lh.GetArr();
				for (int j = 0; j < contentNames.Length; j++)
				{
					Db4oUnit.Assert.AreEqual(contentNames[j], sacs[j].GetName());
				}
			}
		}
	}
}
