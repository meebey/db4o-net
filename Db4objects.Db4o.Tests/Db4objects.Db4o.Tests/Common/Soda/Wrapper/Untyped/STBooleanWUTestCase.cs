namespace Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped
{
	public class STBooleanWUTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		internal static readonly string DESCENDANT = "i_boolean";

		public object i_boolean;

		public STBooleanWUTestCase()
		{
		}

		private STBooleanWUTestCase(bool a_boolean)
		{
			i_boolean = a_boolean;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				(false), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				(true), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				(false), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				(false), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				() };
		}

		public virtual void TestEqualsTrue()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				(true));
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				(true));
		}

		public virtual void TestEqualsFalse()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				(false));
			q.Descend(DESCENDANT).Constrain(false);
			Expect(q, new int[] { 0, 2, 3 });
		}

		public virtual void TestNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				());
			q.Descend(DESCENDANT).Constrain(null);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				());
		}

		public virtual void TestNullOrTrue()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				());
			Db4objects.Db4o.Query.IQuery qd = q.Descend(DESCENDANT);
			qd.Constrain(null).Or(qd.Constrain(true));
			Expect(q, new int[] { 1, 4 });
		}

		public virtual void TestNotNullAndFalse()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STBooleanWUTestCase
				());
			Db4objects.Db4o.Query.IQuery qd = q.Descend(DESCENDANT);
			qd.Constrain(null).Not().And(qd.Constrain(false));
			Expect(q, new int[] { 0, 2, 3 });
		}
	}
}
