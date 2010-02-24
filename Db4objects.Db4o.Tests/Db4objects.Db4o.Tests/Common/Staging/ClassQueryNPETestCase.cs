/* Copyright (C) 2004 - 2009  Versant Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Extensions;
using Db4objects.Db4o;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Staging;

namespace Db4objects.Db4o.Tests.Common.Staging
{
	public class ClassQueryNPETestCase : AbstractDb4oTestCase
	{
		public class Item
		{
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(ClassQueryNPETestCase.Item)).Indexed(false);
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Store()
		{
			Store(new ClassQueryNPETestCase.Item());
		}

		public virtual void Test()
		{
			IObjectSet query = Db().Query(typeof(ClassQueryNPETestCase.Item));
			Assert.AreEqual(0, query.Count);
		}
	}
}
