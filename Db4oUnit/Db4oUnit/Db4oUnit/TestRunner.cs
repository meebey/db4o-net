/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4oUnit;

namespace Db4oUnit
{
	public class TestRunner
	{
		private readonly IEnumerable _tests;

		public TestRunner(IEnumerable tests)
		{
			_tests = tests;
		}

		public virtual void Run(ITestListener listener)
		{
			listener.RunStarted();
			IEnumerator iterator = _tests.GetEnumerator();
			while (iterator.MoveNext())
			{
				ITest test = (ITest)iterator.Current;
				listener.TestStarted(test);
				try
				{
					test.Run();
				}
				catch (TestException x)
				{
					Exception reason = x.GetReason();
					listener.TestFailed(test, reason == null ? x : reason);
				}
				catch (Exception failure)
				{
					listener.TestFailed(test, failure);
				}
			}
			listener.RunFinished();
		}
	}
}
