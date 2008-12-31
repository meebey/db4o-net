/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit.Extensions;
using Db4oUnit.Extensions.Fixtures;
using Db4objects.Db4o.Config;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Exceptions;

namespace Db4objects.Db4o.Tests.Common.Exceptions
{
	public class Db4oIOExceptionTestCaseBase : AbstractDb4oTestCase, IOptOutCS, IOptOutTA
	{
		protected override void Configure(IConfiguration config)
		{
			config.LockDatabaseFile(false);
			config.Storage = new ExceptionSimulatingStorage(new FileStorage());
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Db4oSetupBeforeStore()
		{
			ExceptionSimulatingStorage.exception = false;
		}

		/// <exception cref="System.Exception"></exception>
		protected override void Db4oTearDownBeforeClean()
		{
			ExceptionSimulatingStorage.exception = false;
		}
	}
}
