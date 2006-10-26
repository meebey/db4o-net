namespace Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped
{
	public class STByteWUTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		internal static readonly string DESCENDANT = "i_byte";

		public object i_byte;

		public STByteWUTestCase()
		{
		}

		private STByteWUTestCase(byte a_byte)
		{
			i_byte = a_byte;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)0), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)1), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)99), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)113) };
		}

		public virtual void TestEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)0));
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestNotEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(_array[0]);
			q.Descend(DESCENDANT).Constraints().Not();
			Expect(q, new int[] { 1, 2, 3 });
		}

		public virtual void TestGreater()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)9));
			q.Descend(DESCENDANT).Constraints().Greater();
			Expect(q, new int[] { 2, 3 });
		}

		public virtual void TestSmaller()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)1));
			q.Descend(DESCENDANT).Constraints().Smaller();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)9));
			q.Descend(DESCENDANT).Constraints().Contains();
			Expect(q, new int[] { 2 });
		}

		public virtual void TestNotContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)0));
			q.Descend(DESCENDANT).Constraints().Contains().Not();
			Expect(q, new int[] { 1, 2, 3 });
		}

		public virtual void TestLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)11));
			q.Descend(DESCENDANT).Constraints().Like();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)113));
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)10));
			q.Descend(DESCENDANT).Constraints().Like();
			Expect(q, new int[] {  });
		}

		public virtual void TestNotLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)1));
			q.Descend(DESCENDANT).Constraints().Like().Not();
			Expect(q, new int[] { 0, 2 });
		}

		public virtual void TestIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)1));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase)set.Next(
				);
			identityConstraint.i_byte = (byte)102;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity();
			identityConstraint.i_byte = (byte)1;
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[1]);
		}

		public virtual void TestNotIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)1));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase)set.Next(
				);
			identityConstraint.i_byte = (byte)102;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity().Not();
			identityConstraint.i_byte = (byte)1;
			Expect(q, new int[] { 0, 2, 3 });
		}

		public virtual void TestConstraints()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)1));
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				((byte)0));
			Db4objects.Db4o.Query.IConstraints cs = q.Constraints();
			Db4objects.Db4o.Query.IConstraint[] csa = cs.ToArray();
			if (csa.Length != 2)
			{
				Db4oUnit.Assert.Fail("Constraints not returned");
			}
		}

		public virtual void TestNull()
		{
		}

		public virtual void TestEvaluation()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
				());
			q.Constrain(new _AnonymousInnerClass137(this));
			Expect(q, new int[] { 2, 3 });
		}

		private sealed class _AnonymousInnerClass137 : Db4objects.Db4o.Query.IEvaluation
		{
			public _AnonymousInnerClass137(STByteWUTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Evaluate(Db4objects.Db4o.Query.ICandidate candidate)
			{
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase sts = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STByteWUTestCase
					)candidate.GetObject();
				candidate.Include((((byte)sts.i_byte) + 2) > 100);
			}

			private readonly STByteWUTestCase _enclosing;
		}
	}
}
