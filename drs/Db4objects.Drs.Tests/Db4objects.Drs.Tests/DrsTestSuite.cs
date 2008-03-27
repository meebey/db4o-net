/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests
{
	/// <exclude></exclude>
	public abstract class DrsTestSuite : Db4objects.Drs.Tests.DrsTestCase, Db4oUnit.ITestSuiteBuilder
	{
		public virtual System.Collections.IEnumerator GetEnumerator()
		{
			return new Db4objects.Drs.Tests.DrsTestSuiteBuilder(A(), B(), TestCases()).GetEnumerator
				();
		}

		protected System.Type[] TestCases()
		{
			return Concat(Shared(), SpecificTestCases());
		}

		protected abstract System.Type[] SpecificTestCases();

		private System.Type[] Shared()
		{
			return new System.Type[] { typeof(Db4objects.Drs.Tests.Foundation.AllTests), typeof(
				Db4objects.Drs.Tests.TheSimplest), typeof(Db4objects.Drs.Tests.ReplicationEventTest
				), typeof(Db4objects.Drs.Tests.ReplicationProviderTest), typeof(Db4objects.Drs.Tests.ReplicationAfterDeletionTest
				), typeof(Db4objects.Drs.Tests.SimpleArrayTest), typeof(Db4objects.Drs.Tests.SimpleParentChild
				), typeof(Db4objects.Drs.Tests.ByteArrayTest), typeof(Db4objects.Drs.Tests.ListTest
				), typeof(Db4objects.Drs.Tests.Db4oListTest), typeof(Db4objects.Drs.Tests.R0to4Runner
				), typeof(Db4objects.Drs.Tests.ReplicationFeaturesMain), typeof(Db4objects.Drs.Tests.CollectionHandlerImplTest
				), typeof(Db4objects.Drs.Tests.ReplicationTraversalTest), typeof(Db4objects.Drs.Tests.SingleTypeCollectionReplicationTest
				), typeof(Db4objects.Drs.Tests.Regression.DRS42Test) };
		}

		// Simple
		// Collection
		// Complex
		// General
		//TODO Convert to .NET
		//MapTest.class,
		//ArrayReplicationTest.class,
		//MixedTypesCollectionReplicationTest.class
		//regression
		private System.Type[] Concat(System.Type[] shared, System.Type[] db4oSpecific)
		{
			Db4objects.Db4o.Foundation.Collection4 c = new Db4objects.Db4o.Foundation.Collection4
				(shared);
			c.AddAll(db4oSpecific);
			return (System.Type[])c.ToArray(new System.Type[c.Size()]);
		}
	}
}
