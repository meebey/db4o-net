using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Text;
using Db4objects.Db4o;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	/// <summary>
	/// Compilation helper.
	/// </summary>
	public class CompilationServices
	{
		public static void EmitAssembly(string assemblyFileName, params string[] code)
		{
			string basePath = Path.GetDirectoryName(assemblyFileName);
			CreateDirectoryIfNeeded(basePath);
			CompileFromSource(assemblyFileName, code);
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

		public static void CompileFromSource(string assemblyFName, params string[] sources)
		{
			using (CodeDomProvider provider = GetCSharpCodeDomProvider())
			{
				CompilerParameters parameters = CreateDefaultCompilerParameters();
				// TODO: run test cases in both modes (optimized and debug)
				parameters.IncludeDebugInformation = true;
				parameters.OutputAssembly = assemblyFName;
				parameters.ReferencedAssemblies.Add(typeof(IObjectContainer).Module.FullyQualifiedName);
				parameters.ReferencedAssemblies.Add(typeof(CompilationServices).Module.FullyQualifiedName);
                parameters.ReferencedAssemblies.Add(typeof(Assert).Module.FullyQualifiedName);
				
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
