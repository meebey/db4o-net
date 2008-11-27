/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.Foundation;
using Sharpen.Lang;

namespace Db4oUnit
{
	public class ContextfulTest : Contextful, ITest
	{
		private readonly ITestFactory _factory;

		public ContextfulTest(ITestFactory factory)
		{
			_factory = factory;
		}

		public virtual string Label()
		{
			return (string)Run(new _IClosure4_18(this));
		}

		private sealed class _IClosure4_18 : IClosure4
		{
			public _IClosure4_18(ContextfulTest _enclosing)
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
			Run(new _IRunnable_26(this));
		}

		private sealed class _IRunnable_26 : IRunnable
		{
			public _IRunnable_26(ContextfulTest _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run()
			{
				this._enclosing.TestInstance().Run();
			}

			private readonly ContextfulTest _enclosing;
		}

		private ITest TestInstance()
		{
			return _factory.NewInstance();
		}
	}
}
