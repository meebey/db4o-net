/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

using System.Collections.Generic;
using Db4oTool.Tests.TA; // MockActivator
using Db4oUnit;

class Tagged
{
    public string tags;

    public Tagged(string tags_)
    {
        tags = tags_;
    }
}

struct ValueTypeSubject
{
    public ValueTypeSubject(int i)
    {
        intValue = i;
    }

    public int intValue;
}

class FieldSetterTestSubject
{
    public int intValue = 0;
    public volatile byte volatileByte = 0;
    public Tagged refValue = null;
    public ValueTypeSubject valueType;
    public List<int> intList = null;
}

class TAFieldSetterSubject : ITestCase
{
    public void TestFieldSetterActivatesObject()
    {
        FieldSetterTestSubject obj = new FieldSetterTestSubject();
        MockActivator a = ActivatorFor(obj);
        Assert.AreEqual(0, a.Count);
     
        obj.intValue = 10;
        int activationCount = 1;
        Assert.AreEqual(activationCount++, a.Count);

        obj.refValue = null;
        Assert.AreEqual(activationCount++, a.Count);

        obj.volatileByte = 3;
        Assert.AreEqual(activationCount++, a.Count);

        obj.valueType.intValue = 4;
        Assert.AreEqual(activationCount++, a.Count);

        obj.valueType = new ValueTypeSubject(5);
        Assert.AreEqual(activationCount++, a.Count);

        obj.intList = new List<int>(6);
        Assert.AreEqual(activationCount++, a.Count);
    }

    private static MockActivator ActivatorFor(object p)
    {
        return MockActivator.ActivatorFor(p);
    }
}