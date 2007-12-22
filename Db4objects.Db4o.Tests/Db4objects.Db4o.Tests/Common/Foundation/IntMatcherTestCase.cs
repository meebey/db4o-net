/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Internal;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class IntMatcherTestCase : ITestCase
	{
		public virtual void Test()
		{
			Assert.IsTrue(IntMatcher.ZERO.Match(0));
			Assert.IsFalse(IntMatcher.ZERO.Match(-1));
			Assert.IsFalse(IntMatcher.ZERO.Match(1));
			Assert.IsFalse(IntMatcher.ZERO.Match(int.MinValue));
			Assert.IsFalse(IntMatcher.ZERO.Match(int.MaxValue));
			Assert.IsFalse(IntMatcher.POSITIVE.Match(0));
			Assert.IsFalse(IntMatcher.POSITIVE.Match(-1));
			Assert.IsTrue(IntMatcher.POSITIVE.Match(1));
			Assert.IsFalse(IntMatcher.POSITIVE.Match(int.MinValue));
			Assert.IsTrue(IntMatcher.POSITIVE.Match(int.MaxValue));
			Assert.IsFalse(IntMatcher.NEGATIVE.Match(0));
			Assert.IsTrue(IntMatcher.NEGATIVE.Match(-1));
			Assert.IsFalse(IntMatcher.NEGATIVE.Match(1));
			Assert.IsTrue(IntMatcher.NEGATIVE.Match(int.MinValue));
			Assert.IsFalse(IntMatcher.NEGATIVE.Match(int.MaxValue));
		}
	}
}
