namespace Db4objects.Db4o.Tests.Common.Reflect
{
	public class ReflectArrayTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public virtual void TestNewInstance()
		{
			string[][] a23 = (string[][])NewInstance(typeof(string), new int[] { 2, 3 });
			Db4oUnit.Assert.AreEqual(2, a23.Length);
			for (int i = 0; i < a23.Length; ++i)
			{
				Db4oUnit.Assert.AreEqual(3, a23[i].Length);
			}
		}

		private object NewInstance(System.Type elementType, int[] dimensions)
		{
			return Reflector().Array().NewInstance(Reflector().ForClass(elementType), dimensions
				);
		}
	}
}
