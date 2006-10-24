namespace Db4objects.Db4o.Tests.Common.Soda.Arrays
{
	public class STArrStringOTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public class Data
		{
			public object _strArr;

			public Data(object arr)
			{
				_strArr = arr;
			}
		}

		protected override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data
				(null), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data(new 
				object[] { null }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data
				(new object[] { null, null }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data
				(new object[] { "foo", "bar", "fly" }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data
				(new object[] { null, "bar", "wohay", "johy" }) };
		}

		public virtual void TestDefaultContainsOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data
				(new object[] { "bar" }));
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDefaultContainsTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data
				(new object[] { "foo", "bar" }));
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar");
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDescendTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo");
			qElements.Constrain("bar");
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOneNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data)
				);
			q.Descend("_strArr").Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}

		public virtual void TestDescendTwoNot()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrStringOTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_strArr");
			qElements.Constrain("foo").Not();
			qElements.Constrain("bar").Not();
			Expect(q, new int[] { 0, 1, 2 });
		}
	}
}
