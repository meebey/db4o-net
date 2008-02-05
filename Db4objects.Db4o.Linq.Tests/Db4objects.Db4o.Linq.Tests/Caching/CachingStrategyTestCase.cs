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
			strategy.Add("foo", 42);
			Assert.AreEqual(42, strategy.Get("foo"));
			strategy.Add("bar", 7);
			Assert.AreEqual(0, strategy.Get("foo"));
			Assert.AreEqual(7, strategy.Get("bar"));
		}

		public void TestAllItemsStrategy()
		{
			var strategy = new AllItemsCachingStrategy<string, int>();
			strategy.Add("foo", 42);
			strategy.Add("bar", 7);
			strategy.Add("baz", 12);

			Assert.AreEqual(7, strategy.Get("bar"));
			Assert.AreEqual(12, strategy.Get("baz"));
			Assert.AreEqual(42, strategy.Get("foo"));
		}

		public void TestNullStrategy()
		{
			var strategy = new NullCachingStrategy();
			strategy.Add("foo", 42);
			strategy.Add("bar", 7);

			Assert.AreEqual(null, strategy.Get("foo"));
			Assert.AreEqual(null, strategy.Get("bar"));
		}
	}
}
