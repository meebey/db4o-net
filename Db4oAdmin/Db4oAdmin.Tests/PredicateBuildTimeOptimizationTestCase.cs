using Db4objects.Db4o.Inside.Query;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	public class PredicateBuildTimeOptimizationTestCase : AbstractCommandLineInstrumentationTestCase
	{
		protected override string ResourceName
		{
			get { return "PredicateSubject"; }
		}

		protected override string CommandLine
		{
			get { return "-optimize-predicates"; }
		}

		protected override void OnQueryExecution(object sender, QueryExecutionEventArgs args)
		{
			Assert.AreEqual(QueryExecutionKind.PreOptimized, args.ExecutionKind);
		}
	}
}
