namespace Db4objects.Db4o.Tests.Common.Soda.Arrays
{
	public class STArrMixedTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public class Data
		{
			public object[] _arr;

			public Data(object[] arr)
			{
				this._arr = arr;
			}
		}

		protected override object[] CreateData()
		{
			object[] arr = new object[] { new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data
				(null), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data(new 
				object[0]), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data
				(new object[] { 0, 0, "foo", false }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data
				(new object[] { 1, 17, int.MaxValue - 1, "foo", "bar" }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data
				(new object[] { 3, 17, 25, int.MaxValue - 2 }) };
			return arr;
		}

		public virtual void TestDefaultContainsInteger()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data(
				new object[] { 17 }));
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDefaultContainsString()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data(
				new object[] { "foo" }));
			Expect(q, new int[] { 2, 3 });
		}

		public virtual void TestDefaultContainsBoolean()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data(
				new object[] { false }));
			Expect(q, new int[] { 2 });
		}

		public virtual void TestDefaultContainsTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data(
				new object[] { 17, "bar" }));
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data)
				);
			q.Descend("_arr").Constrain(17);
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDescendTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_arr");
			qElements.Constrain(17);
			qElements.Constrain("bar");
			Expect(q, new int[] { 3 });
		}

		public virtual void TestDescendSmaller()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.STArrMixedTestCase.Data)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("_arr");
			qElements.Constrain(3).Smaller();
			Expect(q, new int[] { 2, 3 });
		}
	}
}
