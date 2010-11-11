/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Regression;

namespace Db4objects.Drs.Tests
{
	/// <exclude></exclude>
	public abstract class DrsTestSuite : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			if (false)
			{
				return SpecificTestCases();
			}
			if (false)
			{
				return new Type[] { typeof(ReplicatingTwiceTestCase) };
			}
			if (false)
			{
				return new Type[] { typeof(TheSimplest) };
			}
			return Concat(Shared(), SpecificTestCases());
		}

		protected abstract Type[] SpecificTestCases();

		private Type[] Shared()
		{
			return new Type[] { typeof(Db4objects.Drs.Tests.Foundation.AllTests), typeof(TheSimplest
				), typeof(ReplicationEventTest), typeof(ReplicationProviderTest), typeof(ReplicationAfterDeletionTest
				), typeof(SimpleArrayTest), typeof(SimpleParentChild), typeof(ByteArrayTest), typeof(
				ComplexListTestCase), typeof(ListTest), typeof(R0to4Runner), typeof(ReplicationFeaturesMain
				), typeof(CollectionHandlerImplTest), typeof(ReplicationTraversalTest), typeof(MapTest
				), typeof(ArrayReplicationTest), typeof(SingleTypeCollectionReplicationTest), typeof(
				MixedTypesCollectionReplicationTest), typeof(TransparentActivationTestCase), typeof(
				DRS42Test), typeof(SameHashCodeTestCase) };
		}

		// Simple
		// Collection
		// Complex
		// General
		//regression
		protected virtual Type[] Concat(Type[] x, Type[] y)
		{
			Collection4 c = new Collection4(x);
			c.AddAll(y);
			return (Type[])c.ToArray(new Type[c.Size()]);
		}
	}
}
