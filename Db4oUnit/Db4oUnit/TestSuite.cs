namespace Db4oUnit
{
	public class TestSuite : Db4oUnit.ITest
	{
		private Db4oUnit.ITest[] _tests;

		private string _label;

		public TestSuite(string label, Db4oUnit.ITest[] tests)
		{
			this._label = label;
			this._tests = tests;
		}

		public TestSuite(Db4oUnit.ITest[] tests) : this(null, tests)
		{
		}

		public virtual string GetLabel()
		{
			return _label == null ? LabelFromTests(_tests) : _label;
		}

		public virtual Db4oUnit.ITest[] GetTests()
		{
			return _tests;
		}

		public virtual void Run(Db4oUnit.TestResult result)
		{
			Db4oUnit.ITest[] tests = GetTests();
			for (int i = 0; i < tests.Length; i++)
			{
				tests[i].Run(result);
			}
		}

		private static string LabelFromTests(Db4oUnit.ITest[] tests)
		{
			if (tests.Length == 0)
			{
				return "[]";
			}
			string firstLabel = tests[0].GetLabel();
			if (tests.Length == 1)
			{
				return "[" + firstLabel + "]";
			}
			return "[" + firstLabel + ", ...]";
		}
	}
}
