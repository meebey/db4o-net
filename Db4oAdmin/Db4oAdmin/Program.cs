/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Db4oAdmin.Core;
using Db4oAdmin.NQ;
using Db4oAdmin.TA;

namespace Db4oAdmin
{
	public class Program
	{
		public static int Main(string[] args)
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
				if (options.TransparentActivation)
				{
					pipeline.Add(new TAInstrumentation());
				}
				foreach (IAssemblyInstrumentation instr in MapToObjects<IAssemblyInstrumentation>(options.CustomInstrumentations))
				{
					pipeline.Add(instr);
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
		
		private static IEnumerable<T> MapToObjects<T>(IEnumerable<string> typeNames)
		{
			foreach (string typeName in typeNames)
			{
				yield return (T)Activator.CreateInstance(Type.GetType(typeName, true));
			}
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
            foreach (string attribute in options.AttributeFilters)
            {
                configuration.AddFilter(new ByAttributeFilter(attribute));
            }
            foreach (string name in options.NameFilters)
            {
                configuration.AddFilter(new ByNameFilter(name));
            }
            foreach (ITypeFilter filter in MapToObjects<ITypeFilter>(options.CustomFilters))
            {
                configuration.AddFilter(filter);
            }
			return configuration;
		}
	}
}