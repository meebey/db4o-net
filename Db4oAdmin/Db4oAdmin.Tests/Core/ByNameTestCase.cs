namespace Db4oAdmin.Tests.Core
{
    class ByNameTestCase : AbstractCommandLineInstrumentationTestCase
    {
        protected override string ResourceName
        {
            get { return "ByNameInstrumentationSubject"; }
        }

        protected override string CommandLine
        {
            get
            {
                return "-by-name:F.+o -instrumentation:Db4oAdmin.Tests.Core.TraceInstrumentation,Db4oAdmin.Tests";
            }
        }
    }
}
