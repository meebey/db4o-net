using System;
using Mono.Cecil;

namespace Db4oTool.Core
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
			return Implements(type, interfaceType.FullName);
		}

		private bool Implements(TypeDefinition type, string interfaceName)
		{
			if (Contains(type.Interfaces, interfaceName)) return true;
			if (null == type.BaseType) return false;

			TypeDefinition baseType = ResolveTypeReference(type.BaseType);
			if (null != baseType) return Implements(baseType, interfaceName);

			return false;
		}

		public TypeDefinition ResolveTypeReference(TypeReference typeRef)
		{
			if (null == typeRef) throw new ArgumentNullException("typeRef");

			TypeDefinition type = typeRef as TypeDefinition;
			if (null != type) return type;

			GenericParameter parameter = typeRef as GenericParameter;
			if (parameter != null) return null;
            
            GenericInstanceType genericType = typeRef as GenericInstanceType;
            if (genericType != null) return ResolveTypeReference(genericType.ElementType);

            AssemblyNameReference assemblyRef = typeRef.Scope as AssemblyNameReference;

			AssemblyDefinition assembly = ResolveAssembly(assemblyRef);
			if (null == assembly) return null;

			return FindType(assembly, typeRef);
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
