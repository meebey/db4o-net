namespace Db4oAdmin.Tests.Core
{
	class PreserveDebugInfoTestCase : SingleResourceTestCase
	{
		protected override string ResourceName
		{
			get { return "PreserveDebugInfoSubject"; }
		}

		protected override string CommandLine
		{
			get
			{
				return "-debug"
					+ " -instrumentation:Db4oAdmin.Tests.Core.TraceInstrumentation,Db4oAdmin.Tests";
			}
		}

		// TODO: remove this method and fix the test
		protected override void InstrumentAssembly(string location)
		{	
			//base.InstrumentAssembly(location);
		}
	}
}
