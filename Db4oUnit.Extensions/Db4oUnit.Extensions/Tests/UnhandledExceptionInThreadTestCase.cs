/* Copyright (C) 2004 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4oUnit.Extensions.Tests;
using Sharpen.Lang;

namespace Db4oUnit.Extensions.Tests
{
	public class UnhandledExceptionInThreadTestCase : ITestCase
	{
		public class TestCase : AbstractDb4oTestCase
		{
			public virtual void Test()
			{
				Container().ThreadPool().Start(new _IRunnable_13());
			}

			private sealed class _IRunnable_13 : IRunnable
			{
				public _IRunnable_13()
				{
				}

				public void Run()
				{
					throw new InvalidOperationException();
				}
			}
		}

		public virtual void TestSolo()
		{
			Db4oTestSuiteBuilder suite = new Db4oTestSuiteBuilder(new Db4oInMemory(), typeof(
				UnhandledExceptionInThreadTestCase.TestCase));
			TestResult result = new TestResult();
			new TestRunner(suite).Run(result);
			Assert.AreEqual(1, result.Failures.Count);
		}
	}
}
