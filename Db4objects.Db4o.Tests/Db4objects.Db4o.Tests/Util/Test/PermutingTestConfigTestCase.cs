namespace Db4objects.Db4o.Tests.Util.Test
{
	public class PermutingTestConfigTestCase : Db4oUnit.ITestCase
	{
		public virtual void TestPermutation()
		{
			object[][] data = new object[][] { new object[] { "A", "B" }, new object[] { "X", 
				"Y", "Z" } };
			Db4objects.Db4o.Tests.Util.PermutingTestConfig config = new Db4objects.Db4o.Tests.Util.PermutingTestConfig
				(data);
			object[][] expected = new object[][] { new object[] { "A", "X" }, new object[] { 
				"A", "Y" }, new object[] { "A", "Z" }, new object[] { "B", "X" }, new object[] { 
				"B", "Y" }, new object[] { "B", "Z" } };
			for (int groupIdx = 0; groupIdx < expected.Length; groupIdx++)
			{
				Db4oUnit.Assert.IsTrue(config.MoveNext());
				object[] current = { config.Current(0), config.Current(1) };
				Db4oUnit.ArrayAssert.AreEqual(expected[groupIdx], current);
			}
			Db4oUnit.Assert.IsFalse(config.MoveNext());
		}
	}
}
