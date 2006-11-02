namespace Db4oAdmin
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
			foreach (TypeDefinition typedef in module.Types)
			{
				TraceVerbose("Entering type '{0}'", typedef);
				ProcessType(typedef);
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

		private void ProcessMethods(System.Collections.IEnumerable methods)
		{
			foreach (MethodDefinition methodef in methods)
			{
				ProcessMethod(methodef);
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