using System;
using Db4oAdmin.Core;
using Mono.Cecil;

namespace Db4oAdmin.TA
{
	public class CecilReflector
	{
		private readonly InstrumentationContext _context;
		private readonly RelativeAssemblyResolver _resolver;

		public CecilReflector(InstrumentationContext context)
		{
			_context = context;
			_resolver = new RelativeAssemblyResolver(_context);
		}

		public bool Implements(TypeDefinition type, Type interfaceType)
		{
			return Contains(type.Interfaces, interfaceType.FullName);
		}

		public TypeDefinition ResolveTypeReference(TypeReference typeRef)
		{
			TypeDefinition type = typeRef as TypeDefinition;
			if (null != type) return type;

			AssemblyNameReference assemblyRef = typeRef.Scope as AssemblyNameReference;
			if (IsSystemAssembly(assemblyRef)) return null;

			AssemblyDefinition assembly = ResolveAssembly(assemblyRef);
			if (null == assembly) return null;

			return FindType(assembly, typeRef);
		}

		private bool IsSystemAssembly(AssemblyNameReference assemblyRef)
		{
			switch (assemblyRef.Name)
			{
				case "mscorlib":
				case "corlib":
				case "System":
					return true;
			}
			return false;
		}

		private AssemblyDefinition ResolveAssembly(AssemblyNameReference assemblyRef)
		{
			return _resolver.Resolve(assemblyRef);
		}

		private TypeDefinition FindType(AssemblyDefinition assembly, TypeReference typeRef)
		{
			foreach (ModuleDefinition m in assembly.Modules)
			{
				foreach (TypeDefinition t in m.Types)
				{
					if (t.FullName == typeRef.FullName) return t;
				}
			}
			return null;
		}

		private static bool Contains(InterfaceCollection collection, string fullName)
		{
			foreach (TypeReference typeRef in collection)
			{
				if (typeRef.FullName == fullName) return true;
			}
			return false;
		}
	}
}
