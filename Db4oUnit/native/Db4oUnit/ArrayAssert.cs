namespace Db4oUnit
{
	public partial class ArrayAssert
	{
		public static void AreEqual<T>(T[] expected, T[] actual)
		{
			Iterator4Assert.AreEqual(expected.GetEnumerator(), actual.GetEnumerator());
		}
	}
}
