namespace Db4objects.Db4o.Tests.Common.Soda.Arrays
{
	public class STArrStringTTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public class Data
		{
			public string[] _strArr;

			public Data(STArrStringTTestCase _enclosing, string[] arr)
			{
				this._enclosing = _enclosing;
				this._strArr = arr;
			}

			private readonly STArrStringTTestCase _enclosing;
		}

		protected override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data
				(this, null), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data
				(this, new string[] { null }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data
				(this, new string[] { null, null }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data
				(this, new string[] { "foo", "bar", "fly" }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data
				(this, new string[] { null, "bar", "wohay", "johy" }) };
		}

		public virtual void TestDefaultContainsOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data
				(this, new string[] { "bar" }));
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDefaultContainsTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data
				(this, new string[] { "foo", "bar" }));
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar");
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDescendTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo");
			qElements.Constrain("bar");
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOneNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}

		public virtual void TestDescendTwoNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringTTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo").Not();
			qElements.Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}
	}
}
