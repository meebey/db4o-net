namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class YapReaderTestCase : Db4oUnit.ITestCase
	{
		private const int READERLENGTH = 64;

		public virtual void TestCopy()
		{
			Db4objects.Db4o.YapReader from = new Db4objects.Db4o.YapReader(READERLENGTH);
			for (int i = 0; i < READERLENGTH; i++)
			{
				from.Append((byte)i);
			}
			Db4objects.Db4o.YapReader to = new Db4objects.Db4o.YapReader(READERLENGTH - 1);
			from.CopyTo(to, 1, 2, 10);
			Db4oUnit.Assert.AreEqual(0, to.ReadByte());
			Db4oUnit.Assert.AreEqual(0, to.ReadByte());
			for (int i = 1; i <= 10; i++)
			{
				Db4oUnit.Assert.AreEqual((byte)i, to.ReadByte());
			}
			for (int i = 12; i < READERLENGTH - 1; i++)
			{
				Db4oUnit.Assert.AreEqual(0, to.ReadByte());
			}
		}
	}
}
