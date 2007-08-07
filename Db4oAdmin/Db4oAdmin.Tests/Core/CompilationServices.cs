/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

namespace Db4oAdmin.Tests.Core
{
	/// <summary>
	/// Compilation helper.
	/// </summary>
	public class CompilationServices
	{
		public static void EmitAssembly(string assemblyFileName, Assembly[] references, params string[] code)
		{
			string basePath = Path.GetDirectoryName(assemblyFileName);
			CreateDirectoryIfNeeded(basePath);
			CompileFromSource(assemblyFileName, references, code);
		}

		public static void CreateDirectoryIfNeeded(string directory)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}

		static CompilerInfo GetCSharpCompilerInfo()
		{
			return CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(".cs"));
		}

		static CodeDomProvider GetCSharpCodeDomProvider()
		{
			return GetCSharpCompilerInfo().CreateProvider();
		}

		static CompilerParameters CreateDefaultCompilerParameters()
		{
			return GetCSharpCompilerInfo().CreateDefaultCompilerParameters();
		}

		public static void CompileFromSource(string assemblyFName, Assembly[] references, params string[] sources)
		{
			using (CodeDomProvider provider = GetCSharpCodeDomProvider())
			{
				CompilerParameters parameters = CreateDefaultCompilerParameters();
				// TODO: run test cases in both modes (optimized and debug)
				parameters.IncludeDebugInformation = true;
				parameters.OutputAssembly = assemblyFName;
				foreach (Assembly reference in references)
				{
					parameters.ReferencedAssemblies.Add(reference.ManifestModule.FullyQualifiedName);
				}
				CompilerResults results = provider.CompileAssemblyFromSource(parameters, sources);
				if (results.Errors.Count > 0)
				{
					throw new ApplicationException(GetErrorString(results.Errors));
				}
			}
		}

		static string GetErrorString(CompilerErrorCollection errors)
		{
			StringBuilder builder = new StringBuilder();
			foreach (CompilerError error in errors)
			{
				builder.Append(error.ToString());
				builder.Append(Environment.NewLine);
			}
			return builder.ToString();
		}

		private CompilationServices()
		{
		}
	}
}
