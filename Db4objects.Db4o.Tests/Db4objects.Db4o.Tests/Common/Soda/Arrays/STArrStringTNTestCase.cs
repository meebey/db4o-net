namespace Db4objects.Db4o.Tests.Common.Soda.Arrays
{
	public class STArrStringTNTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public class Data
		{
			public string[][][] _strArr;

			public Data(string[][][] arr)
			{
				_strArr = arr;
			}
		}

		protected override object[] CreateData()
		{
			Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data[] arr = new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data
				[5];
			arr[0] = new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data(
				null);
			string[][][] content = new string[][][] { new string[][] { new string[2] } };
			arr[1] = new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data(
				content);
			content = new string[][][] { new string[][] { new string[3], new string[3] } };
			arr[2] = new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data(
				content);
			content = new string[][][] { new string[][] { new string[3], new string[3] } };
			content[0][0][1] = "foo";
			content[0][1][0] = "bar";
			content[0][1][2] = "fly";
			arr[3] = new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data(
				content);
			content = new string[][][] { new string[][] { new string[3], new string[3] } };
			content[0][0][0] = "bar";
			content[0][1][0] = "wohay";
			content[0][1][1] = "johy";
			arr[4] = new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data(
				content);
			object[] ret = new object[arr.Length];
			System.Array.Copy(arr, 0, ret, 0, arr.Length);
			return ret;
		}

		public virtual void TestDefaultContainsOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			string[][][] content = new string[][][] { new string[][] { new string[1] } };
			content[0][0][0] = "bar";
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data
				(content));
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDefaultContainsTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			string[][][] content = new string[][][] { new string[][] { new string[1] }, new string[]
				[] { new string[1] } };
			content[0][0][0] = "bar";
			content[1][0][0] = "foo";
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data
				(content));
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar");
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDescendTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo");
			qElements.Constrain("bar");
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOneNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}

		public virtual void TestDescendTwoNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTNTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo").Not();
			qElements.Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}
	}
}
