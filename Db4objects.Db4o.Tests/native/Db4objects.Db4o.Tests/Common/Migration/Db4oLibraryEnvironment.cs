using System;
using System.IO;
using System.Reflection;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Tests.Util;
using File=Sharpen.IO.File;

namespace Db4objects.Db4o.Tests.Common.Migration
{	
	[Serializable]
	class InvokeInstanceMethod
	{
		private readonly string _typeName;
		private readonly string _methodName;
		private readonly object[] _arguments;

		public InvokeInstanceMethod(string typeName, string methodName, object[] arguments)
		{
			_typeName = typeName;
			_methodName = methodName;
			_arguments = arguments;
		}

		public void Execute()
		{
			Type type = Type.GetType(_typeName);
			MethodInfo method =
				type.GetMethod(_methodName, BindingFlags.IgnoreCase | BindingFlags.Instance | BindingFlags.Public);
			method.Invoke(Activator.CreateInstance(type), _arguments);
		}
	}

	[Serializable]
	class InstallAssemblyResolver
	{
		private readonly string _assembly;
		private readonly string _assemblyName;

		public InstallAssemblyResolver(string assembly)
		{
			_assembly = assembly;
			_assemblyName = Path.GetFileNameWithoutExtension(_assembly);
		}

		public void Execute()
		{
			AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
		}

		Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
		{
			if (SimpleName(args.Name) == _assemblyName) return Assembly.LoadFrom(_assembly);
			return null;
		}

		private string SimpleName(string name)
		{
			return name.Split(',')[0];
		}
	}

	public class Db4oLibraryEnvironment
	{
		private readonly AppDomain _domain;

		private readonly string _fname;

		public Db4oLibraryEnvironment(File file)
		{
			_fname = file.GetAbsolutePath();
			_domain = SetUpDomain();
		}

		private AppDomain SetUpDomain()
		{
			string baseDirectory = IOServices.BuildTempPath("migration-domain-" + Version());
			CopyAssemblies(baseDirectory);
			AppDomain domain = CreateDomain(baseDirectory);
			SetUpAssemblyResolver(domain);
			return domain;
		}

		private void SetUpAssemblyResolver(AppDomain domain)
		{
			domain.DoCallBack(new CrossAppDomainDelegate(new InstallAssemblyResolver(_fname).Execute));
		}

		private static AppDomain CreateDomain(string baseDirectory)
		{
			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationBase = baseDirectory;
			return AppDomain.CreateDomain(Path.GetFileName(baseDirectory), null, setup);
		}

		private void CopyAssemblies(string domainBase)
		{
			IOServices.CopyTo(_fname, domainBase);
			IOServices.CopyEnclosingAssemblyTo(GetType(), domainBase);
			IOServices.CopyEnclosingAssemblyTo(typeof(Db4oUnit.ITest), domainBase);
			IOServices.CopyEnclosingAssemblyTo(typeof(Db4oUnit.Extensions.IDb4oTestCase), domainBase);
		}

		public string Version()
		{
#if NET_2_0
			return System.Reflection.Assembly.ReflectionOnlyLoadFrom(_fname).GetName().Version.ToString();
#else
			return System.Reflection.Assembly.LoadFrom(_fname).GetName().Version.ToString();
#endif
		}

		public void InvokeInstanceMethod(Type type, string methodName, params object[] args)
		{
			_domain.DoCallBack(new CrossAppDomainDelegate(new InvokeInstanceMethod(ReflectPlatform.FullyQualifiedName(type), methodName, args).Execute));
		}
	}
}