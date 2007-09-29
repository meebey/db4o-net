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
	public class SimpleParentChild : Db4objects.Drs.Test.DrsTestCase
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

		private void EnsureNames(Db4objects.Drs.Test.IDrsFixture fixture, string parentName
			, string childName)
		{
			EnsureOneInstanceOfParentAndChild(fixture);
			Db4objects.Drs.Test.SPCParent parent = (Db4objects.Drs.Test.SPCParent)GetOneInstance
				(fixture, typeof(Db4objects.Drs.Test.SPCParent));
			if (!parent.GetName().Equals(parentName))
			{
				Sharpen.Runtime.Out.WriteLine("expected = " + parentName);
				Sharpen.Runtime.Out.WriteLine("actual = " + parent.GetName());
			}
			Db4oUnit.Assert.AreEqual(parent.GetName(), parentName);
			Db4oUnit.Assert.AreEqual(parent.GetChild().GetName(), childName);
		}

		private void EnsureOneInstanceOfParentAndChild(Db4objects.Drs.Test.IDrsFixture fixture
			)
		{
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Test.SPCParent));
			EnsureOneInstance(fixture, typeof(Db4objects.Drs.Test.SPCChild));
		}

		private void ModifyParentAndChildInProviderA()
		{
			Db4objects.Drs.Test.SPCParent parent = (Db4objects.Drs.Test.SPCParent)GetOneInstance
				(A(), typeof(Db4objects.Drs.Test.SPCParent));
			parent.SetName("p3");
			Db4objects.Drs.Test.SPCChild child = parent.GetChild();
			child.SetName("c3");
			A().Provider().Update(parent);
			A().Provider().Update(child);
			A().Provider().Commit();
			EnsureNames(A(), "p3", "c3");
		}

		private void ModifyParentInProviderB()
		{
			Db4objects.Drs.Test.SPCParent parent = (Db4objects.Drs.Test.SPCParent)GetOneInstance
				(B(), typeof(Db4objects.Drs.Test.SPCParent));
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
			ReplicateClass(A().Provider(), B().Provider(), typeof(Db4objects.Drs.Test.SPCParent)
				);
			EnsureNames(A(), "p3", "c3");
			EnsureNames(B(), "p3", "c3");
		}

		private void StoreParentAndChildToProviderA()
		{
			Db4objects.Drs.Test.SPCChild child = new Db4objects.Drs.Test.SPCChild("c1");
			Db4objects.Drs.Test.SPCParent parent = new Db4objects.Drs.Test.SPCParent(child, "p1"
				);
			A().Provider().StoreNew(parent);
			A().Provider().Commit();
			EnsureNames(A(), "p1", "c1");
		}
	}
}
