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
using Db4oUnit;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class SimpleParentChild : DrsTestCase
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

		private void EnsureNames(IDrsFixture fixture, string parentName, string childName
			)
		{
			EnsureOneInstanceOfParentAndChild(fixture);
			SPCParent parent = (SPCParent)GetOneInstance(fixture, typeof(SPCParent));
			if (!parent.GetName().Equals(parentName))
			{
				Sharpen.Runtime.Out.WriteLine("expected = " + parentName);
				Sharpen.Runtime.Out.WriteLine("actual = " + parent.GetName());
			}
			Assert.AreEqual(parent.GetName(), parentName);
			Assert.AreEqual(parent.GetChild().GetName(), childName);
		}

		private void EnsureOneInstanceOfParentAndChild(IDrsFixture fixture)
		{
			EnsureOneInstance(fixture, typeof(SPCParent));
			EnsureOneInstance(fixture, typeof(SPCChild));
		}

		private void ModifyParentAndChildInProviderA()
		{
			SPCParent parent = (SPCParent)GetOneInstance(A(), typeof(SPCParent));
			parent.SetName("p3");
			SPCChild child = parent.GetChild();
			child.SetName("c3");
			A().Provider().Update(parent);
			A().Provider().Update(child);
			A().Provider().Commit();
			EnsureNames(A(), "p3", "c3");
		}

		private void ModifyParentInProviderB()
		{
			SPCParent parent = (SPCParent)GetOneInstance(B(), typeof(SPCParent));
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
			ReplicateClass(A().Provider(), B().Provider(), typeof(SPCParent));
			EnsureNames(A(), "p3", "c3");
			EnsureNames(B(), "p3", "c3");
		}

		private void StoreParentAndChildToProviderA()
		{
			SPCChild child = new SPCChild("c1");
			SPCParent parent = new SPCParent(child, "p1");
			A().Provider().StoreNew(parent);
			A().Provider().Commit();
			EnsureNames(A(), "p1", "c1");
		}
	}
}
