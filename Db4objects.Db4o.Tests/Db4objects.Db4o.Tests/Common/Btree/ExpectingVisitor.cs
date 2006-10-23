namespace Db4objects.Db4o.Tests.Common.Btree
{
	public class ExpectingVisitor : Db4objects.Db4o.Foundation.IVisitor4
	{
		private const bool DEBUG = false;

		private readonly object[] _expected;

		private readonly bool _obeyOrder;

		private readonly Db4objects.Db4o.Foundation.Collection4 _unexpected = new Db4objects.Db4o.Foundation.Collection4
			();

		private int _cursor;

		private sealed class _AnonymousInnerClass22 : object
		{
			public _AnonymousInnerClass22()
			{
			}

			public override string ToString()
			{
				return "[FOUND]";
			}
		}

		private static readonly object FOUND = new _AnonymousInnerClass22();

		public ExpectingVisitor(object[] results, bool obeyOrder)
		{
			_expected = results;
			_obeyOrder = obeyOrder;
		}

		public ExpectingVisitor(object[] results) : this(results, false)
		{
		}

		public ExpectingVisitor(object singleObject) : this(new object[] { singleObject }
			)
		{
		}

		public virtual void Visit(object obj)
		{
			if (_obeyOrder)
			{
				VisitOrdered(obj);
			}
			else
			{
				VisitUnOrdered(obj);
			}
		}

		private void VisitOrdered(object obj)
		{
			if (_cursor < _expected.Length)
			{
				if (AreEqual(_expected[_cursor], obj))
				{
					Ods("Expected OK: " + obj.ToString());
					_expected[_cursor] = FOUND;
					_cursor++;
					return;
				}
			}
			Unexpected(obj);
		}

		private void Unexpected(object obj)
		{
			_unexpected.Add(obj);
			Ods("Unexpected: " + obj);
		}

		private void VisitUnOrdered(object obj)
		{
			for (int i = 0; i < _expected.Length; i++)
			{
				object expectedItem = _expected[i];
				if (AreEqual(obj, expectedItem))
				{
					Ods("Expected OK: " + obj);
					_expected[i] = FOUND;
					return;
				}
			}
			Unexpected(obj);
		}

		private bool AreEqual(object obj, object expectedItem)
		{
			return expectedItem == obj || (expectedItem != null && obj != null && expectedItem
				.Equals(obj));
		}

		private static void Ods(string message)
		{
		}

		public virtual void AssertExpectations()
		{
			if (_unexpected.Size() > 0)
			{
				Db4oUnit.Assert.Fail("UNEXPECTED: " + _unexpected.ToString());
			}
			for (int i = 0; i < _expected.Length; i++)
			{
				Db4oUnit.Assert.AreSame(FOUND, _expected[i]);
			}
		}
	}
}
