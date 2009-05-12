/* Copyright (C) 2009 Versant Corporation.   http://www.db4o.com */
using System.Diagnostics;
using Db4oUnit;

namespace Db4oTool.Tests.TA
{
	abstract class TAOutputListenerTestCaseBase : TATestCaseBase
	{
		protected void InstrumentAndAssert(bool shouldContain, string assemblyPath, params string[] expectedMessages)
		{
			TraceListener listener = new TraceListener();
			Trace.Listeners.Add(listener);

			Db4oTool.Program.Main(new string[] { "-ta", assemblyPath });

			Trace.Listeners.Remove(listener);
			foreach (string message in expectedMessages)
			{
				Assert.AreEqual(shouldContain, listener.Contents.Contains(message));	
			}
		}
	}
}
