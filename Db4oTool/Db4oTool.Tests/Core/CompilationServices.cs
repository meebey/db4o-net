/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.CodeDom.Compiler;
using System.IO;
using System.Reflection;
using System.Text;

namespace Db4oTool.Tests.Core
{
	/// <summary>
	/// Compilation helper.
	/// </summary>
	public class CompilationServices
	{
		public static readonly ContextVariable<bool> Unsafe = new ContextVariable<bool>(false);
		public static readonly ContextVariable<SignConfiguration> KeyFile = new ContextVariable<SignConfiguration>(null);

		public static void EmitAssembly(string assemblyFileName, Assembly[] references, params string[] sourceFiles)
		{
			string basePath = Path.GetDirectoryName(assemblyFileName);
			CreateDirectoryIfNeeded(basePath);
			CompileFromFile(assemblyFileName, references, sourceFiles);
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

		public static void CompileFromFile(string assemblyFName, Assembly[] references, params string[] sourceFiles)
		{
			using (CodeDomProvider provider = GetCSharpCodeDomProvider())
			{
				CompilerParameters parameters = CreateDefaultCompilerParameters();
				// TODO: run test cases in both modes (optimized and debug)
				parameters.IncludeDebugInformation = true;
				parameters.OutputAssembly = assemblyFName;
				
				if (Unsafe.Value) parameters.CompilerOptions = "/unsafe";
				if (KeyFile.Value != null)
				{
					parameters.CompilerOptions += " /keyfile:" + KeyFile.Value.KeyFile;
					parameters.CompilerOptions += " /delaysign" + (KeyFile.Value.DelaySign ? '+' : '-');
				}

				foreach (Assembly reference in references)
				{
					parameters.ReferencedAssemblies.Add(reference.ManifestModule.FullyQualifiedName);
				}
				CompilerResults results = provider.CompileAssemblyFromFile(parameters, sourceFiles);
                if (ContainsErrors(results.Errors))
                {
					throw new ApplicationException(GetErrorString(results.Errors));
				}
			}
		}

        private static Boolean ContainsErrors(CompilerErrorCollection errors)
        {
            foreach (CompilerError error in errors)
            {
                if (!error.IsWarning)
                {
                    return true;
                }
            }
            return false;
        }

        public static string EmitAssemblyFromResource(string resourceName, params Assembly[] references)
        {
            string assemblyFileName = Path.Combine(ShellUtilities.GetTempPath(), resourceName + ".dll");
            string sourceFileName = Path.Combine(ShellUtilities.GetTempPath(), resourceName);
            File.WriteAllText(sourceFileName, ResourceServices.GetResourceAsString(resourceName));
            DeleteAssemblyAndPdb(assemblyFileName);
            EmitAssembly(assemblyFileName, references, sourceFileName);
            return assemblyFileName;
        }

        private static void DeleteAssemblyAndPdb(string path)
        {
            ShellUtilities.DeleteFile(Path.ChangeExtension(path, ".pdb"));
            ShellUtilities.DeleteFile(path);
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
