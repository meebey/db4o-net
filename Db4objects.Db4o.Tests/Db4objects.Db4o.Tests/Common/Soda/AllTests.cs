/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Tests.Common.Soda;
using Db4objects.Db4o.Tests.Common.Soda.Classes.Simple;
using Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy;
using Db4objects.Db4o.Tests.Common.Soda.Classes.Untypedhierarchy;
using Db4objects.Db4o.Tests.Common.Soda.Joins.Typed;
using Db4objects.Db4o.Tests.Common.Soda.Joins.Untyped;
using Db4objects.Db4o.Tests.Common.Soda.Ordered;
using Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped;

namespace Db4objects.Db4o.Tests.Common.Soda
{
	public class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[] { typeof(STOrderingTestCase), typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.AllTests
				), typeof(CollectionIndexedJoinTestCase), typeof(NullIdentityConstraintTestCase)
				, typeof(QueryUnknownClassTestCase), typeof(SortMultipleTestCase), typeof(STBooleanTestCase
				), typeof(STBooleanWUTestCase), typeof(STByteTestCase), typeof(STByteWUTestCase)
				, typeof(STCharTestCase), typeof(STCharWUTestCase), typeof(STDoubleTestCase), typeof(
				STDoubleWUTestCase), typeof(STETH1TestCase), typeof(STFloatTestCase), typeof(STFloatWUTestCase
				), typeof(STIntegerTestCase), typeof(STIntegerWUTestCase), typeof(STLongTestCase
				), typeof(STLongWUTestCase), typeof(STOrTTestCase), typeof(STOrUTestCase), typeof(
				STOStringTestCase), typeof(STOIntegerTestCase), typeof(STOIntegerWTTestCase), typeof(
				STRTH1TestCase), typeof(STSDFT1TestCase), typeof(STShortTestCase), typeof(STShortWUTestCase
				), typeof(STStringUTestCase), typeof(STRUH1TestCase), typeof(STTH1TestCase), typeof(
				STUH1TestCase), typeof(UntypedEvaluationTestCase), typeof(JointEqualsIdentityTestCase
				) };
		}

		public static void Main(string[] args)
		{
			new Db4objects.Db4o.Tests.Common.Soda.AllTests().RunSolo();
		}
	}
}
