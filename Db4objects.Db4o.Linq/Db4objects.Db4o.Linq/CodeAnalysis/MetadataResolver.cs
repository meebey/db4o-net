/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq;
using System.Reflection;

using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Linq.Caching;

using Mono.Cecil;

namespace Db4objects.Db4o.Linq.CodeAnalysis
{
	internal class MetadataResolver : DefaultAssemblyResolver
	{
		public static MetadataResolver Instance = new MetadataResolver();

		private ICachingStrategy<MethodInfo, MethodDefinition> _methodCache;

		private MetadataResolver()
		{
			_methodCache = new SingleItemCachingStrategy<MethodInfo, MethodDefinition>();
		}

		private AssemblyDefinition GetAssembly(Assembly assembly)
		{
			try
			{
				return Resolve(assembly.FullName);
			}
			catch (System.IO.FileNotFoundException)
			{
				var definition = AssemblyFactory.GetAssembly(assembly.ManifestModule.FullyQualifiedName);
				RegisterAssembly(definition);
				return definition;
			}
		}

		private static string GetFullName(Type type)
		{
			if (type.IsNested) return type.FullName.Replace('+', '/');
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

			var definition = _methodCache.Get(method);
			if (definition != null) return definition;

			definition = GetMethod(method);
			_methodCache.Add(method, definition);
			return definition;
		}
	}
}
