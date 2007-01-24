namespace Db4objects.Db4o.Tests.Common.Soda.Experiments
{
	public class STIdentityEvaluationTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
	{
		public override object[] CreateData()
		{
			Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
				 helperA = new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
				("aaa");
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(null), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(helperA), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(helperA), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(helperA), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.HelperDerivate
				("bbb")), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
				("dod")) };
		}

		public Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
			 helper;

		public STIdentityEvaluationTestCase()
		{
		}

		public STIdentityEvaluationTestCase(Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
			 h)
		{
			this.helper = h;
		}

		public virtual void Test()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
				("aaa"));
			Db4objects.Db4o.IObjectSet os = q.Execute();
			Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
				 helperA = (Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
				)os.Next();
			q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase)
				);
			q.Descend("helper").Constrain(helperA).Identity();
			q.Constrain(new _AnonymousInnerClass42(this));
			Expect(q, new int[] { 1, 2, 3 });
		}

		private sealed class _AnonymousInnerClass42 : Db4objects.Db4o.Query.IEvaluation
		{
			public _AnonymousInnerClass42(STIdentityEvaluationTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Evaluate(Db4objects.Db4o.Query.ICandidate candidate)
			{
				candidate.Include(true);
			}

			private readonly STIdentityEvaluationTestCase _enclosing;
		}

		public virtual void TestMemberClassConstraint()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase)
				);
			q.Descend("helper").Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.HelperDerivate)
				);
			Expect(q, new int[] { 4 });
		}

		public class Helper
		{
			public string hString;

			public Helper()
			{
			}

			public Helper(string str)
			{
				hString = str;
			}
		}

		public class HelperDerivate : Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase.Helper
		{
			public HelperDerivate()
			{
			}

			public HelperDerivate(string str) : base(str)
			{
			}
		}
	}
}
