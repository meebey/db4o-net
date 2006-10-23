namespace Db4objects.Db4o.Tests.Common.Types.Arrays
{
	public class ArrayNOrderTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		public class Data
		{
			public string[][][] _strArr;

			public object[][] _objArr;

			public Data(string[][][] strArr, object[][] objArr)
			{
				this._strArr = strArr;
				this._objArr = objArr;
			}
		}

		protected override void Store()
		{
			string[][][] strArr = new string[][][] { new string[][] { new string[3], new string
				[3] }, new string[][] { new string[3], new string[3] } };
			strArr[0][0][0] = "000";
			strArr[0][0][1] = "001";
			strArr[0][0][2] = "002";
			strArr[0][1][0] = "010";
			strArr[0][1][1] = "011";
			strArr[0][1][2] = "012";
			strArr[1][0][0] = "100";
			strArr[1][0][1] = "101";
			strArr[1][0][2] = "102";
			strArr[1][1][0] = "110";
			strArr[1][1][1] = "111";
			strArr[1][1][2] = "112";
			object[][] objArr = new object[][] { new object[2], new object[2] };
			objArr[0][0] = 0;
			objArr[0][1] = "01";
			objArr[1][0] = System.Convert.ToSingle(10);
			objArr[1][1] = 1.1;
			Db().Set(new Db4objects.Db4o.Tests.Common.Types.Arrays.ArrayNOrderTestCase.Data(strArr
				, objArr));
		}

		/// <summary>Ignore this test on .net for now</summary>
		public virtual void _Test()
		{
			Db4objects.Db4o.Tests.Common.Types.Arrays.ArrayNOrderTestCase.Data data = (Db4objects.Db4o.Tests.Common.Types.Arrays.ArrayNOrderTestCase.Data
				)RetrieveOnlyInstance(typeof(Db4objects.Db4o.Tests.Common.Types.Arrays.ArrayNOrderTestCase.Data)
				);
			Check(data);
		}

		public virtual void Check(Db4objects.Db4o.Tests.Common.Types.Arrays.ArrayNOrderTestCase.Data
			 data)
		{
			Db4oUnit.Assert.AreEqual("000", data._strArr[0][0][0]);
			Db4oUnit.Assert.AreEqual("001", data._strArr[0][0][1]);
			Db4oUnit.Assert.AreEqual("002", data._strArr[0][0][2]);
			Db4oUnit.Assert.AreEqual("010", data._strArr[0][1][0]);
			Db4oUnit.Assert.AreEqual("011", data._strArr[0][1][1]);
			Db4oUnit.Assert.AreEqual("012", data._strArr[0][1][2]);
			Db4oUnit.Assert.AreEqual("100", data._strArr[1][0][0]);
			Db4oUnit.Assert.AreEqual("101", data._strArr[1][0][1]);
			Db4oUnit.Assert.AreEqual("102", data._strArr[1][0][2]);
			Db4oUnit.Assert.AreEqual("110", data._strArr[1][1][0]);
			Db4oUnit.Assert.AreEqual("111", data._strArr[1][1][1]);
			Db4oUnit.Assert.AreEqual("112", data._strArr[1][1][2]);
			Db4oUnit.Assert.AreEqual(0, data._objArr[0][0]);
			Db4oUnit.Assert.AreEqual("01", data._objArr[0][1]);
			Db4oUnit.Assert.AreEqual(System.Convert.ToSingle(10), data._objArr[1][0]);
			Db4oUnit.Assert.AreEqual(1.1, data._objArr[1][1]);
		}
	}
}
