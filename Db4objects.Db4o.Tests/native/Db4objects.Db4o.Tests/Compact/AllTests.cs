/* Copyright (C) 2004-2008   db4objects Inc.   http://www.db4o.com */

using System;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Tests.Compact
{
	class AllTests : Db4oTestSuite
	{
		protected override Type[] TestCases()
		{
#if CF_3_5
			return new Type[] { typeof(UnoptimizedLinqTestCase), };
#else
			return new Type[0] ;
#endif
		}
	}
}
