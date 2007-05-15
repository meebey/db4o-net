
using System;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4oUnit;

class Project
{
	private string _name;

	public Project(string name)
	{
		_name = name;
	}

	public string Name
	{
		get { return _name;  }
	}
}

struct AValueType
{
	
}

class MockActivator : IActivator
{
	private int _count;

	public int Count
	{
		get { return _count;  }
	}

	public void Activate()
	{
		++_count;
	}
}

class TAInstrumentationSubject : ITestCase
{
	public void TestActivatableBehavior()
	{
		Assert.IsTrue(IsActivatable(typeof(Project)));

		MockActivator activator = new MockActivator();
		Project p = new Project("test");
		((IActivatable)p).Bind(activator);
		Assert.AreEqual(0, activator.Count);
		Assert.AreEqual("test", p.Name);
		Assert.AreEqual(1, activator.Count);
	}

	public void TestValueTypesAreNotInstrumented()
	{
		Assert.IsFalse(IsActivatable(typeof(AValueType)));
	}

	private static bool IsActivatable(Type type)
	{
		return typeof(IActivatable).IsAssignableFrom(type);
	}
}