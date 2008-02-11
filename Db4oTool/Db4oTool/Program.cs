/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Db4oTool.Core;
using Db4oTool.NQ;
using Db4oTool.TA;

namespace Db4oTool
{
	public class Program
	{
		public static int Main(string[] args)
		{
			Trace.Listeners.Add(new TextWriterTraceListener(Console.Error));

			ProgramOptions options = new ProgramOptions();
			try
			{	
				options.ProcessArgs(args);
				if (!options.IsValid)
				{
					options.DoHelp();
					return -1;
				}

				Run(options);
			}
			catch (Exception x)
			{
				ReportError(options, x);
				return -2;
			}
			return 0;
		}

		private static void Run(ProgramOptions options)
		{
            foreach (string fileName in options.StatisticsFileNames)
            {
                new Statistics().Run(fileName);
            }
            if (options.Assembly == null)
            {
                return;
            }

			using (new CurrentDirectoryAssemblyResolver())
			{
				RunPipeline(options);
			}
		}

		private static void RunPipeline(ProgramOptions options)
		{
			InstrumentationPipeline pipeline = new InstrumentationPipeline(GetConfiguration(options));
			if (options.NQ)
			{
				pipeline.Add(new DelegateOptimizer());
				pipeline.Add(new PredicateOptimizer());
			}
			if (options.TransparentPersistence)
			{
				pipeline.Add(new TAInstrumentation());
			}
			foreach (IAssemblyInstrumentation instr in Factory.Instantiate<IAssemblyInstrumentation>(options.CustomInstrumentations))
			{
				pipeline.Add(instr);
			}
			if (!options.Fake)
			{
				pipeline.Add(new SaveAssemblyInstrumentation());
			}
			pipeline.Run();
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
			configuration.PreserveDebugInfo = options.Debug;
			if (options.Verbose)
			{
				configuration.TraceSwitch.Level = options.PrettyVerbose ? TraceLevel.Verbose : TraceLevel.Info;
			}
            foreach (TypeFilterFactory factory in options.Filters)
            {
                configuration.AddFilter(factory());
            }
			return configuration;
		}
	}
}