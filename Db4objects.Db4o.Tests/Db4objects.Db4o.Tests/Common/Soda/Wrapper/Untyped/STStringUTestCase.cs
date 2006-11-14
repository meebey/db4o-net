namespace Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped
{
	public class STStringUTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public object str;

		public STStringUTestCase()
		{
		}

		public STStringUTestCase(string str)
		{
			this.str = str;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				(null), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase(
				"aaa"), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase(
				"bbb"), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase(
				"dod") };
		}

		public virtual void TestEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(_array[2]);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[2]);
		}

		public virtual void TestNotEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(_array[2]);
			q.Descend("str").Constraints().Not();
			Expect(q, new int[] { 0, 1, 3 });
		}

		public virtual void TestDescendantEquals()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				());
			q.Descend("str").Constrain("bbb");
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("bbb"));
		}

		public virtual void TestContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("od"));
			q.Descend("str").Constraints().Contains();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("dod"));
		}

		public virtual void TestNotContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("od"));
			q.Descend("str").Constraints().Contains().Not();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase(null), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("aaa"), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("bbb") });
		}

		public virtual void TestLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("do"));
			q.Descend("str").Constraints().Like();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("dod"));
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("od"));
			q.Descend("str").Constraints().Like();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[3]);
		}

		public virtual void TestNotLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("aaa"));
			q.Descend("str").Constraints().Like().Not();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase(null), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("bbb"), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("dod") });
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("xxx"));
			q.Descend("str").Constraints().Like();
			Expect(q, new int[] {  });
		}

		public virtual void TestIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("aaa"));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase)set.Next
				();
			identityConstraint.str = "hihs";
			q = NewQuery();
			q.Constrain(identityConstraint).Identity();
			identityConstraint.str = "aaa";
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("aaa"));
		}

		public virtual void TestNotIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("aaa"));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase)set.Next
				();
			identityConstraint.str = null;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity().Not();
			identityConstraint.str = "aaa";
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase(null), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("bbb"), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("dod") });
		}

		public virtual void TestNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				(null));
			q.Descend("str").Constrain(null);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				(null));
		}

		public virtual void TestNotNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				(null));
			q.Descend("str").Constrain(null).Not();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase("aaa"), new 
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase("bbb"), new 
				Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase("dod") });
		}

		public virtual void TestConstraints()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("aaa"));
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("bbb"));
			Db4objects.Db4o.Query.IConstraints cs = q.Constraints();
			Db4objects.Db4o.Query.IConstraint[] csa = cs.ToArray();
			if (csa.Length != 2)
			{
				Db4oUnit.Assert.Fail("Constraints not returned");
			}
		}
	}
}
