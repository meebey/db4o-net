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
			try
			{
				method.Invoke(Activator.CreateInstance(type), _arguments);
			}
			catch (Exception x)
			{
				throw new Exception(x.ToString());
			}
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
			if (SimpleName(args.Name) == _assemblyName)
			{
				return Assembly.LoadFrom(_assembly);
			}
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

		private readonly string _targetAssembly;

		private string _version;

		public Db4oLibraryEnvironment(File file)
		{
			_targetAssembly = file.GetAbsolutePath();
			_domain = SetUpDomain();
			SetUpLegacyAdapter();
		}

		private void SetUpLegacyAdapter()
		{
			if (Version().StartsWith("6")) return;

			string adapterAssembly = Path.Combine(BaseDirectory(), "Db4objects.Db4o.dll");
			new LegacyAdapterEmitter(_targetAssembly, Version()).Emit(adapterAssembly);
		}

		private AppDomain SetUpDomain()
		{
			string baseDirectory = BaseDirectory();
			CopyAssemblies(baseDirectory);
			AppDomain domain = CreateDomain(baseDirectory);
			SetUpAssemblyResolver(domain);
			return domain;
		}

		private string BaseDirectory()
		{
			return IOServices.BuildTempPath("migration-domain-" + Version());
		}

		private void SetUpAssemblyResolver(AppDomain domain)
		{
			domain.DoCallBack(new CrossAppDomainDelegate(new InstallAssemblyResolver(_targetAssembly).Execute));
		}

		private static AppDomain CreateDomain(string baseDirectory)
		{
			AppDomainSetup setup = new AppDomainSetup();
			setup.ApplicationBase = baseDirectory;
			return AppDomain.CreateDomain(Path.GetFileName(baseDirectory), null, setup);
		}

		private void CopyAssemblies(string domainBase)
		{
			IOServices.CopyTo(_targetAssembly, domainBase);
			IOServices.CopyEnclosingAssemblyTo(GetType(), domainBase);
			IOServices.CopyEnclosingAssemblyTo(typeof(Db4oUnit.ITest), domainBase);
			IOServices.CopyEnclosingAssemblyTo(typeof(Db4oUnit.Extensions.IDb4oTestCase), domainBase);
		}

		public string Version()
		{
			if (null != _version) return _version;
			return _version = GetVersion();
		}

		private string GetVersion()
		{
#if NET_2_0
			return System.Reflection.Assembly.ReflectionOnlyLoadFrom(_targetAssembly).GetName().Version.ToString();
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