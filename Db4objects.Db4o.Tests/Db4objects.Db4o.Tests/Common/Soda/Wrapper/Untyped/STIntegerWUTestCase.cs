namespace Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped
{
	[System.Serializable]
	public class STIntegerWUTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public object i_int;

		public STIntegerWUTestCase()
		{
		}

		private STIntegerWUTestCase(int a_int)
		{
			i_int = a_int;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(0), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase(1
				), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase(99)
				, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase(909)
				 };
		}

		public virtual void TestEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(0));
			q.Descend("i_int").Constrain(0);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestNotEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				());
			q.Descend("i_int").Constrain(0).Not();
			Expect(q, new int[] { 1, 2, 3 });
		}

		public virtual void TestGreater()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(9));
			q.Descend("i_int").Constraints().Greater();
			Expect(q, new int[] { 2, 3 });
		}

		public virtual void TestSmaller()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(1));
			q.Descend("i_int").Constraints().Smaller();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[0]);
		}

		public virtual void TestContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(9));
			q.Descend("i_int").Constraints().Contains();
			Expect(q, new int[] { 2, 3 });
		}

		public virtual void TestNotContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				());
			q.Descend("i_int").Constrain(0).Contains().Not();
			Expect(q, new int[] { 1, 2 });
		}

		public virtual void TestLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(90));
			q.Descend("i_int").Constraints().Like();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(909));
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(10));
			q.Descend("i_int").Constraints().Like();
			Expect(q, new int[] {  });
		}

		public virtual void TestNotLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(1));
			q.Descend("i_int").Constraints().Like().Not();
			Expect(q, new int[] { 0, 2, 3 });
		}

		public virtual void TestIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(1));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase)set.Next
				();
			identityConstraint.i_int = 9999;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity();
			identityConstraint.i_int = 1;
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[1]);
		}

		public virtual void TestNotIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(1));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase)set.Next
				();
			identityConstraint.i_int = 9080;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity().Not();
			identityConstraint.i_int = 1;
			Expect(q, new int[] { 0, 2, 3 });
		}

		public virtual void TestConstraints()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(1));
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				(0));
			Db4objects.Db4o.Query.IConstraints cs = q.Constraints();
			Db4objects.Db4o.Query.IConstraint[] csa = cs.ToArray();
			if (csa.Length != 2)
			{
				Db4oUnit.Assert.Fail("Constraints not returned");
			}
		}

		public virtual void TestEvaluation()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
				());
			q.Constrain(new _AnonymousInnerClass137(this));
			Expect(q, new int[] { 2, 3 });
		}

		private sealed class _AnonymousInnerClass137 : Db4objects.Db4o.Query.IEvaluation
		{
			public _AnonymousInnerClass137(STIntegerWUTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Evaluate(Db4objects.Db4o.Query.ICandidate candidate)
			{
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase sti = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STIntegerWUTestCase
					)candidate.GetObject();
				candidate.Include((((int)sti.i_int) + 2) > 100);
			}

			private readonly STIntegerWUTestCase _enclosing;
		}
	}
}
