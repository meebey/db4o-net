/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */

namespace Db4oTool.Core
{
	using Mono.Cecil;

	public abstract class AbstractAssemblyInstrumentation : IAssemblyInstrumentation
	{
		protected InstrumentationContext _context;

		public virtual void Run(InstrumentationContext context)
		{
			_context = context;
			try
			{
				BeforeAssemblyProcessing();
				ProcessAssembly();
				AfterAssemblyProcessing();
			}
			finally
			{
				_context = null;
			}
		}

		protected virtual void BeforeAssemblyProcessing()
		{
		}
		
		protected virtual void AfterAssemblyProcessing()
		{	
		}

		protected virtual void ProcessAssembly()
		{
			foreach (ModuleDefinition module in _context.Assembly.Modules)
			{
				TraceVerbose("Entering module '{0}'", module);
				ProcessModule(module);
				TraceVerbose("Leaving module '{0}'", module);
			}
		}

		protected virtual void ProcessModule(ModuleDefinition module)
		{
			ProcessTypes(module.Types, ProcessType);
		}

		protected virtual void ProcessTypes(TypeDefinitionCollection types, System.Action<TypeDefinition> action)
		{
			ProcessTypes(types, Accept, action);
		}

		protected bool Accept(TypeDefinition type)
		{
			return _context.Accept(type);
		}

		protected static bool NoFiltering(TypeDefinition type)
		{
			return true;
		}

		protected virtual void ProcessTypes(TypeDefinitionCollection types, System.Predicate<TypeDefinition> filter, System.Action<TypeDefinition> action)
		{
			foreach (TypeDefinition typedef in types)
			{
				if (!filter(typedef)) continue;

				TraceVerbose("Entering type '{0}'", typedef);
				action(typedef);
				TraceVerbose("Leaving type '{0}'", typedef);
			}
		}
		
		protected virtual void ProcessType(TypeDefinition type)
		{
			ProcessMethods(type.Methods);
			ProcessMethods(type.Constructors);
		}
		
		protected virtual void ProcessMethod(MethodDefinition method)
		{	
		}

		protected void ProcessMethods(TypeDefinition type)
		{
			ProcessMethods(type.Methods);
		}

		protected void ProcessMethods(System.Collections.IEnumerable methods)
		{
			foreach (MethodDefinition methodef in methods)
			{
				TraceVerbose("Entering method '{0}'", methodef);
				ProcessMethod(methodef);
				TraceVerbose("Leaving method '{0}'", methodef);
			}
		}
		
		protected void TraceWarning(string format, params object[] args)
		{
			_context.TraceWarning(format, args);
		}

		protected void TraceVerbose(string format, params object[] args)
		{
			_context.TraceVerbose(format, args);
		}

		protected void TraceInfo(string format, params object[] args)
		{
			_context.TraceInfo(format, args);
		}

		protected void TraceMethodBody(MethodDefinition m)
		{
			if (_context.TraceSwitch.TraceVerbose)
			{
				TraceVerbose(Cecil.FlowAnalysis.Utilities.Formatter.FormatMethodBody(m));
			}
		}

		public TypeReference Import(System.Type type)
		{
			return _context.Import(type);
		}

		public MethodReference Import(System.Reflection.MethodBase method)
		{
			return _context.Import(method);
		}
	}
}