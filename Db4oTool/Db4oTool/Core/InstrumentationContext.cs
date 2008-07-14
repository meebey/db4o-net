/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Diagnostics;
using System.Reflection;
using Mono.Cecil;

namespace Db4oTool.Core
{
	public class InstrumentationContext
	{
		private AssemblyDefinition _assembly;
		private Configuration _configuration;

        public InstrumentationContext(Configuration configuration, AssemblyDefinition assembly)
        {
            _configuration = configuration;
            SetupAssembly(assembly);

        }

        public InstrumentationContext(Configuration configuration)
		{
			_configuration = configuration;
			SetupAssembly(LoadAssembly());
		}

        private AssemblyDefinition LoadAssembly()
		{
			return AssemblyFactory.GetAssembly(_configuration.AssemblyLocation);
        }

        private void SetupAssembly(AssemblyDefinition assembly)
        {
            _assembly = assembly;
            if (PreserveDebugInfo())
            {
                _assembly.MainModule.LoadSymbols();
            }
            _assembly.MainModule.FullLoad(); // resolves all references
        }

        public Configuration Configuration
		{
			get { return _configuration; }
		}

		public TraceSwitch TraceSwitch
		{
			get { return _configuration.TraceSwitch; }
		}

		public AssemblyDefinition Assembly
		{
			get { return _assembly; }
		}

		public string AssemblyLocation
		{
			get { return _assembly.MainModule.Image.FileInformation.FullName; }
		}

		public TypeReference Import(Type type)
		{
			return _assembly.MainModule.Import(type);
		}

		public MethodReference Import(MethodBase method)
		{
			return _assembly.MainModule.Import(method);
		}

		public void SaveAssembly()
		{
			if (PreserveDebugInfo())
			{
				_assembly.MainModule.SaveSymbols();
			}
			AssemblyFactory.SaveAssembly(_assembly, AssemblyLocation);
		}

		private bool PreserveDebugInfo()
		{
			return _configuration.PreserveDebugInfo;
		}

		public void TraceWarning(string message, params object[] args)
		{
			if (TraceSwitch.TraceWarning)
			{
				Trace.WriteLine(string.Format(message, args));
			}
		}

		public void TraceInfo(string message, params object[] args)
		{
			if (TraceSwitch.TraceInfo)
			{
				Trace.WriteLine(string.Format(message, args));
			}
		}

		public void TraceVerbose(string format, params object[] args)
		{
			if (TraceSwitch.TraceVerbose)
			{
				Trace.WriteLine(string.Format(format, args));
			}
		}

		public bool Accept(TypeDefinition typedef)
		{
			return _configuration.Accept(typedef);
		}

		public bool IsAssemblySigned()
		{
			return _assembly.Name.HasPublicKey;
		}
	}
}