/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Querying;

namespace Db4objects.Db4o.Tests.Common.Querying
{
	public class AllTests : Db4oTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Querying.AllTests().RunSoloAndClientServer();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(CascadedDeleteUpdate), typeof(CascadeDeleteArray), typeof(
				CascadeDeleteDeleted), typeof(CascadeDeleteFalse), typeof(CascadeOnActivate), typeof(
				CascadeOnDelete), typeof(CascadeOnDeleteHierarchyTestCase), typeof(CascadeOnUpdate
				), typeof(CascadeToArray), typeof(ConjunctiveQbETestCase), typeof(DescendIndexQueryTestCase
				), typeof(IdListQueryResultTestCase), typeof(IndexedJoinQueriesTestCase), typeof(
				IndexOnParentFieldTestCase), typeof(IndexedQueriesTestCase), typeof(LazyQueryResultTestCase
				), typeof(MultiFieldIndexQueryTestCase), typeof(NullConstraintQueryTestCase), typeof(
				ObjectSetTestCase), typeof(OrderedQueryTestCase), typeof(QueryByExampleTestCase)
				, typeof(QueryingVersionFieldTestCase) };
		}
	}
}
