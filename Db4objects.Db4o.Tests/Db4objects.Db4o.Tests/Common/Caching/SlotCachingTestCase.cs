/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System.Collections;
using Db4oUnit;
using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Tests.Common.Caching;

namespace Db4objects.Db4o.Tests.Common.Caching
{
	public class SlotCachingTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		public class Item
		{
			public Item(int i)
			{
				_id = 1;
			}

			public int _id;
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Cache().SlotCacheSize = 10;
			config.ObjectClass(typeof(SlotCachingTestCase.Item)).ObjectField("_id").Indexed(true
				);
		}

		public virtual void Test()
		{
			Store(new SlotCachingTestCase.Item(1));
			Db().Commit();
			LocalTransaction systemTrans = (LocalTransaction)SystemTrans();
			ICache4 cache = systemTrans.SlotCache();
			Assert.IsNotNull(cache);
			IEnumerator i = cache.GetEnumerator();
		}
		// FIXME: doesn't decaf
		//		Assert.isTrue(i.hasNext());
	}
}
