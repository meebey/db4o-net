/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Tests.Foundation
{
	public class Set4Testcase : Db4oUnit.ITestCase
	{
		public virtual void TestSingleElementIteration()
		{
			Db4objects.Drs.Tests.Foundation.Set4 set = NewSet("first");
			Db4oUnit.Assert.AreEqual("first", Db4objects.Db4o.Foundation.Iterators.Next(set.GetEnumerator
				()));
		}

		public virtual void TestContainsAll()
		{
			Db4objects.Drs.Tests.Foundation.Set4 set = NewSet("42");
			set.Add("foo");
			Db4oUnit.Assert.IsTrue(set.ContainsAll(NewSet("42")));
			Db4oUnit.Assert.IsTrue(set.ContainsAll(NewSet("foo")));
			Db4oUnit.Assert.IsTrue(set.ContainsAll(set));
			Db4objects.Drs.Tests.Foundation.Set4 other = new Db4objects.Drs.Tests.Foundation.Set4
				(set);
			other.Add("bar");
			Db4oUnit.Assert.IsFalse(set.ContainsAll(other));
		}

		private Db4objects.Drs.Tests.Foundation.Set4 NewSet(string firstElement)
		{
			Db4objects.Drs.Tests.Foundation.Set4 set = new Db4objects.Drs.Tests.Foundation.Set4
				();
			set.Add(firstElement);
			return set;
		}
	}
}
