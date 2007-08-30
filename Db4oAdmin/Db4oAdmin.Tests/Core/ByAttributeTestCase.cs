namespace Db4oAdmin.Tests.Core
{
	class ByAttributeTestCase : AbstractCommandLineInstrumentationTestCase
	{
		protected override string ResourceName
		{
			get { return "ByAttributeInstrumentationSubject"; }
		}

		protected override string CommandLine
		{
			get
			{
				return "-by-attribute:MyCustomAttribute -instrumentation:Db4oAdmin.Tests.Core.TraceInstrumentation,Db4oAdmin.Tests";
			}
		}
	}
}
