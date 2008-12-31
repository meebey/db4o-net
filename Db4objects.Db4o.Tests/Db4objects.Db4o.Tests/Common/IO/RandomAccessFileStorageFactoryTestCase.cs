/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.IO;
using Db4objects.Db4o.Tests.Common.Api;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class RandomAccessFileStorageFactoryTestCase : TestWithTempFile
	{
		private readonly FileStorage subject = new FileStorage();

		public virtual void TestExistsWithUnexistentFile()
		{
			Assert.IsFalse(subject.Exists(_tempFile));
		}

		public virtual void TestExistsWithZeroLengthFile()
		{
			IBin storage = subject.Open(new BinConfiguration(_tempFile, false, 0, false));
			storage.Close();
			Assert.IsFalse(subject.Exists(_tempFile));
		}
	}
}
