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
using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Drs.Tests;
using Db4objects.Drs.Tests.Foundation;
using Db4objects.Drs.Tests.Regression;

namespace Db4objects.Drs.Tests
{
	/// <exclude></exclude>
	public abstract class DrsTestSuite : ReflectionTestSuite
	{
		protected sealed override Type[] TestCases()
		{
			//		if (true) return specificTestCases();
			//		if (true) return new Class[] { UntypedFieldTestCase.class };
			return Concat(Shared(), SpecificTestCases());
		}

		protected abstract Type[] SpecificTestCases();

		private Type[] Shared()
		{
			return new Type[] { typeof(AllTests), typeof(TheSimplest), typeof(ReplicationEventTest
				), typeof(ReplicationProviderTest), typeof(ReplicationAfterDeletionTest), typeof(
				SimpleArrayTest), typeof(SimpleParentChild), typeof(ByteArrayTest), typeof(ComplexListTestCase
				), typeof(ListTest), typeof(Db4oListTest), typeof(R0to4Runner), typeof(ReplicationFeaturesMain
				), typeof(CollectionHandlerImplTest), typeof(ReplicationTraversalTest), typeof(MapTest
				), typeof(ArrayReplicationTest), typeof(SingleTypeCollectionReplicationTest), typeof(
				MixedTypesCollectionReplicationTest), typeof(DRS42Test) };
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
