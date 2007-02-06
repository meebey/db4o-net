namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class SimpleTypeArrayInUntypedVariableTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static readonly int[] ARRAY = { 1, 2, 3 };

		public class Data
		{
			public object _arr;

			public Data(object arr)
			{
				this._arr = arr;
			}
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleTypeArrayInUntypedVariableTestCase.Data
				(ARRAY));
		}

		public virtual void TestRetrieval()
		{
			Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleTypeArrayInUntypedVariableTestCase.Data
				 data = (Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleTypeArrayInUntypedVariableTestCase.Data
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleTypeArrayInUntypedVariableTestCase.Data)
				);
			Db4oUnit.Assert.IsTrue(data._arr is int[]);
			int[] arri = (int[])data._arr;
			Db4oUnit.ArrayAssert.AreEqual(ARRAY, arri);
		}
	}
}
