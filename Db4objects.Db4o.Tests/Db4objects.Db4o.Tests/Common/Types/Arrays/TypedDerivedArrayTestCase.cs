namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class TypedDerivedArrayTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private static readonly Db4objects.Db4o.Tests.Common.Sampledata.MoleculeData[] ARRAY
			 = { new Db4objects.Db4o.Tests.Common.Sampledata.MoleculeData("TypedDerivedArray"
			) };

		public class Data
		{
			public Db4objects.Db4o.Tests.Common.Sampledata.AtomData[] _array;

			public Data(Db4objects.Db4o.Tests.Common.Sampledata.AtomData[] AtomDatas)
			{
				this._array = AtomDatas;
			}
		}

		protected override void Store()
		{
			Db().Set(new Db4objects.Db4o.Tests.Common.Types.Arrays.TypedDerivedArrayTestCase.Data
				(ARRAY));
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Tests.Common.Types.Arrays.TypedDerivedArrayTestCase.Data data = (
				Db4objects.Db4o.Tests.Common.Types.Arrays.TypedDerivedArrayTestCase.Data)RetrieveOnlyInstance
				(typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.TypedDerivedArrayTestCase.Data)
				);
			Db4oUnit.Assert.IsTrue(data._array is Db4objects.Db4o.Tests.Common.Sampledata.MoleculeData[]
				, "Expected instance of " + typeof(Db4objects.Db4o.Tests.Common.Sampledata.MoleculeData[])
				 + ", but got " + data._array);
			Db4oUnit.ArrayAssert.AreEqual(ARRAY, data._array);
		}
	}
}
