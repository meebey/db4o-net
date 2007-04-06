using Db4objects.Db4o;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tests.Common.Soda.Experiments;
using Db4objects.Db4o.Tests.Common.Soda.Util;

namespace Db4objects.Db4o.Tests.Common.Soda.Experiments
{
	public class STIdentityEvaluationTestCase : SodaBaseTestCase
	{
		public override object[] CreateData()
		{
			STIdentityEvaluationTestCase.Helper helperA = new STIdentityEvaluationTestCase.Helper
				("aaa");
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(null), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(helperA), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(helperA), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(helperA), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(new STIdentityEvaluationTestCase.HelperDerivate("bbb")), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase
				(new STIdentityEvaluationTestCase.Helper("dod")) };
		}

		public STIdentityEvaluationTestCase.Helper helper;

		public STIdentityEvaluationTestCase()
		{
		}

		public STIdentityEvaluationTestCase(STIdentityEvaluationTestCase.Helper h)
		{
			this.helper = h;
		}

		public virtual void Test()
		{
			IQuery q = NewQuery();
			q.Constrain(new STIdentityEvaluationTestCase.Helper("aaa"));
			IObjectSet os = q.Execute();
			STIdentityEvaluationTestCase.Helper helperA = (STIdentityEvaluationTestCase.Helper
				)os.Next();
			q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase)
				);
			q.Descend("helper").Constrain(helperA).Identity();
			q.Constrain(new _AnonymousInnerClass42(this));
			Expect(q, new int[] { 1, 2, 3 });
		}

		private sealed class _AnonymousInnerClass42 : IEvaluation
		{
			public _AnonymousInnerClass42(STIdentityEvaluationTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Evaluate(ICandidate candidate)
			{
				candidate.Include(true);
			}

			private readonly STIdentityEvaluationTestCase _enclosing;
		}

		public virtual void TestMemberClassConstraint()
		{
			IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Experiments.STIdentityEvaluationTestCase)
				);
			q.Descend("helper").Constrain(typeof(STIdentityEvaluationTestCase.HelperDerivate)
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

		public class HelperDerivate : STIdentityEvaluationTestCase.Helper
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
