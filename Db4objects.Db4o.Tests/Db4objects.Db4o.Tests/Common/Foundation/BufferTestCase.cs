/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class BufferTestCase : ITestCase
	{
		private const int Readerlength = 64;

		public virtual void TestCopy()
		{
			BufferImpl from = new BufferImpl(Readerlength);
			for (int i = 0; i < Readerlength; i++)
			{
				from.WriteByte((byte)i);
			}
			BufferImpl to = new BufferImpl(Readerlength - 1);
			from.CopyTo(to, 1, 2, 10);
			Assert.AreEqual(0, to.ReadByte());
			Assert.AreEqual(0, to.ReadByte());
			for (int i = 1; i <= 10; i++)
			{
				Assert.AreEqual((byte)i, to.ReadByte());
			}
			for (int i = 12; i < Readerlength - 1; i++)
			{
				Assert.AreEqual(0, to.ReadByte());
			}
		}
	}
}
