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
        Assert.AreEqual(0, a.ReadCount);
     
        obj.intValue = 10;
        int writeCount = 1;
		Assert.AreEqual(writeCount++, a.WriteCount);

        obj.refValue = null;
		Assert.AreEqual(writeCount++, a.WriteCount);

        obj.volatileByte = 3;
		Assert.AreEqual(writeCount++, a.WriteCount);
		
		Assert.AreEqual(0, a.ReadCount);
        obj.valueType.intValue = 4;
		Assert.AreEqual(1, a.ReadCount);

        obj.valueType = new ValueTypeSubject(5);
		Assert.AreEqual(writeCount++, a.WriteCount);

        obj.intList = new List<int>(6);
		Assert.AreEqual(writeCount++, a.WriteCount);
		Assert.AreEqual(1, a.ReadCount);
    }

    private static MockActivator ActivatorFor(object p)
    {
        return MockActivator.ActivatorFor(p);
    }
}