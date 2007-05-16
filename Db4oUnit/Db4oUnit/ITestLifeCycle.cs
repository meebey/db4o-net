/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>For test cases that need setUp/tearDown support.</summary>
	/// <remarks>For test cases that need setUp/tearDown support.</remarks>
	public interface ITestLifeCycle : ITestCase
	{
		void SetUp();

		void TearDown();
	}
}
