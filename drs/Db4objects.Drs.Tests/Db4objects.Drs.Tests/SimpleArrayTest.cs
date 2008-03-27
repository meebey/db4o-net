/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class SimpleArrayTest : Db4objects.Drs.Tests.DrsTestCase
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
			Delete(new System.Type[] { typeof(Db4objects.Drs.Tests.SimpleArrayHolder), typeof(
				Db4objects.Drs.Tests.SimpleArrayContent) });
		}

		private void StoreListToProviderA()
		{
			Db4objects.Drs.Tests.SimpleArrayHolder sah = new Db4objects.Drs.Tests.SimpleArrayHolder
				("h1");
			Db4objects.Drs.Tests.SimpleArrayContent sac1 = new Db4objects.Drs.Tests.SimpleArrayContent
				("c1");
			Db4objects.Drs.Tests.SimpleArrayContent sac2 = new Db4objects.Drs.Tests.SimpleArrayContent
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
			Db4objects.Drs.Tests.SimpleArrayHolder sah = (Db4objects.Drs.Tests.SimpleArrayHolder
				)GetOneInstance(B(), typeof(Db4objects.Drs.Tests.SimpleArrayHolder));
			sah.SetName("h2");
			Db4objects.Drs.Tests.SimpleArrayContent sac1 = sah.GetArr()[0];
			Db4objects.Drs.Tests.SimpleArrayContent sac2 = sah.GetArr()[1];
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
			Db4objects.Drs.Tests.SimpleArrayHolder sah = (Db4objects.Drs.Tests.SimpleArrayHolder
				)GetOneInstance(A(), typeof(Db4objects.Drs.Tests.SimpleArrayHolder));
			sah.SetName("h3");
			Db4objects.Drs.Tests.SimpleArrayContent lc3 = new Db4objects.Drs.Tests.SimpleArrayContent
				("co3");
			A().Provider().StoreNew(lc3);
			sah.Add(lc3);
			A().Provider().Update(sah);
			A().Provider().Commit();
			EnsureContent(A(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
		}

		private void ReplicateHolderStep3()
		{
			ReplicateClass(A().Provider(), B().Provider(), typeof(Db4objects.Drs.Tests.SimpleArrayHolder
				));
			EnsureContent(A(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
			EnsureContent(B(), new string[] { "h3" }, new string[] { "co1", "co2", "co3" });
		}

		private void EnsureContent(Db4objects.Drs.Tests.IDrsFixture fixture, string[] holderNames
			, string[] contentNames)
		{
			int holderCount = holderNames.Length;
			int contentCount = contentNames.Length;
			EnsureInstanceCount(fixture, typeof(Db4objects.Drs.Tests.SimpleArrayHolder), holderCount
				);
			EnsureInstanceCount(fixture, typeof(Db4objects.Drs.Tests.SimpleArrayContent), contentCount
				);
			int i = 0;
			Db4objects.Db4o.IObjectSet objectSet = fixture.Provider().GetStoredObjects(typeof(
				Db4objects.Drs.Tests.SimpleArrayHolder));
			System.Collections.IEnumerator iterator = objectSet.GetEnumerator();
			while (iterator.MoveNext())
			{
				Db4objects.Drs.Tests.SimpleArrayHolder lh = (Db4objects.Drs.Tests.SimpleArrayHolder
					)iterator.Current;
				Db4oUnit.Assert.AreEqual(holderNames[i], lh.GetName());
				//Test.ensure(holderNames[i].equals(lh.getName()));
				Db4objects.Drs.Tests.SimpleArrayContent[] sacs = lh.GetArr();
				for (int j = 0; j < contentNames.Length; j++)
				{
					Db4oUnit.Assert.AreEqual(contentNames[j], sacs[j].GetName());
				}
			}
		}
		//Test.ensure(contentNames[j].equals(sacs[j].getName()));
	}
}
