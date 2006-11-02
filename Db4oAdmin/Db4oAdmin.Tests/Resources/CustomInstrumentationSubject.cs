using System;
using System.IO;
using Db4oUnit;

public class CustomInstrumentationSubject : ITestCase
{
	static void Foo()
	{
		Bar();
	}
	
	static void Bar()
	{	
	}
	
	public void TestInstrumentation()
	{
		StringWriter writer = new StringWriter();
		TextWriter old = Console.Out;
		try
		{
			Console.SetOut(writer);
			Foo();
			string expected = @"
TRACE: System.Void CustomInstrumentationSubject::Foo()
TRACE: System.Void CustomInstrumentationSubject::Bar()
";
			Assert.AreEqual(expected.Trim(), writer.ToString().Trim());
		}
		finally
		{
			Console.SetOut(old);
		}
	}
}