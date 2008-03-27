/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	public class SimpleParentChild : Db4objects.Drs.Tests.DrsTestCase
	{
		public virtual void Test()
		{
			StoreParentAndChildToProviderA();
			ReplicateAllToProviderBFirstTime();
			ModifyParentInProviderB();
			ReplicateAllStep2();
			ModifyParentAndChildInProviderA();
			ReplicateParentClassStep3();
		}

		private void EnsureNames(Db4objects.Drs.Tests.IDrsFixture fixture, string parentName
			, string childName)
		{
			EnsureOneInstanceOfParentAndChild(fixture);
			Db4objects.Drs.Tests.SPCParent parent = (Db4objects.Drs.Tests.SPCParent)GetOneInstance
				(fixture, typeof(Db4objects.Drs.Tests.SPCParent));
			if (!parent.GetName().Equals(parentName))
			{
				Sharpen.Runtime.Out.WriteLine("expected = " + parentName);
				Sharpen.Runtime.Out.WriteLine("actual = " + parent.GetName());
			}
			Db4oUnit.Assert.AreEqual(parent.GetName(), parentName);
			Db4oUnit.Assert.AreEqual(parent.GetChild().GetName(), childName);
		}

		private void EnsureOneInstanceOfParentAndChild(Db4objects.Drs.Tests.IDrsFixture fixture
			)
		{
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Tests.SPCParent));
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Tests.SPCChild));
		}

		private void ModifyParentAndChildInProviderA()
		{
			Db4objects.Drs.Tests.SPCParent parent = (Db4objects.Drs.Tests.SPCParent)GetOneInstance
				(A(), typeof(Db4objects.Drs.Tests.SPCParent));
			parent.SetName("p3");
			Db4objects.Drs.Tests.SPCChild child = parent.GetChild();
			child.SetName("c3");
			A().Provider().Update(parent);
			A().Provider().Update(child);
			A().Provider().Commit();
			EnsureNames(A(), "p3", "c3");
		}

		private void ModifyParentInProviderB()
		{
			Db4objects.Drs.Tests.SPCParent parent = (Db4objects.Drs.Tests.SPCParent)GetOneInstance
				(B(), typeof(Db4objects.Drs.Tests.SPCParent));
			parent.SetName("p2");
			B().Provider().Update(parent);
			B().Provider().Commit();
			EnsureNames(B(), "p2", "c1");
		}

		private void ReplicateAllStep2()
		{
			ReplicateAll(B().Provider(), A().Provider());
			EnsureNames(A(), "p2", "c1");
			EnsureNames(B(), "p2", "c1");
		}

		private void ReplicateAllToProviderBFirstTime()
		{
			ReplicateAll(A().Provider(), B().Provider());
			EnsureNames(A(), "p1", "c1");
			EnsureNames(B(), "p1", "c1");
		}

		private void ReplicateParentClassStep3()
		{
			ReplicateClass(A().Provider(), B().Provider(), typeof(Db4objects.Drs.Tests.SPCParent
				));
			EnsureNames(A(), "p3", "c3");
			EnsureNames(B(), "p3", "c3");
		}

		private void StoreParentAndChildToProviderA()
		{
			Db4objects.Drs.Tests.SPCChild child = new Db4objects.Drs.Tests.SPCChild("c1");
			Db4objects.Drs.Tests.SPCParent parent = new Db4objects.Drs.Tests.SPCParent(child, 
				"p1");
			A().Provider().StoreNew(parent);
			A().Provider().Commit();
			EnsureNames(A(), "p1", "c1");
		}
	}
}
