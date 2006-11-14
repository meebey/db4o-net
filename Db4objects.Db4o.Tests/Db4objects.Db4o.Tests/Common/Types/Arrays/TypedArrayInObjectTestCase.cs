namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class TypedArrayInObjectTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static readonly Db4objects.Db4o.Tests.Common.Sampledata.AtomData[] ARRAY = 
			{ new Db4objects.Db4o.Tests.Common.Sampledata.AtomData("TypedArrayInObject") };

		public class Data
		{
			public object _obj;

			public object[] _objArr;

			public Data(object obj, object[] obj2)
			{
				this._obj = obj;
				this._objArr = obj2;
			}
		}

		protected override void Store()
		{
			Db4objects.Db4o.Tests.Common.Types.Arrays.TypedArrayInObjectTestCase.Data data = 
				new Db4objects.Db4o.Tests.Common.Types.Arrays.TypedArrayInObjectTestCase.Data(ARRAY
				, ARRAY);
			Db().Set(data);
		}

		public virtual void TestRetrieve()
		{
			Db4objects.Db4o.Tests.Common.Types.Arrays.TypedArrayInObjectTestCase.Data data = 
				(Db4objects.Db4o.Tests.Common.Types.Arrays.TypedArrayInObjectTestCase.Data)RetrieveOnlyInstance
				(typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.TypedArrayInObjectTestCase.Data)
				);
			Db4oUnit.Assert.IsTrue(data._obj is Db4objects.Db4o.Tests.Common.Sampledata.AtomData[]
				, "Expected instance of " + typeof(Db4objects.Db4o.Tests.Common.Sampledata.AtomData[])
				 + ", but got " + data._obj);
			Db4oUnit.Assert.IsTrue(data._objArr is Db4objects.Db4o.Tests.Common.Sampledata.AtomData[]
				, "Expected instance of " + typeof(Db4objects.Db4o.Tests.Common.Sampledata.AtomData[])
				 + ", but got " + data._objArr);
			Db4oUnit.ArrayAssert.AreEqual(ARRAY, data._objArr);
			Db4oUnit.ArrayAssert.AreEqual(ARRAY, (Db4objects.Db4o.Tests.Common.Sampledata.AtomData[]
				)data._obj);
		}
	}
}
