/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.Tests.Common.Constraints;

namespace Db4objects.Db4o.Tests.Common.Constraints
{
	public class UniqueFieldIndexWithVersionNumbersTestCase : UniqueFieldIndexTestCase
	{
		/// <exception cref="Exception"></exception>
		protected override void Configure(IConfiguration config)
		{
			base.Configure(config);
			config.GenerateVersionNumbers(ConfigScope.Globally);
		}

		public static void Main(string[] args)
		{
			new UniqueFieldIndexWithVersionNumbersTestCase().RunAll();
		}
	}
}
