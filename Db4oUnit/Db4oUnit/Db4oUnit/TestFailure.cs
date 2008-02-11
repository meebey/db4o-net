/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;
using Db4oUnit;

namespace Db4oUnit
{
	public class TestFailure : Printable
	{
		internal ITest _test;

		internal Exception _failure;

		public TestFailure(ITest test, Exception failure)
		{
			_test = test;
			_failure = failure;
		}

		public virtual ITest GetTest()
		{
			return _test;
		}

		public virtual Exception GetFailure()
		{
			return _failure;
		}

		/// <exception cref="IOException"></exception>
		public override void Print(TextWriter writer)
		{
			writer.Write(_test.GetLabel());
			writer.Write(": ");
			// TODO: don't print the first stack trace elements
			// which reference db4ounit.Assert methods
			TestPlatform.PrintStackTrace(writer, _failure);
		}
	}
}
