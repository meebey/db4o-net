/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;

namespace Db4oUnit
{
	public interface ITestListener
	{
		void RunStarted();

		void TestStarted(ITest test);

		void TestFailed(ITest test, Exception failure);

		void RunFinished();
	}
}
