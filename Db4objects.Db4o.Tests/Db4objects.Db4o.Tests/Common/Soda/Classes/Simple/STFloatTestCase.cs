namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Simple
{
	public class STFloatTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public float i_float;

		public STFloatTestCase()
		{
		}

		private STFloatTestCase(float a_float)
		{
			i_float = a_float;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STFloatTestCase
				(float.MinValue), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STFloatTestCase
				((float)0.0000123), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STFloatTestCase
				((float)1.345), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STFloatTestCase
				(float.MaxValue) };
		}

		public virtual void TestEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(_array[0]);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestGreater()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STFloatTestCase(
				(float)0.1));
			q.Descend("i_float").Constraints().Greater();
			Expect(q, new int[] { 2, 3 });
		}

		public virtual void TestSmaller()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STFloatTestCase(
				(float)1.5));
			q.Descend("i_float").Constraints().Smaller();
			Expect(q, new int[] { 0, 1, 2 });
		}
	}
}
