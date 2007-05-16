/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;

namespace Db4oUnit.Tests
{
	internal class RunsRed : ITest
	{
		private Exception _exception;

		public RunsRed(Exception exception)
		{
			_exception = exception;
		}

		public virtual string GetLabel()
		{
			return "RunsRed";
		}

		public virtual void Run(TestResult result)
		{
			result.TestFailed(this, _exception);
		}
	}
}
