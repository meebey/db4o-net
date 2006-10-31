namespace Db4objects.Db4o.Tests.Util
{
#if !CF_1_0 && !CF_2_0
	using System;
	using System.CodeDom.Compiler;
	using System.IO;
	using System.Text;

	/// <summary>
	/// Compilation helper.
	/// </summary>
	public class CompilationServices
	{
		public static void EmitAssembly(string assemblyFileName, params string[] code)
		{
			string basePath = Path.GetDirectoryName(assemblyFileName);
			CreateDirectoryIfNeeded(basePath);

			string[] sourceFiles = WriteSourceFiles(Path.GetTempPath(), code);
			CompileFiles(assemblyFileName, sourceFiles);
		}

		public static void CreateDirectoryIfNeeded(string directory)
		{
			if (!Directory.Exists(directory))
			{
				Directory.CreateDirectory(directory);
			}
		}

		static string[] WriteSourceFiles(string basePath, string[] code)
		{
			string[] sourceFiles = new string[code.Length];
			for (int i=0; i<code.Length; ++i)
			{
				string sourceFile = Path.Combine(basePath, "source" + i + ".cs");
				WriteFile(sourceFile, code[i]);
				sourceFiles[i] = sourceFile;
			}
			return sourceFiles;
		}

		static void WriteFile(string fname, string contents)
		{
			using (StreamWriter writer = new StreamWriter(fname))
			{
				writer.Write(contents);
			}
		}

#if NET_2_0
		static CompilerInfo GetCSharpCompilerInfo()
		{
			return CodeDomProvider.GetCompilerInfo(CodeDomProvider.GetLanguageFromExtension(".cs"));
		}
#endif

		static CodeDomProvider GetCSharpCodeDomProvider()
		{
#if NET_2_0
			return GetCSharpCompilerInfo().CreateProvider();
#else
			return (CodeDomProvider)j4o.lang.Class.ForName("Microsoft.CSharp.CSharpCodeProvider, System").NewInstance();
#endif
		}

		static CompilerParameters CreateDefaultCompilerParameters()
		{
#if NET_2_0
			return GetCSharpCompilerInfo().CreateDefaultCompilerParameters();
#else
			return new CompilerParameters();
#endif
		}

		public static void CompileFiles(string assemblyFName, string[] files)
		{	
			using (CodeDomProvider provider = GetCSharpCodeDomProvider())
			{
				CompilerParameters parameters = CreateDefaultCompilerParameters();
				parameters.IncludeDebugInformation = false;
				parameters.OutputAssembly = assemblyFName;
				parameters.ReferencedAssemblies.Add(typeof(Db4o).Module.FullyQualifiedName);
				parameters.ReferencedAssemblies.Add(typeof(CompilationServices).Module.FullyQualifiedName);

				ICodeCompiler compiler = provider.CreateCompiler();
				CompilerResults results = compiler.CompileAssemblyFromFileBatch(parameters, files);
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
#endif
}
