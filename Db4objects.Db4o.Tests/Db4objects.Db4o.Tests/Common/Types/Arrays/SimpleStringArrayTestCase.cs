namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class SimpleStringArrayTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static readonly string[] ARRAY = new string[] { "hi", "babe" };

		public class Data
		{
			public string[] _arr;

			public Data(string[] _arr)
			{
				this._arr = _arr;
			}
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleStringArrayTestCase.Data
				(ARRAY));
		}

		public virtual void TestRetrieve()
		{
			Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleStringArrayTestCase.Data data = (
				Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleStringArrayTestCase.Data)RetrieveOnlyInstance
				(typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.SimpleStringArrayTestCase.Data)
				);
			Db4oUnit.ArrayAssert.AreEqual(ARRAY, data._arr);
		}
	}
}
