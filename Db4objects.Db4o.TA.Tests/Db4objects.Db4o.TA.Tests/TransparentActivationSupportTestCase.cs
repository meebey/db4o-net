/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.TA;

namespace Db4objects.Db4o.TA.Tests
{
	public class TransparentActivationSupportTestCase : AbstractDb4oTestCase
	{
		protected override void Configure(IConfiguration config)
		{
			config.Add(new TransparentActivationSupport());
		}

		public virtual void TestActivationDepth()
		{
			Assert.AreEqual(0, Db().Configure().ActivationDepth());
		}
	}
}
