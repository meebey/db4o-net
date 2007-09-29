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
namespace Db4objects.Drs.Tests
{
	/// <exclude></exclude>
	public abstract class DrsTestSuite : Db4objects.Drs.Tests.DrsTestCase, Db4oUnit.ITestSuiteBuilder
	{
		public virtual Db4oUnit.TestSuite Build()
		{
			return new Db4objects.Drs.Tests.DrsTestSuiteBuilder(A(), B(), TestCases()).Build(
				);
		}

		protected System.Type[] TestCases()
		{
			return Concat(Shared(), SpecificTestCases());
		}

		protected abstract System.Type[] SpecificTestCases();

		private System.Type[] Shared()
		{
			return new System.Type[] { typeof(Db4objects.Drs.Tests.Foundation.AllTests), typeof(Db4objects.Drs.Tests.TheSimplest)
				, typeof(Db4objects.Drs.Tests.ReplicationEventTest), typeof(Db4objects.Drs.Tests.ReplicationProviderTest)
				, typeof(Db4objects.Drs.Tests.ReplicationAfterDeletionTest), typeof(Db4objects.Drs.Tests.SimpleArrayTest)
				, typeof(Db4objects.Drs.Tests.SimpleParentChild), typeof(Db4objects.Drs.Tests.ByteArrayTest)
				, typeof(Db4objects.Drs.Tests.ListTest), typeof(Db4objects.Drs.Tests.Db4oListTest)
				, typeof(Db4objects.Drs.Tests.R0to4Runner), typeof(Db4objects.Drs.Tests.ReplicationFeaturesMain)
				, typeof(Db4objects.Drs.Tests.Regression.DRS42Test) };
		}

		private System.Type[] Concat(System.Type[] shared, System.Type[] db4oSpecific)
		{
			Db4objects.Db4o.Foundation.Collection4 c = new Db4objects.Db4o.Foundation.Collection4
				(shared);
			c.AddAll(db4oSpecific);
			return (System.Type[])c.ToArray(new System.Type[c.Size()]);
		}
	}
}
