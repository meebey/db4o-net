/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>For test cases that need setUp/tearDown support.</summary>
	/// <remarks>For test cases that need setUp/tearDown support.</remarks>
	public interface ITestLifeCycle : ITestCase
	{
		/// <exception cref="Exception"></exception>
		void SetUp();

		/// <exception cref="Exception"></exception>
		void TearDown();
	}
}
