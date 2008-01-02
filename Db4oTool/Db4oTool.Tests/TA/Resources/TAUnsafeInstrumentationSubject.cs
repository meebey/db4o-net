using System;
using Db4objects.Db4o.TA;
using Db4oUnit;
using Db4oTool.Tests.TA; // MockActivator

unsafe public class PointerContainer
{
	public int* foo;
}

public delegate void FooHandler();

public class DelegateContainer
{
	public FooHandler foo;

	public EventHandler<EventArgs> bar;

	public EventHandler baz;
}

public class TAUnsafeInstrumentationSubject : ITestCase
{
	public void TestPointerDoesntCauseActivation()
	{
		PointerContainer obj = new PointerContainer();
		MockActivator a = ActivatorFor(obj);
		unsafe
		{	
			Assert.IsTrue(null == obj.foo);
		}
		Assert.AreEqual(0, a.ReadCount);
	}

	public void TestDelegateDoesntCauseActivation()
	{
		DelegateContainer obj = new DelegateContainer();
		MockActivator a = ActivatorFor(obj);
		Assert.IsNull(obj.foo);
		Assert.IsNull(obj.bar);
		Assert.IsNull(obj.baz);
		Assert.AreEqual(0, a.ReadCount);
	}

	public void TestDelegateIsNotInstrumented()
	{
		Assert.IsFalse(typeof(IActivatable).IsAssignableFrom(typeof(FooHandler)));
	}

	private MockActivator ActivatorFor(object p)
	{
		return MockActivator.ActivatorFor(p);
	}
}

