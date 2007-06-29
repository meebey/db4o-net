/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class CallConstructorsConfigTestCase : StandaloneCSTestCaseBase
	{
		protected override void RunTest()
		{
			WithClient(new _IClientBlock_14(this));
			WithClient(new _IClientBlock_20(this));
		}

		private sealed class _IClientBlock_14 : StandaloneCSTestCaseBase.IClientBlock
		{
			public _IClientBlock_14(CallConstructorsConfigTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run(IObjectContainer client)
			{
				client.Set(new StandaloneCSTestCaseBase.Item());
			}

			private readonly CallConstructorsConfigTestCase _enclosing;
		}

		private sealed class _IClientBlock_20 : StandaloneCSTestCaseBase.IClientBlock
		{
			public _IClientBlock_20(CallConstructorsConfigTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run(IObjectContainer client)
			{
				Assert.AreEqual(1, client.Query(typeof(StandaloneCSTestCaseBase.Item)).Size());
			}

			private readonly CallConstructorsConfigTestCase _enclosing;
		}

		protected override void Configure(IConfiguration config)
		{
			config.CallConstructors(true);
			config.ExceptionsOnNotStorable(true);
		}
	}
}
