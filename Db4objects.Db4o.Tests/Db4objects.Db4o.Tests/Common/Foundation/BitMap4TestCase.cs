namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class BitMap4TestCase : Db4oUnit.ITestCase
	{
		public virtual void Test()
		{
			byte[] buffer = new byte[100];
			for (int i = 0; i < 17; i++)
			{
				Db4objects.Db4o.Foundation.BitMap4 map = new Db4objects.Db4o.Foundation.BitMap4(i
					);
				map.WriteTo(buffer, 11);
				Db4objects.Db4o.Foundation.BitMap4 reReadMap = new Db4objects.Db4o.Foundation.BitMap4
					(buffer, 11, i);
				for (int j = 0; j < i; j++)
				{
					TBit(map, j);
					TBit(reReadMap, j);
				}
			}
		}

		private void TBit(Db4objects.Db4o.Foundation.BitMap4 map, int bit)
		{
			map.SetTrue(bit);
			Db4oUnit.Assert.IsTrue(map.IsTrue(bit));
			map.SetFalse(bit);
			Db4oUnit.Assert.IsFalse(map.IsTrue(bit));
			map.SetTrue(bit);
			Db4oUnit.Assert.IsTrue(map.IsTrue(bit));
		}
	}
}
