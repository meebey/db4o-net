namespace Db4objects.Db4o.Tests.Common.Soda.Experiments
{
	public class STMagicTestCase : Db4objects.Db4o.Tests.Common.Soda.Util.SodaBaseTestCase
		, Db4objects.Db4o.Tests.Common.Soda.ISTInterface
	{
		public string str;

		public STMagicTestCase()
		{
		}

		private STMagicTestCase(string str)
		{
			this.str = str;
		}

		public override string ToString()
		{
			return "STMagicTestCase: " + str;
		}

		/// <summary>needed for STInterface test</summary>
		public virtual object ReturnSomething()
		{
			return str;
		}

		public override object[] CreateData()
		{
			return new object[] { new Db4objects.Db4o.Tests.Common.Soda.Experiments.STMagicTestCase
				("aaa"), new Db4objects.Db4o.Tests.Common.Soda.Experiments.STMagicTestCase("aaax"
				) };
		}

		/// <summary>
		/// Magic:
		/// Query for all objects with a known attribute,
		/// independant of the class or even if you don't
		/// know the class.
		/// </summary>
		/// <remarks>
		/// Magic:
		/// Query for all objects with a known attribute,
		/// independant of the class or even if you don't
		/// know the class.
		/// </remarks>
		public virtual void TestUnconstrainedClass()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Descend("str").Constrain("aaa");
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Experiments.STMagicTestCase("aaa"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa"), new Db4objects.Db4o.Tests.Common.Soda.Wrapper.Untyped.STStringUTestCase
				("aaa") });
		}

		/// <summary>
		/// Magic:
		/// Query for multiple classes.
		/// </summary>
		/// <remarks>
		/// Magic:
		/// Query for multiple classes.
		/// Every class gets it's own slot in the query graph.
		/// </remarks>
		public virtual void TestMultiClass()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STDoubleTestCase)
				).Or(q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase)
				));
			object[] stDoubles = new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STDoubleTestCase
				().CreateData();
			object[] stStrings = new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				().CreateData();
			object[] res = new object[stDoubles.Length + stStrings.Length];
			System.Array.Copy(stDoubles, 0, res, 0, stDoubles.Length);
			System.Array.Copy(stStrings, 0, res, stDoubles.Length, stStrings.Length);
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, res);
		}

		/// <summary>
		/// Magic:
		/// Execute any node in the query graph.
		/// </summary>
		/// <remarks>
		/// Magic:
		/// Execute any node in the query graph.
		/// The data for this example can be found in STTH1.java.
		/// </remarks>
		public virtual void TestExecuteAnyNode()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STTH1TestCase
				().CreateData()[5]);
			q = q.Descend("h2").Descend("h3");
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.ExpectOne(q, new Db4objects.Db4o.Tests.Common.Soda.Classes.Typedhierarchy.STTH3
				("str3"));
		}

		/// <summary>
		/// Magic:
		/// Querying for an implemented Interface.
		/// </summary>
		/// <remarks>
		/// Magic:
		/// Querying for an implemented Interface.
		/// Using an Evaluation allows calls to the interface methods
		/// during the run of the query.s
		/// </remarks>
		public virtual void TestInterface()
		{
			Db4objects.Db4o.Query.IQuery q = NewQuery();
			q.Constrain(typeof(Db4objects.Db4o.Tests.Common.Soda.ISTInterface));
			q.Constrain(new _AnonymousInnerClass117(this));
			Db4objects.Db4o.Tests.Common.Soda.Util.SodaTestUtil.Expect(q, new object[] { new 
				Db4objects.Db4o.Tests.Common.Soda.Experiments.STMagicTestCase("aaa"), new Db4objects.Db4o.Tests.Common.Soda.Classes.Simple.STStringTestCase
				("aaa") });
		}

		private sealed class _AnonymousInnerClass117 : Db4objects.Db4o.Query.IEvaluation
		{
			public _AnonymousInnerClass117(STMagicTestCase _enclosing)
			{
				this._enclosing = _enclosing;
			}

			public void Evaluate(Db4objects.Db4o.Query.ICandidate candidate)
			{
				Db4objects.Db4o.Tests.Common.Soda.ISTInterface sti = (Db4objects.Db4o.Tests.Common.Soda.ISTInterface
					)candidate.GetObject();
				candidate.Include(sti.ReturnSomething().Equals("aaa"));
			}

			private readonly STMagicTestCase _enclosing;
		}
	}
}
