/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */
using System;
using System.Diagnostics;

namespace Db4oAdmin
{
	public class Program
	{
		static int Main(string[] args)
		{	
			ProgramOptions options = new ProgramOptions(args);
			if (!options.IsValid)
			{
				options.DoHelp();
				return -1;
			}

			return Run(options);
		}

		private static int Run(ProgramOptions options)
		{
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));
			try
			{
				InstrumentationPipeline pipeline = new InstrumentationPipeline(GetConfiguration(options));
				if (options.OptimizePredicates)
				{
					pipeline.Add(new PredicateOptimizer());
				}
				if (options.EnableCF2DelegateQueries)
				{
					pipeline.Add(new CFNQEnabler());
				}
				foreach (string customInstrumentation in options.CustomInstrumentations)
				{
					pipeline.Add((IAssemblyInstrumentation)Activator.CreateInstance(Type.GetType(customInstrumentation, true)));
				}
				if (!options.Fake)
				{
					pipeline.Add(new SaveAssemblyInstrumentation());
				}
				pipeline.Run();
			}
			catch (Exception x)
			{
				ReportError(options, x);
				return -2;
			}
			return 0;
		}

		private static void ReportError(ProgramOptions options, Exception x)
		{
			if (options.Verbose)
			{
				Console.WriteLine(x);
			}
			else
			{
				Console.WriteLine(x.Message);
			}
		}

		private static Configuration GetConfiguration(ProgramOptions options)
		{
			Configuration configuration = new Configuration(options.Assembly);
			configuration.CaseSensitive = options.CaseSensitive;
			if (options.Verbose)
			{
				configuration.TraceSwitch.Level = options.PrettyVerbose ? TraceLevel.Verbose : TraceLevel.Info;
			}
			return configuration;
		}
	}
}