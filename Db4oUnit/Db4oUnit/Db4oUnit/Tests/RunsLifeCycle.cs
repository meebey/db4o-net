/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Tests;

namespace Db4oUnit.Tests
{
	public class RunsLifeCycle : ITestCase, ITestLifeCycle
	{
		private bool _setupCalled = false;

		private bool _tearDownCalled = false;

		public virtual void SetUp()
		{
			_setupCalled = true;
		}

		public virtual void TearDown()
		{
			_tearDownCalled = true;
		}

		public virtual bool SetupCalled()
		{
			return _setupCalled;
		}

		public virtual bool TearDownCalled()
		{
			return _tearDownCalled;
		}

		/// <exception cref="Exception"></exception>
		public virtual void TestMethod()
		{
			Assert.IsTrue(_setupCalled);
			Assert.IsTrue(!_tearDownCalled);
			throw FrameworkTestCase.Exception;
		}
	}
}
