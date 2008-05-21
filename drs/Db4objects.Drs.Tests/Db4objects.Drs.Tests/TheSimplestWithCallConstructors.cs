/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Drs.Tests;

namespace Db4objects.Drs.Tests
{
	public class TheSimplestWithCallConstructors : TheSimplest
	{
		protected override void Configure(IConfiguration config)
		{
			config.CallConstructors(true);
		}

		protected override SPCChild CreateChildObject(string name)
		{
			return new SPCChildWithoutDefaultConstructor(name);
		}
	}
}
