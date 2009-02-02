/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq.Expressions;
using System.Text;

using Db4objects.Db4o.Linq.Caching;

using Db4oUnit;
using Db4oUnit.Extensions;

namespace Db4objects.Db4o.Linq.Tests.Caching
{
	public class CachingStrategyTestCase : AbstractDb4oTestCase
	{
		public void TestSingleItemStrategy()
		{
			var strategy = new SingleItemCachingStrategy<string, int>();
			Assert.AreEqual(42, strategy.Produce("foo", key => 42));
			Assert.AreEqual(7, strategy.Produce("bar", key => 7));
			Assert.AreEqual(0, strategy.Produce("foo", key => 0));
		}

		public void TestAllItemsStrategy()
		{
			var strategy = new AllItemsCachingStrategy<string, int>();
			for (int delta = 0; delta < 2; ++delta)
			{
				Assert.AreEqual(7, strategy.Produce("bar", key => 7 + delta));
				Assert.AreEqual(12, strategy.Produce("baz", key => 12 + delta));
				Assert.AreEqual(42, strategy.Produce("foo", key => 42 + delta));
			}
		}

		public void TestNullStrategy()
		{
			var strategy = new NullCachingStrategy<string, string>();
			Assert.AreEqual("bar", strategy.Produce("foo", key=>"bar"));
			Assert.AreEqual("baz", strategy.Produce("foo", key =>"baz"));
		}
	}
}
