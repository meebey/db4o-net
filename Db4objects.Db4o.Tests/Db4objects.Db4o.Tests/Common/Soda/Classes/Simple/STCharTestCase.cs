namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Simple
{
	public class STCharTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		internal static readonly string DESCENDANT = "i_char";

		public char i_char;

		public STCharTestCase()
		{
		}

		private STCharTestCase(char a_char)
		{
			i_char = a_char;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase
				((char)0), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)1), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((char
				)99), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((char)
				909) };
		}

		public virtual void TestEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)0));
			q.Descend(DESCENDANT).Constrain((char)0);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestNotEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(_array[0]);
			q.Descend(DESCENDANT).Constrain((char)0).Not();
			Expect(q, new int[] { 1, 2, 3 });
		}

		public virtual void TestGreater()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)9));
			q.Descend(DESCENDANT).Constraints().Greater();
			Expect(q, new int[] { 2, 3 });
		}

		public virtual void TestSmaller()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)1));
			q.Descend(DESCENDANT).Constraints().Smaller();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)1));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase)set.Next();
			identityConstraint.i_char = (char)9999;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity();
			identityConstraint.i_char = (char)1;
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[1]);
		}

		public virtual void TestNotIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)1));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase)set.Next();
			identityConstraint.i_char = (char)9080;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity().Not();
			identityConstraint.i_char = (char)1;
			Expect(q, new int[] { 0, 2, 3 });
		}

		public virtual void TestConstraints()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)1));
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STCharTestCase((
				char)0));
			Db4objects.Db4o.Query.IConstraints cs = q.Constraints();
			Db4objects.Db4o.Query.IConstraint[] csa = cs.ToArray();
			if (csa.Length != 2)
			{
				Db4oUnit.Assert.Fail("Constraints not returned");
			}
		}
	}
}
