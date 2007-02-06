namespace Db4objects.Db4o.Tests.Common.Foundation
{
	/// <exclude></exclude>
	public class Arrays4TestCase : Db4oUnit.ITestCase
	{
		public virtual void TestContainsInstanceOf()
		{
			object[] array = new object[] { "foo", 42 };
			Db4oUnit.Assert.IsTrue(Db4objects.Db4o.Foundation.Arrays4.ContainsInstanceOf(array
				, typeof(string)));
			Db4oUnit.Assert.IsTrue(Db4objects.Db4o.Foundation.Arrays4.ContainsInstanceOf(array
				, typeof(int)));
			Db4oUnit.Assert.IsTrue(Db4objects.Db4o.Foundation.Arrays4.ContainsInstanceOf(array
				, typeof(object)));
			Db4oUnit.Assert.IsFalse(Db4objects.Db4o.Foundation.Arrays4.ContainsInstanceOf(array
				, typeof(float)));
			Db4oUnit.Assert.IsFalse(Db4objects.Db4o.Foundation.Arrays4.ContainsInstanceOf(new 
				object[0], typeof(object)));
			Db4oUnit.Assert.IsFalse(Db4objects.Db4o.Foundation.Arrays4.ContainsInstanceOf(new 
				object[1], typeof(object)));
			Db4oUnit.Assert.IsFalse(Db4objects.Db4o.Foundation.Arrays4.ContainsInstanceOf(null
				, typeof(object)));
		}
	}
}
