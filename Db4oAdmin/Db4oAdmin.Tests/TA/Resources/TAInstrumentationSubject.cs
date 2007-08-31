﻿/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System;
using System.Runtime.CompilerServices;
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

	override public bool Equals(object o)
	{
		Project other = o as Project;
		if (other == null) return false;

		// foreign field access
		return _name == other._name;
	}

	override public int GetHashCode()
	{
		return _name.GetHashCode();
	}
}

struct AValueType
{	
}

abstract class AnAbstractType
{
}

[CompilerGenerated]
class CompilerGeneratedType
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
	public void TestIsActivatable()
	{
		Assert.IsTrue(IsActivatable(typeof(Project)));
	}

	public void TestPropertyGetter()
	{
		Project p = new Project("test");
		MockActivator activator = ActivatorFor(p);

		Assert.AreEqual(0, activator.Count);
		Assert.AreEqual("test", p.Name);
		Assert.AreEqual(1, activator.Count);
	}

	public void TestForeignFieldAccess()
	{
		Project p1 = new Project("test");
		Project p2 = new Project("test");

		MockActivator a1 = ActivatorFor(p1);
		MockActivator a2 = ActivatorFor(p2);

		Assert.IsTrue(p1.Equals(p2));

		Assert.AreEqual(1, a1.Count);
		Assert.AreEqual(1, a2.Count);
	}

	public void TestValueTypesAreNotInstrumented()
	{
		Assert.IsFalse(IsActivatable(typeof(AValueType)));
	}

	public void TestAbstractClassesAreNotInstrumented()
	{
		Assert.IsFalse(IsActivatable(typeof(AnAbstractType)));
	}

	public void TestCompilerGeneratedClassesAreNotInstrumented()
	{
		Assert.IsFalse(IsActivatable(typeof(CompilerGeneratedType)));
	}

	private MockActivator ActivatorFor(Project p)
	{
		MockActivator activator = new MockActivator();
		((IActivatable)p).Bind(activator);
		return activator;
	}

	private static bool IsActivatable(Type type)
	{
		return typeof(IActivatable).IsAssignableFrom(type);
	}
}