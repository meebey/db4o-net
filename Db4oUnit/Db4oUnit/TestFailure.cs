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

		public override void Print(TextWriter writer)
		{
			writer.Write(_test.GetLabel());
			writer.Write(": ");
			TestPlatform.PrintStackTrace(writer, _failure);
		}
	}
}
