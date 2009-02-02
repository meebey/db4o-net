/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq;
using System.Reflection;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Linq.Caching;

using Mono.Cecil;

namespace Db4objects.Db4o.Linq.CodeAnalysis
{
	internal class MetadataResolver
	{
		public static MetadataResolver Instance = new MetadataResolver();

		private ICachingStrategy<Assembly, AssemblyDefinition> _assemblyCache;
		private ICachingStrategy<MethodInfo, MethodDefinition> _methodCache;

		private MetadataResolver()
		{
//			_assemblyCache = new SingleItemCachingStrategy<Assembly, AssemblyDefinition>();
//			_methodCache = new SingleItemCachingStrategy<MethodInfo, MethodDefinition>();
			_assemblyCache = new Cache4CachingStrategy<Assembly, AssemblyDefinition>(CacheFactory.New2QXCache(5));
			_methodCache = new Cache4CachingStrategy<MethodInfo, MethodDefinition>(CacheFactory.New2QXCache(5));
		}

		private AssemblyDefinition GetAssembly(Assembly assembly)
		{
			return _assemblyCache.Produce(assembly,
						newAssembly => AssemblyFactory.GetAssembly(newAssembly.ManifestModule.FullyQualifiedName));
		}

		private static string GetFullName(Type type)
		{
			if (type.DeclaringType != null) return type.FullName.Replace('+', '/');
			return type.FullName;
		}

		private TypeDefinition GetType(Type type)
		{
			var assembly = GetAssembly(type.Assembly);
			return assembly.MainModule.Types[GetFullName(type)];
		}

		private static bool ParameterMatch(ParameterDefinition parameter, ParameterInfo info)
		{
			return parameter.ParameterType.FullName == GetFullName(info.ParameterType);
		}

		private static bool ParametersMatch(ParameterDefinitionCollection parameters, ParameterInfo[] infos)
		{
			if (parameters.Count != infos.Length) return false;

			for (int i = 0; i < parameters.Count; i++)
				if (!ParameterMatch(parameters[i], infos[i])) return false;

			return true;
		}

		private static bool MethodMatch(MethodDefinition method, MethodInfo info)
		{
			if (method.Name != info.Name) return false;
			if (method.ReturnType.ReturnType.Name != info.ReturnType.Name) return false;

			return ParametersMatch(method.Parameters, info.GetParameters());
		}

		private MethodDefinition GetMethod(MethodInfo method)
		{
			TypeDefinition type = GetType(method.DeclaringType);

			var matches = from MethodDefinition meth in type.Methods
						  where MethodMatch(meth, method)
						  select meth;

			return matches.FirstOrDefault();
		}

		public MethodDefinition ResolveMethod(MethodInfo method)
		{
			if (method == null) throw new ArgumentNullException("method");

			return _methodCache.Produce(method, GetMethod);
		}
	}
}
