/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Internal.Handlers;
using Db4objects.Db4o.Tests.Common.Internal;

namespace Db4objects.Db4o.Tests.Common.Internal
{
	public class Comparable4TestCase : ITestCase
	{
		public static void Main(string[] args)
		{
			new TestRunner(typeof(Comparable4TestCase)).Run();
		}

		public virtual void TestIntHandler()
		{
			Assert.IsGreater(0, CompareInteger(4, 2));
			Assert.IsSmaller(0, CompareInteger(3, 5));
			Assert.AreEqual(0, CompareInteger(7, 7));
		}

		private int CompareInteger(int lhs, int rhs)
		{
			IntHandler intHandler = new IntHandler(null);
			IPreparedComparison comparable = intHandler.NewPrepareCompare(lhs);
			return comparable.CompareTo(rhs);
		}
	}
}
