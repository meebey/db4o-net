/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4oUnit;
using Db4objects.Db4o.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class Arrays4TestCase : ITestCase
	{
		public virtual void TestContainsInstanceOf()
		{
			object[] array = new object[] { "foo", 42 };
			Assert.IsTrue(Arrays4.ContainsInstanceOf(array, typeof(string)));
			Assert.IsTrue(Arrays4.ContainsInstanceOf(array, typeof(int)));
			Assert.IsTrue(Arrays4.ContainsInstanceOf(array, typeof(object)));
			Assert.IsFalse(Arrays4.ContainsInstanceOf(array, typeof(float)));
			Assert.IsFalse(Arrays4.ContainsInstanceOf(new object[0], typeof(object)));
			Assert.IsFalse(Arrays4.ContainsInstanceOf(new object[1], typeof(object)));
			Assert.IsFalse(Arrays4.ContainsInstanceOf(null, typeof(object)));
		}
	}
}
