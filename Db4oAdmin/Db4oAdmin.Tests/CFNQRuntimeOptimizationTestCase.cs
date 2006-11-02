/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */
using Db4objects.Db4o.Inside.Query;

namespace Db4oAdmin.Tests
{
	using System;

	public class CFNQRuntimeOptimizationTestCase : AbstractCommandLineInstrumentationTestCase
	{
		protected override string ResourceName
		{
			get { return "CFNQSubject"; }
		}

		protected override string CommandLine
		{
			get { return "-cf2-delegates"; }
		}

		override protected void OnQueryExecution(object sender, QueryExecutionEventArgs args)
		{
			Type type = typeof(MetaDelegate<object>).GetGenericTypeDefinition();
			if (args.Predicate.GetType().GetGenericTypeDefinition() != type)
			{
				throw new ApplicationException("Query invocation was not instrumented!");
			}
		}
	}
}