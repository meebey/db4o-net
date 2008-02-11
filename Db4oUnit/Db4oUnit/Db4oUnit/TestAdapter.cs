/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>
	/// Implements the
	/// <see cref="ITest">ITest</see>
	/// protocol.
	/// </summary>
	public abstract class TestAdapter : ITest
	{
		public TestAdapter() : base()
		{
		}

		/// <exception cref="Exception"></exception>
		protected abstract void RunTest();

		protected virtual void SetUp()
		{
		}

		protected virtual void TearDown()
		{
		}

		public virtual void Run(TestResult result)
		{
			try
			{
				result.TestStarted(this);
				SetUp();
				RunTest();
			}
			catch (TargetInvocationException e)
			{
				result.TestFailed(this, e.InnerException);
			}
			catch (Exception e)
			{
				result.TestFailed(this, e);
			}
			finally
			{
				try
				{
					TearDown();
				}
				catch (TestException e)
				{
					result.TestFailed(this, e);
				}
			}
		}

		public abstract string GetLabel();
	}
}
