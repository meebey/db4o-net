using System;
using Db4oUnit;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Tests.Common.Foundation;

namespace Db4objects.Db4o.Tests.Common.Foundation
{
	public class Stack4TestCase : ITestCase
	{
		public static void Main(string[] args)
		{
			new TestRunner(typeof(Stack4TestCase)).Run();
		}

		public virtual void TestPushPop()
		{
			Stack4 stack = new Stack4();
			AssertEmpty(stack);
			stack.Push("a");
			stack.Push("b");
			stack.Push("c");
			Assert.IsFalse(stack.IsEmpty());
			Assert.AreEqual("c", stack.Peek());
			Assert.AreEqual("c", stack.Peek());
			Assert.AreEqual("c", stack.Pop());
			Assert.AreEqual("b", stack.Pop());
			Assert.AreEqual("a", stack.Peek());
			Assert.AreEqual("a", stack.Pop());
			AssertEmpty(stack);
		}

		private void AssertEmpty(Stack4 stack)
		{
			Assert.IsTrue(stack.IsEmpty());
			Assert.IsNull(stack.Peek());
			Assert.Expect(typeof(InvalidOperationException), new _AnonymousInnerClass35(this, 
				stack));
		}

		private sealed class _AnonymousInnerClass35 : ICodeBlock
		{
			public _AnonymousInnerClass35(Stack4TestCase _enclosing, Stack4 stack)
			{
				this._enclosing = _enclosing;
				this.stack = stack;
			}

			public void Run()
			{
				stack.Pop();
			}

			private readonly Stack4TestCase _enclosing;

			private readonly Stack4 stack;
		}
	}
}
