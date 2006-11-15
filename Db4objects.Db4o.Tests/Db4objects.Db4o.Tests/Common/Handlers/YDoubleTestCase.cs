namespace Db4objects.Db4o.Tests.Common.Handlers
{
	/// <exclude></exclude>
	public class YDoubleTestCase : Db4oUnit.Extensions.AbstractDb4oTestCase
	{
		private Db4objects.Db4o.ITypeHandler4 _handler;

		protected override void Db4oSetupBeforeStore()
		{
			_handler = new Db4objects.Db4o.YDouble(Stream());
		}

		public virtual void TestMarshalling()
		{
			double expected = 1.1;
			Db4objects.Db4o.YapReader buffer = new Db4objects.Db4o.YapReader(_handler.LinkLength
				());
			_handler.WriteIndexEntry(buffer, expected);
			buffer.Seek(0);
			object actual = _handler.ReadIndexEntry(buffer);
			Db4oUnit.Assert.AreEqual(expected, actual);
		}

		public virtual void TestComparison()
		{
			AssertComparison(0, 1.1, 1.1);
			AssertComparison(1, 1.0, 1.1);
			AssertComparison(-1, 1.1, 0.5);
		}

		private void AssertComparison(int expected, double prepareWith, double compareTo)
		{
			_handler.PrepareComparison(prepareWith);
			double doubleCompareTo = compareTo;
			Db4oUnit.Assert.AreEqual(expected, _handler.CompareTo(doubleCompareTo));
			switch (expected)
			{
				case 0:
				{
					Db4oUnit.Assert.IsTrue(_handler.IsEqual(doubleCompareTo));
					Db4oUnit.Assert.IsFalse(_handler.IsGreater(doubleCompareTo));
					Db4oUnit.Assert.IsFalse(_handler.IsSmaller(doubleCompareTo));
					break;
				}

				case 1:
				{
					Db4oUnit.Assert.IsFalse(_handler.IsEqual(doubleCompareTo));
					Db4oUnit.Assert.IsTrue(_handler.IsGreater(doubleCompareTo));
					Db4oUnit.Assert.IsFalse(_handler.IsSmaller(doubleCompareTo));
					break;
				}

				case -1:
				{
					Db4oUnit.Assert.IsFalse(_handler.IsEqual(doubleCompareTo));
					Db4oUnit.Assert.IsFalse(_handler.IsGreater(doubleCompareTo));
					Db4oUnit.Assert.IsTrue(_handler.IsSmaller(doubleCompareTo));
					break;
				}
			}
		}
	}
}
