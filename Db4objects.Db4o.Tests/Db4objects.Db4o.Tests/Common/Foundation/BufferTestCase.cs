using Db4oUnit;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class BufferTestCase : ITestCase
	{
		private const int READERLENGTH = 64;

		public virtual void TestCopy()
		{
			Db4objects.Db4o.Internal.Buffer from = new Db4objects.Db4o.Internal.Buffer(READERLENGTH
				);
			for (int i = 0; i < READERLENGTH; i++)
			{
				from.Append((byte)i);
			}
			Db4objects.Db4o.Internal.Buffer to = new Db4objects.Db4o.Internal.Buffer(READERLENGTH
				 - 1);
			from.CopyTo(to, 1, 2, 10);
			Assert.AreEqual(0, to.ReadByte());
			Assert.AreEqual(0, to.ReadByte());
			for (int i = 1; i <= 10; i++)
			{
				Assert.AreEqual((byte)i, to.ReadByte());
			}
			for (int i = 12; i < READERLENGTH - 1; i++)
			{
				Assert.AreEqual(0, to.ReadByte());
			}
		}
	}
}
