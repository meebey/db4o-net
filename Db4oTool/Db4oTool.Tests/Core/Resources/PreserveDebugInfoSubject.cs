using System;
using Db4oUnit;
using System.Globalization;
using System.Threading;

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
        CultureInfo currentUICulture = Thread.CurrentThread.CurrentUICulture;
        try
        {
            Thread.CurrentThread.CurrentUICulture = CultureInfo.InvariantCulture;
            try
            {
                Foo.Bar();
            }
            catch (Exception x)
            {
                string message = x.ToString();
                Assert.IsTrue(message.Contains("PreserveDebugInfoSubject.cs:line 10"), message);
            }
        }
        finally
        {
            Thread.CurrentThread.CurrentUICulture = currentUICulture;
        }
	}
}