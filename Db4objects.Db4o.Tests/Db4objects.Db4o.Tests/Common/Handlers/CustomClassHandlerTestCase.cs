/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Handlers;

namespace Db4objects.Db4o.Tests.Common.Handlers
{
	public class CustomClassHandlerTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		internal static bool handlerCalled;

		public static void Main(string[] arguments)
		{
			new CustomClassHandlerTestCase().RunSolo();
		}

		public class Item
		{
		}

		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.ObjectClass(typeof(CustomClassHandlerTestCase.Item)).InstallCustomHandler(
				new _VanillaClassHandler_26(this));
		}

		private sealed class _VanillaClassHandler_26 : VanillaClassHandler
		{
			public _VanillaClassHandler_26(CustomClassHandlerTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public override bool CanNewInstance()
			{
				CustomClassHandlerTestCase.handlerCalled = true;
				return base.CanNewInstance();
			}

			private readonly CustomClassHandlerTestCase _enclosing;
		}

		protected override void Store()
		{
			Store(new CustomClassHandlerTestCase.Item());
		}

		public virtual void Test()
		{
			handlerCalled = false;
			RetrieveOnlyInstance(typeof(CustomClassHandlerTestCase.Item));
			Assert.IsTrue(handlerCalled);
		}
	}
}
