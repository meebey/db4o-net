namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class SearcherTestCase : Db4oUnit.ITestCase, Db4oUnit.ITestLifeCycle
	{
		private Db4objects.Db4o.Inside.Btree.Searcher _searcher;

		private const int FIRST = 4;

		private const int LAST = 11;

		private readonly int[] EVEN_VALUES = new int[] { 4, 7, 9, 11 };

		private readonly int[] ODD_VALUES = new int[] { 4, 7, 8, 9, 11 };

		private readonly int[] NON_MATCHES = new int[] { 3, 5, 6, 10, 12 };

		private readonly int[] MATCHES = new int[] { 4, 7, 9, 11 };

		private const int BEFORE = FIRST - 1;

		private const int BEYOND = LAST + 1;

		public virtual void TtestPrintResults()
		{
			int[] evenValues = new int[] { 4, 7, 9, 11 };
			int[] searches = new int[] { 3, 4, 5, 7, 10, 11, 12 };
			for (int i = 0; i < searches.Length; i++)
			{
				int res = Search(evenValues, searches[i]);
				System.Console.Out.WriteLine(res);
			}
		}

		public virtual void TestCursorEndsOnSmaller()
		{
			Db4oUnit.Assert.AreEqual(0, Search(EVEN_VALUES, 6));
			Db4oUnit.Assert.AreEqual(0, Search(ODD_VALUES, 6));
			Db4oUnit.Assert.AreEqual(2, Search(EVEN_VALUES, 10));
			Db4oUnit.Assert.AreEqual(3, Search(ODD_VALUES, 10));
		}

		public virtual void TestMatchEven()
		{
			AssertMatch(EVEN_VALUES);
		}

		public virtual void TestMatchOdd()
		{
			AssertMatch(ODD_VALUES);
		}

		public virtual void TestNoMatchEven()
		{
			AssertNoMatch(EVEN_VALUES);
		}

		public virtual void TestNoMatchOdd()
		{
			AssertNoMatch(ODD_VALUES);
		}

		public virtual void TestBeyondEven()
		{
			AssertBeyond(EVEN_VALUES);
		}

		public virtual void TestBeyondOdd()
		{
			AssertBeyond(ODD_VALUES);
		}

		public virtual void TestNotBeyondEven()
		{
			AssertNotBeyond(EVEN_VALUES);
		}

		public virtual void TestNotBeyondOdd()
		{
			AssertNotBeyond(ODD_VALUES);
		}

		public virtual void TestBeforeEven()
		{
			AssertBefore(EVEN_VALUES);
		}

		public virtual void TestBeforeOdd()
		{
			AssertBefore(ODD_VALUES);
		}

		public virtual void TestNotBeforeEven()
		{
			AssertNotBefore(EVEN_VALUES);
		}

		public virtual void TestNotBeforeOdd()
		{
			AssertNotBefore(ODD_VALUES);
		}

		public virtual void TestEmptySet()
		{
			_searcher = new Db4objects.Db4o.Inside.Btree.Searcher(Db4objects.Db4o.Inside.Btree.SearchTarget
				.ANY, 0);
			if (_searcher.Incomplete())
			{
				Db4oUnit.Assert.Fail();
			}
			Db4oUnit.Assert.AreEqual(0, _searcher.Cursor());
		}

		private void AssertMatch(int[] values)
		{
			for (int i = 0; i < MATCHES.Length; i++)
			{
				Search(values, MATCHES[i]);
				Db4oUnit.Assert.IsTrue(_searcher.FoundMatch());
			}
		}

		private void AssertNoMatch(int[] values)
		{
			for (int i = 0; i < NON_MATCHES.Length; i++)
			{
				Search(values, NON_MATCHES[i]);
				Db4oUnit.Assert.IsFalse(_searcher.FoundMatch());
			}
		}

		private void AssertBeyond(int[] values)
		{
			int res = Search(values, BEYOND);
			Db4oUnit.Assert.AreEqual(values.Length - 1, res);
			Db4oUnit.Assert.IsTrue(_searcher.AfterLast());
		}

		private void AssertNotBeyond(int[] values)
		{
			int res = Search(values, LAST);
			Db4oUnit.Assert.AreEqual(values.Length - 1, res);
			Db4oUnit.Assert.IsFalse(_searcher.AfterLast());
		}

		private void AssertBefore(int[] values)
		{
			int res = Search(values, BEFORE);
			Db4oUnit.Assert.AreEqual(0, res);
			Db4oUnit.Assert.IsTrue(_searcher.BeforeFirst());
		}

		private void AssertNotBefore(int[] values)
		{
			int res = Search(values, FIRST);
			Db4oUnit.Assert.AreEqual(0, res);
			Db4oUnit.Assert.IsFalse(_searcher.BeforeFirst());
		}

		private int Search(int[] values, int value)
		{
			_searcher = new Db4objects.Db4o.Inside.Btree.Searcher(Db4objects.Db4o.Inside.Btree.SearchTarget
				.ANY, values.Length);
			while (_searcher.Incomplete())
			{
				_searcher.ResultIs(values[_searcher.Cursor()] - value);
			}
			return _searcher.Cursor();
		}

		public virtual void SetUp()
		{
			_searcher = null;
		}

		public virtual void TearDown()
		{
		}
	}
}
