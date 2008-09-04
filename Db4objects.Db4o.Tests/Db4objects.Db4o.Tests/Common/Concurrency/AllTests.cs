/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Concurrency;

namespace Db4objects.Db4o.Tests.Common.Concurrency
{
	public class AllTests : Db4oConcurrencyTestSuite
	{
		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Concurrency.AllTests().RunConcurrency();
		}

		protected override Type[] TestCases()
		{
			return new Type[] { typeof(ReadObjectNQTestCase) };
		}
		//				ArrayNOrderTestCase.class, 
		//				ByteArrayTestCase.class,
		//				CascadeDeleteDeletedTestCase.class,
		//				CascadeDeleteFalseTestCase.class,
		//				CascadeOnActivateTestCase.class,
		//				CascadeOnUpdateTestCase.class,
		//				CascadeOnUpdate2TestCase.class,
		//				CascadeToVectorTestCase.class,
		//				CaseInsensitiveTestCase.class,
		//				Circular1TestCase.class,
		//				ClientDisconnectTestCase.class,
		//				CreateIndexInheritedTestCase.class,
		//				DeepSetTestCase.class,
		//				DeleteDeepTestCase.class,
		//				DifferentAccessPathsTestCase.class,
		//				ExtMethodsTestCase.class,
		//				GetAllTestCase.class,
		//				GreaterOrEqualTestCase.class,
		//				IndexedByIdentityTestCase.class,
		//				IndexedUpdatesWithNullTestCase.class,
		//				InternStringsTestCase.class,
		//				InvalidUUIDTestCase.class,
		//				IsStoredTestCase.class,
		//				MessagingTestCase.class,
		//				MultiDeleteTestCase.class,
		//				MultiLevelIndexTestCase.class,
		//				NestedArraysTestCase.class,
		//				ObjectSetIDsTestCase.class,
		//				ParameterizedEvaluationTestCase.class,
		//				PeekPersistedTestCase.class,
		//				PersistStaticFieldValuesTestCase.class,
		//				QueryForUnknownFieldTestCase.class,
		//				QueryNonExistantTestCase.class,
		//				ReadObjectQBETestCase.class,
		//				ReadObjectSODATestCase.class,
		//				RefreshTestCase.class,
		//				UpdateObjectTestCase.class,
	}
}
