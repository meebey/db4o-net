/* Copyright (C) 2004 - 2010 Versant Inc.  http://www.db4o.com */
using System.Diagnostics;
using System.IO;
using System.Reflection;
using Db4oTool.Core;
using Db4oTool.Tests.Core;
using Db4oUnit;
using Mono.Cecil;

namespace Db4oTool.Tests.TA
{
	internal abstract class TATestCaseBase : ITestCase
	{
		protected static AssemblyDefinition GenerateAssembly(string resourceName, params Assembly[] references)
		{
			return AssemblyDefinition.ReadAssembly(
						CompilationServices.EmitAssemblyFromResource(
							ResourceServices.CompleteResourceName(
													typeof(TATestCaseBase),
													resourceName), references));
		}

		protected string InstrumentAssembly(AssemblyDefinition testAssembly)
		{
			return InstrumentAssembly(testAssembly, false);
		}

		protected string InstrumentAssembly(AssemblyDefinition testAssembly, bool instrumentCollections)
		{
			StringWriter output = new StringWriter();
			Trace.Listeners.Add(new TextWriterTraceListener(output));

			string assemblyFullPath = testAssembly.MainModule.FullyQualifiedName;
			InstrumentationContext context = new InstrumentationContext(Configuration(assemblyFullPath), testAssembly);

			new Db4oTool.TA.TAInstrumentation(instrumentCollections).Run(context);
			context.SaveAssembly();

			VerifyAssembly(assemblyFullPath);

			return output.ToString();
		}

		protected static void VerifyAssembly(string assemblyPath)
		{
			new VerifyAssemblyTest(assemblyPath).Run();
		}

		protected virtual Configuration Configuration(string assemblyLocation)
		{
			Configuration configuration = new Configuration(assemblyLocation);
			configuration.TraceSwitch.Level = TraceLevel.Info;
 
			return configuration;
		}
	}
}
