namespace Db4oAdmin.Tests.Core
{
    class ByNotAttributeTestCase : SingleResourceTestCase
	{
		protected override string ResourceName
		{
			get { return "ByNotAttributeInstrumentationSubject"; }
		}

		protected override string CommandLine
		{
			get
			{
				return "-by-attribute:MyCustomAttribute -not -instrumentation:Db4oAdmin.Tests.Core.TraceInstrumentation,Db4oAdmin.Tests";
			}
		}
	}
}
