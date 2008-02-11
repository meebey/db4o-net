/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Tests.Common.Defragment;

namespace Db4objects.Db4o.Tests.Common.Defragment
{
	public abstract class AbstractDb4oDefragTestCase : ITest
	{
		public virtual string GetLabel()
		{
			return "DefragAllTestCase: " + TestSuite().FullName;
		}

		public abstract Type TestSuite();

		public virtual void Run(TestResult result)
		{
			try
			{
				new Db4oTestSuiteBuilder(new Db4oDefragSolo(new IndependentConfigurationSource())
					, TestSuite()).Build().Run(result);
			}
			catch (Exception e)
			{
				result.TestFailed(this, e);
			}
		}
	}
}
