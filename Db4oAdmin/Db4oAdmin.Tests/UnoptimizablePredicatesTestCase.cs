using System;
using System.IO;
using Db4objects.Db4o.Internal.Query;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	// TODO: generate evaluation based queries for unoptimizable predicates
	// so they don't need to be analyzed again in runtime
	// TODO: report unoptimizable through the API as a Warning object
	public class UnoptimizablePredicatesTestCase : AbstractCommandLineInstrumentationTestCase
	{
		protected override string ResourceName
		{
			get { return "UnoptimizablePredicateSubject"; }
		}

		protected override string CommandLine
		{
			get { return "-optimize-predicates"; }
		}

		protected override void CheckInstrumentationOutput(ShellUtilities.ProcessOutput output)
		{
			base.CheckInstrumentationOutput(output);
			Assert.AreEqual(
				"WARNING: Predicate 'ByUpperNameUnoptimizable' could not be optimized. Unsupported expression: candidate.get_Name().ToUpper",
				output.StdErr.Trim());
			Assert.AreEqual("", output.StdOut);
		}

		protected override void OnQueryExecution(object sender, QueryExecutionEventArgs args)
		{
			if (args.Predicate.GetType().Name.EndsWith("Unoptimizable"))
			{
				Assert.AreEqual(QueryExecutionKind.Unoptimized, args.ExecutionKind);
			}
			else
			{
				Assert.AreEqual(QueryExecutionKind.PreOptimized, args.ExecutionKind);
			}
		}

		protected override void OnQueryOptimizationFailure(object sender, QueryOptimizationFailureEventArgs args)
		{
			// do nothing as we expect some predicate to fail
		}
	}
}
