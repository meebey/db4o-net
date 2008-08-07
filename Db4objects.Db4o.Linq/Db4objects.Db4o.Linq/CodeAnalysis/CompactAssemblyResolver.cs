/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.IO;

using Mono.Cecil;

#if CF_3_5

namespace Db4objects.Db4o.Linq.CodeAnalysis
{
	class CompactAssemblyResolver : IAssemblyResolver
	{
		IAssemblyResolver _resolver;
		AssemblyDefinition _corlib;

		public static readonly CompactAssemblyResolver Instance = new CompactAssemblyResolver();

		public CompactAssemblyResolver()
		{
			var resolver = new DefaultAssemblyResolver();
			AddSearchDirectory(resolver, typeof(CompactAssemblyResolver));
			_resolver = resolver;
		}

		static void AddSearchDirectory(DefaultAssemblyResolver resolver, Type type)
		{
			resolver.AddSearchDirectory(Path.GetDirectoryName(type.Module.FullyQualifiedName));
		}

		public AssemblyDefinition Resolve (AssemblyNameReference name)
		{
			if (name.Name == "mscorlib")
			{
				if (_corlib == null) _corlib = AssemblyFactory.GetAssembly(typeof(object).Module.FullyQualifiedName);
				return _corlib;
			}

			return _resolver.Resolve(name);
		}

		public AssemblyDefinition Resolve (string fullName)
		{
			return _resolver.Resolve(fullName);
		}
	}
}

#endif
