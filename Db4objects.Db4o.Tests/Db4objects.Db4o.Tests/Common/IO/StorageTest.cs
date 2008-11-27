/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4oUnit.Fixtures;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class StorageTest : TestWithTempFile
	{
		public virtual void TestInitialLength()
		{
			Storage().Open(_tempFile, false, 1000, false).Close();
			IBin bin = Storage().Open(_tempFile, false, 0, false);
			try
			{
				Assert.AreEqual(1000, bin.Length());
			}
			finally
			{
				bin.Close();
			}
		}

		private IStorage Storage()
		{
			return ((IStorage)SubjectFixtureProvider.Value());
		}
	}
}
