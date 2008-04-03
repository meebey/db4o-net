/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Foundation;
using Db4objects.Drs.Tests.Regression;

namespace Db4objects.Drs.Tests
{
	/// <exclude></exclude>
	public abstract class DrsTestSuite : DrsTestCase, ITestSuiteBuilder
	{
		public virtual IEnumerator GetEnumerator()
		{
			return new DrsTestSuiteBuilder(A(), B(), TestCases()).GetEnumerator();
		}

		protected Type[] TestCases()
		{
			return Concat(Shared(), SpecificTestCases());
		}

		protected abstract Type[] SpecificTestCases();

		private Type[] Shared()
		{
			return new Type[] { typeof(AllTests), typeof(TheSimplest), typeof(ReplicationEventTest
				), typeof(ReplicationProviderTest), typeof(ReplicationAfterDeletionTest), typeof(
				SimpleArrayTest), typeof(SimpleParentChild), typeof(ByteArrayTest), typeof(ListTest
				), typeof(Db4oListTest), typeof(R0to4Runner), typeof(ReplicationFeaturesMain), typeof(
				CollectionHandlerImplTest), typeof(ReplicationTraversalTest), typeof(SingleTypeCollectionReplicationTest
				), typeof(DRS42Test) };
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
		private Type[] Concat(Type[] shared, Type[] db4oSpecific)
		{
			Collection4 c = new Collection4(shared);
			c.AddAll(db4oSpecific);
			return (Type[])c.ToArray(new Type[c.Size()]);
		}
	}
}
