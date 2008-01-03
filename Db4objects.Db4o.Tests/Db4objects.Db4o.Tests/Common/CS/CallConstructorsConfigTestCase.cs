/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.CS;

namespace Db4objects.Db4o.Tests.Common.CS
{
	public class CallConstructorsConfigTestCase : StandaloneCSTestCaseBase
	{
		/// <exception cref="Exception"></exception>
		protected override void RunTest()
		{
			WithClient(new _IContainerBlock_15(this));
			WithClient(new _IContainerBlock_21(this));
		}

		private sealed class _IContainerBlock_15 : IContainerBlock
		{
			public _IContainerBlock_15(CallConstructorsConfigTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Run(IObjectContainer client)
			{
				client.Store(new StandaloneCSTestCaseBase.Item());
			}

			private readonly CallConstructorsConfigTestCase _enclosing;
		}

		private sealed class _IContainerBlock_21 : IContainerBlock
		{
			public _IContainerBlock_21(CallConstructorsConfigTestCase _enclosing)
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
