/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;

namespace Db4oUnit
{
	/// <summary>A group of tests.</summary>
	/// <remarks>
	/// A group of tests.
	/// A TestSuite can be ran only once because it disposes of test instances
	/// as it runs them.
	/// </remarks>
	public class TestSuite : ITest
	{
		private ITest[] _tests;

		private string _label;

		public TestSuite(string label, ITest[] tests)
		{
			this._label = label;
			this._tests = tests;
		}

		public TestSuite(ITest[] tests) : this(null, tests)
		{
		}

		public TestSuite(ITest singleTest) : this(null, new ITest[] { singleTest })
		{
		}

		public virtual string GetLabel()
		{
			return _label == null ? LabelFromTests(_tests) : _label;
		}

		public virtual ITest[] GetTests()
		{
			return _tests;
		}

		public virtual void Run(TestResult result)
		{
			try
			{
				ITest[] tests = GetTests();
				for (int i = 0; i < tests.Length; i++)
				{
					tests[i].Run(result);
					tests[i] = null;
				}
			}
			finally
			{
				_tests = null;
			}
		}

		private static string LabelFromTests(ITest[] tests)
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
