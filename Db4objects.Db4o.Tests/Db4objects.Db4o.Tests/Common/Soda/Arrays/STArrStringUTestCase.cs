namespace Db4objects.Db4o.Tests.Common.Soda.Arrays
{
	public class STArrStringUTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public class Data
		{
			public object[] _strArr;

			public Data(STArrStringUTestCase _enclosing, object[] strArr)
			{
				this._enclosing = _enclosing;
				this._strArr = strArr;
			}

			private readonly STArrStringUTestCase _enclosing;
		}

		protected override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data
				(this, null), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data
				(this, new object[] { null }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data
				(this, new object[] { null, null }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data
				(this, new object[] { "foo", "bar", "fly" }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data
				(this, new object[] { null, "bar", "wohay", "johy" }) };
		}

		public virtual void TestDefaultContainsOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data
				(this, new object[] { "bar" }));
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDefaultContainsTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data
				(this, new object[] { "foo", "bar" }));
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar");
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDescendTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo");
			qElements.Constrain("bar");
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOneNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}

		public virtual void TestDescendTwoNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringUTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo").Not();
			qElements.Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}
	}
}
