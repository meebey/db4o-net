/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using Db4oAdmin.Tests.Core;
using Db4objects.Db4o.Internal.Query;
using Db4oUnit;

namespace Db4oAdmin.Tests.NQ
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
