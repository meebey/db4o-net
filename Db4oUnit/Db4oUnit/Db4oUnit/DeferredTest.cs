/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	public class DeferredTest : ITestDecoration
	{
		private readonly ITestFactory _factory;

		private ITest _test;

		public DeferredTest(ITestFactory factory)
		{
			_factory = factory;
		}

		public virtual string GetLabel()
		{
			return Test().GetLabel();
		}

		public virtual void Run()
		{
			Test().Run();
		}

		public virtual ITest Test()
		{
			if (_test == null)
			{
				_test = _factory.NewInstance();
			}
			return _test;
		}
	}
}
