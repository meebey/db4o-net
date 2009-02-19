/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;

namespace Db4oUnit.Extensions
{
	public class AbstractInMemoryDb4oTestCase : AbstractDb4oTestCase, IOptOutCS
	{
		/// <exception cref="System.Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			config.Storage = new MemoryStorage();
		}
	}
}
