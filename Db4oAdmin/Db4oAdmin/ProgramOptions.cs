using System.Collections.Generic;
using Mono.GetOptions;

namespace Db4oAdmin
{
	public class ProgramOptions : Options
	{
		private bool _prettyVerbose;
		
		[Option("Optimize predicate subclasses", "optimize-predicates")]
		public bool OptimizePredicates;
		
		[Option("Enable delegate style queries for CompactFramework 2", "cf2-delegates")]
		public bool EnableCF2DelegateQueries;

		[Option("Case sensitive queries", "case-sensitive")]
		public bool CaseSensitive;

		[Option("Verbose operation mode", 'v', "verbose")]
		public bool Verbose;

		[Option("Pretty verbose operation mode", "vv")]
		public bool PrettyVerbose
		{
			get
			{
				return _prettyVerbose;
			}
			
			set
			{
				_prettyVerbose = value;
				Verbose = value;
			}
		}

		[Option("Fake operation mode, assembly won't be written", "fake")]
		public bool Fake;

		public List<string> CustomInstrumentations = new List<string>();
		
		[Option("Custom instrumentation type", "instrumentation", MaxOccurs=-1)]
		public WhatToDoNext CustomInstrumentation(string instrumentation)
		{	
			CustomInstrumentations.Add(instrumentation);
			return WhatToDoNext.GoAhead;
		}

		public string Assembly
		{
			get
			{
				if (RemainingArguments.Length != 1) return null;
				return RemainingArguments[0];
			}
		}
		
		public bool IsValid
		{
			get
			{
				return Assembly != null
				       && (OptimizePredicates
				           || EnableCF2DelegateQueries
				           || CustomInstrumentations.Count > 0);
			}
		}
		
		public ProgramOptions(string[] args) : this()
		{
			ProcessArgs(args);
		}
		
		public ProgramOptions()
		{	
			this.DontSplitOnCommas = true;
		}
	}
}