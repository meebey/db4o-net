namespace Db4objects.Db4o.Tests.Common.Soda.Classes.Simple
{
	public class STStringTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
		, Db4objects.Db4o.Tests.Common.Soda.ISTInterface
	{
		public string str;

		public STStringTestCase()
		{
		}

		public STStringTestCase(string str)
		{
			this.str = str;
		}

		/// <summary>needed for STInterface test</summary>
		public virtual object ReturnSomething()
		{
			return str;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				(null), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("aaa"
				), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("bbb"), 
				new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("dod") };
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
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				());
			q.Descend("str").Constrain("bbb");
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("bbb"));
		}

		public virtual void TestContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("od"));
			q.Descend("str").Constraints().Contains();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("dod"));
		}

		public virtual void TestNotContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("od"));
			q.Descend("str").Constraints().Contains().Not();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase(null), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("bbb"
				) });
		}

		public virtual void TestLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("do"));
			q.Descend("str").Constraints().Like();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("dod"));
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("od"));
			q.Descend("str").Constraints().Like();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, _array[3]);
		}

		public virtual void TestStartsWith()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("do"));
			q.Descend("str").Constraints().StartsWith(true);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("dod"));
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("od"));
			q.Descend("str").Constraints().StartsWith(true);
			Expect(q, new int[] {  });
		}

		public virtual void TestEndsWith()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("do"));
			q.Descend("str").Constraints().EndsWith(true);
			Expect(q, new int[] {  });
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("od"));
			q.Descend("str").Constraints().EndsWith(true);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("dod"));
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("D"));
			q.Descend("str").Constraints().EndsWith(false);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("dod"));
		}

		public virtual void TestNotLike()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa"));
			q.Descend("str").Constraints().Like().Not();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase(null), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("bbb"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("dod"
				) });
			q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("xxx"));
			q.Descend("str").Constraints().Like();
			Expect(q, new int[] {  });
		}

		public virtual void TestIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa"));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase)set.Next();
			identityConstraint.str = "hihs";
			q = NewQuery();
			q.Constrain(identityConstraint).Identity();
			identityConstraint.str = "aaa";
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa"));
		}

		public virtual void TestNotIdentity()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa"));
			Db4objects.Db4o.IObjectSet set = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase identityConstraint
				 = (Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase)set.Next();
			identityConstraint.str = null;
			q = NewQuery();
			q.Constrain(identityConstraint).Identity().Not();
			identityConstraint.str = "aaa";
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase(null), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("bbb"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("dod"
				) });
		}

		public virtual void TestNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				(null));
			q.Descend("str").Constrain(null);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				(null));
		}

		public virtual void TestNotNull()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				(null));
			q.Descend("str").Constrain(null).Not();
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("aaa"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("bbb"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase("dod"
				) });
		}

		public virtual void TestConstraints()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa"));
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("bbb"));
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
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				(null));
			q.Constrain(new _AnonymousInnerClass179(this));
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("dod"));
		}

		private sealed class _AnonymousInnerClass179 : Db4objects.Db4o.Query.IEvaluation
		{
			public _AnonymousInnerClass179(STStringTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Evaluate(Db4objects.Db4o.Query.ICandidate candidate)
			{
				Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase sts = (Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
					)candidate.GetObject();
				candidate.Include(sts.str.IndexOf("od") == 1);
			}

			private readonly STStringTestCase _enclosing;
		}

		public virtual void TestCaseInsenstiveContains()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase)
				);
			q.Constrain(new _AnonymousInnerClass191(this));
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("dod"));
		}

		private sealed class _AnonymousInnerClass191 : Db4objects.Db4o.Query.IEvaluation
		{
			public _AnonymousInnerClass191(STStringTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Evaluate(Db4objects.Db4o.Query.ICandidate candidate)
			{
				Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase sts = (Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
					)candidate.GetObject();
				candidate.Include(sts.str.ToLower().IndexOf("od") >= 0);
			}

			private readonly STStringTestCase _enclosing;
		}
	}
}
