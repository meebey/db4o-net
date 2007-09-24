using System;
using Db4oUnit;

class Foo
{
	public static void Bar()
	{
		throw new ApplicationException();
	}
}

public class PreserveDebugInfoSubject : ITestCase
{
	public void Test()
	{
		try
		{
			Foo.Bar();
		}
		catch (Exception x)
		{
			Assert.IsTrue(x.ToString().Contains("PreserveDebugInfoSubject.cs:line 8"));
		}
	}
}