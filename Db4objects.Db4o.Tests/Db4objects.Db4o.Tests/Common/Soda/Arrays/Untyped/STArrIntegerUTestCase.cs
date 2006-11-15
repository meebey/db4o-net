namespace Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped
{
	public class STArrIntegerUTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public object[] intArr;

		public STArrIntegerUTestCase()
		{
		}

		public STArrIntegerUTestCase(object[] arr)
		{
			intArr = arr;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase
				(), new Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase(new 
				object[0]), new Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase
				(new object[] { 0, 0 }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase
				(new object[] { 1, 17, int.MaxValue - 1 }), new Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase
				(new object[] { 3, 17, 25, int.MaxValue - 2 }) };
		}

		public virtual void TestDefaultContainsOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase
				(new object[] { 17 }));
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDefaultContainsTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase
				(new object[] { 17, 25 }));
			Expect(q, new int[] { 4 });
		}

		public virtual void TestDescendOne()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase)
				);
			q.Descend("intArr").Constrain(17);
			Expect(q, new int[] { 3, 4 });
		}

		public virtual void TestDescendTwo()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("intArr");
			qElements.Constrain(17);
			qElements.Constrain(25);
			Expect(q, new int[] { 4 });
		}

		public virtual void TestDescendSmaller()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Arrays.Untyped.STArrIntegerUTestCase)
				);
			Db4objects.Db4o.Query.IQuery qElements = q.Descend("intArr");
			qElements.Constrain(3).Smaller();
			Expect(q, new int[] { 2, 3 });
		}
	}
}
