/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.IO;

namespace Db4objects.Db4o.Tests.Common.IO
{
	public class MemoryIoAdapterTestCase : ITestCase
	{
		private static readonly string Url = "url";

		private const int GrowBy = 100;

		public virtual void TestGrowth()
		{
			MemoryIoAdapter factory = new MemoryIoAdapter();
			factory.GrowBy(GrowBy);
			IoAdapter io = factory.Open(Url, false, 0, false);
			AssertLength(factory, 0);
			WriteBytes(io, GrowBy - 1);
			AssertLength(factory, GrowBy);
			WriteBytes(io, GrowBy - 1);
			AssertLength(factory, GrowBy * 2);
			WriteBytes(io, GrowBy * 2);
			AssertLength(factory, GrowBy * 4 - 2);
		}

		private void WriteBytes(IoAdapter io, int numBytes)
		{
			io.Write(new byte[numBytes]);
		}

		private void AssertLength(MemoryIoAdapter factory, int expected)
		{
			Assert.AreEqual(expected, factory.Get(Url).Length);
		}
	}
}
