/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4oUnit.Extensions;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Assorted;

namespace Db4objects.Db4o.Tests.Common.Assorted
{
	public class GetByUUIDTestCase : AbstractDb4oTestCase
	{
		public static void Main(string[] args)
		{
			new GetByUUIDTestCase().RunSolo();
		}

		protected override void Configure(IConfiguration config)
		{
			config.ObjectClass(typeof(UUIDTestItem)).GenerateUUIDs(true);
		}

		protected override void Store()
		{
			Db().Store(new UUIDTestItem("one"));
			Db().Store(new UUIDTestItem("two"));
		}

		/// <exception cref="Exception"></exception>
		public virtual void Test()
		{
			Hashtable4 uuidCache = new Hashtable4();
			AssertItemsCanBeRetrievedByUUID(uuidCache);
			Reopen();
			AssertItemsCanBeRetrievedByUUID(uuidCache);
		}

		private void AssertItemsCanBeRetrievedByUUID(Hashtable4 uuidCache)
		{
			UUIDTestItem.AssertItemsCanBeRetrievedByUUID(Db(), uuidCache);
		}
	}
}
