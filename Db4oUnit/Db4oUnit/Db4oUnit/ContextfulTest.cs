/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;

namespace Db4oUnit
{
	public class ContextfulTest : Contextful, ITestDecoration
	{
		private readonly ITestFactory _factory;

		private ITest _test;

		public ContextfulTest(ITestFactory factory)
		{
			_factory = factory;
		}

		public virtual string Label()
		{
			return (string)Run(new _IClosure4_19(this));
		}

		private sealed class _IClosure4_19 : IClosure4
		{
			public _IClosure4_19(ContextfulTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public object Run()
			{
				return this._enclosing.TestInstance().Label();
			}

			private readonly ContextfulTest _enclosing;
		}

		public virtual void Run()
		{
			Run(TestInstance());
		}

		public virtual ITest Test()
		{
			if (null == _test)
			{
				throw new InvalidOperationException();
			}
			return _test;
		}

		private ITest TestInstance()
		{
			if (_test == null)
			{
				_test = _factory.NewInstance();
			}
			return _test;
		}
	}
}
