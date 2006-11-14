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
			AssertEmpty(stack);
			stack.Push("a");
			stack.Push("b");
			stack.Push("c");
			Db4oUnit.Assert.AreEqual("c", stack.Peek());
			Db4oUnit.Assert.AreEqual("c", stack.Peek());
			Db4oUnit.Assert.AreEqual("c", stack.Pop());
			Db4oUnit.Assert.AreEqual("b", stack.Pop());
			Db4oUnit.Assert.AreEqual("a", stack.Peek());
			Db4oUnit.Assert.AreEqual("a", stack.Pop());
			AssertEmpty(stack);
		}

		private void AssertEmpty(Db4objects.Db4o.Foundation.Stack4 stack)
		{
			Db4oUnit.Assert.IsNull(stack.Peek());
			Db4oUnit.Assert.Expect(typeof(System.InvalidOperationException), new _AnonymousInnerClass33
				(this, stack));
		}

		private sealed class _AnonymousInnerClass33 : Db4oUnit.ICodeBlock
		{
			public _AnonymousInnerClass33(Stack4TestCase _enclosing, Db4objects.Db4o.Foundation.Stack4
				 stack)
			{
				this._enclosing = _enclosing;
				this.stack = stack;
			}

			public void Run()
			{
				stack.Pop();
			}

			private readonly Stack4TestCase _enclosing;

			private readonly Db4objects.Db4o.Foundation.Stack4 stack;
		}
	}
}
