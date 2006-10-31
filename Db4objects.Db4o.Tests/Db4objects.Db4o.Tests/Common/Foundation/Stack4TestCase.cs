namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class Stack4TestCase : Db4oUnit.ITestCase
	{
		public static void Main(string[] args)
		{
			new Db4oUnit.TestRunner(typeof(Db4objects.Db4o.Tests.Common.Foundation.Stack4TestCase)
				).Run();
		}

		public virtual void TestPushPop()
		{
			Db4objects.Db4o.Foundation.Stack4 stack = new Db4objects.Db4o.Foundation.Stack4();
			Db4oUnit.Assert.IsNull(stack.Peek());
			Db4oUnit.Assert.IsNull(stack.Pop());
			stack.Push("a");
			stack.Push("b");
			stack.Push("c");
			Db4oUnit.Assert.AreEqual("c", stack.Peek());
			Db4oUnit.Assert.AreEqual("c", stack.Peek());
			Db4oUnit.Assert.AreEqual("c", stack.Pop());
			Db4oUnit.Assert.AreEqual("b", stack.Pop());
			Db4oUnit.Assert.AreEqual("a", stack.Peek());
			Db4oUnit.Assert.AreEqual("a", stack.Pop());
			Db4oUnit.Assert.IsNull(stack.Peek());
			Db4oUnit.Assert.IsNull(stack.Pop());
		}
	}
}
