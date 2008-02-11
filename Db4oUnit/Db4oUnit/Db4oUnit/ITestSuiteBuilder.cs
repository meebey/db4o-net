/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>Custom test suite builder interface.</summary>
	/// <remarks>Custom test suite builder interface.</remarks>
	public interface ITestSuiteBuilder
	{
		TestSuite Build();
	}
}
