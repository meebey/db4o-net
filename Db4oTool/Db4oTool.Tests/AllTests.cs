/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using Db4oTool.Tests.Core;
using Db4oUnit;

namespace Db4oTool.Tests
{
	class AllTests : ReflectionTestSuite
	{
		protected override Type[] TestCases()
		{
			return new Type[]
				{
					typeof(ProgramOptionsTestCase),
					typeof(Core.AllTests),
					typeof(NQ.AllTests),
					typeof(TA.AllTests),
				};
		}
	}
}
