namespace Db4objects.Db4o.Tests.Common.Soda.Ordered
{
	public class STOIntegerWTTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public int i_int;

		public STOIntegerWTTestCase()
		{
		}

		private STOIntegerWTTestCase(int a_int)
		{
			i_int = a_int;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase
				(1001), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase(99), 
				new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase(1), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase
				(909), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase(1001), 
				new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase(0), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase
				(1010), new Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase() };
		}

		public virtual void TestDescending()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase)
				);
			q.Descend("i_int").OrderDescending();
			ExpectOrdered(q, new int[] { 6, 4, 0, 3, 1, 2, 5, 7 });
		}

		public virtual void TestAscendingGreater()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Ordered.STOIntegerWTTestCase)
				);
			Db4objects.Db4o.Query.IQuery qInt = q.Descend("i_int");
			qInt.Constrain(100).Greater();
			qInt.OrderAscending();
			ExpectOrdered(q, new int[] { 3, 0, 4, 6 });
		}
	}
}
