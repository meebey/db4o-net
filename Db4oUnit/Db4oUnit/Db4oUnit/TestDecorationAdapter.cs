/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	public class TestDecorationAdapter : ITest
	{
		private readonly ITest _test;

		public TestDecorationAdapter(ITest test)
		{
			_test = test;
		}

		public virtual string Label()
		{
			return _test.Label();
		}

		public virtual void Run()
		{
			_test.Run();
		}
	}
}
