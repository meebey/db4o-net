using System;
using System.IO;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query;
using Db4oUnit;

namespace Db4oAdmin.Tests
{
	public class InstrumentedTestCase : ITestLifeCycle
	{
		protected IObjectContainer _container;
		
		public IObjectContainer Container
		{
			set { _container = value; }
			get { return _container; }
		}
		
		public virtual void SetUp()
		{	
		}
		
		public virtual void TearDown()
		{	
		}
	}
	
	public abstract class AbstractInstrumentationTestCase : ITestSuiteBuilder
	{
		public const string DatabaseFile = "subject.yap";
		
		class InstrumentedTestMethod : TestMethod
		{
			private AbstractInstrumentationTestCase _testCase;

			public InstrumentedTestMethod(AbstractInstrumentationTestCase testCase, object subject, MethodInfo method) : base(subject, method)
			{
				_testCase = testCase;
			}

			override protected void SetUp()
			{
				SetUpAssemblyResolver();
				SetUpContainer();
				base.SetUp();
			}
			
			override protected void TearDown()
			{
				try
				{
					base.TearDown();
				}
				finally
				{
					TearDownContainer();
					TearDownAssemblyResolver();
				}
			}

			private void SetUpAssemblyResolver()
			{
				AppDomain.CurrentDomain.AssemblyResolve += new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			}

			private void TearDownAssemblyResolver()
			{
				AppDomain.CurrentDomain.AssemblyResolve -= new ResolveEventHandler(CurrentDomain_AssemblyResolve);
			}

			private void SetUpContainer()
			{
				((InstrumentedTestCase)GetSubject()).Container = _testCase.OpenDatabase();
			}

			private void TearDownContainer()
			{
				InstrumentedTestCase testCase = (InstrumentedTestCase) GetSubject();
				testCase.Container.Close();
				testCase.Container = null;
			}

			private Assembly CurrentDomain_AssemblyResolve(object sender, ResolveEventArgs args)
			{
				foreach (Assembly assembly in AppDomain.CurrentDomain.GetAssemblies())
				{
					if (assembly.GetName().Name == args.Name) return assembly;
				}
				return null;
			}
		}
		
		class InstrumentationTestSuiteBuilder : ReflectionTestSuiteBuilder
		{
			private AbstractInstrumentationTestCase _testCase;

			public InstrumentationTestSuiteBuilder(AbstractInstrumentationTestCase testCase, Type clazz)
				: base(clazz)
			{
				_testCase = testCase;
			}

			protected override ITest CreateTest(object instance, MethodInfo method)
			{
				return new InstrumentedTestMethod(_testCase, instance, method);
			}
		}

		public TestSuite Build()
		{
			string assemblyPath = EmitAssemblyFromResource();
			InstrumentAssembly(assemblyPath);
			
			Type type = GetTestCaseType(assemblyPath);
			TestSuite suite = type.IsSubclassOf(typeof(InstrumentedTestCase))
			                  	? new InstrumentationTestSuiteBuilder(this, type).Build()
			                  	: new ReflectionTestSuiteBuilder(type).Build();
			return new TestSuite(GetType().FullName, new ITest[] { suite, new VerifyAssemblyTest(assemblyPath)});
		}
		
		protected abstract string ResourceName { get; }

		protected abstract void InstrumentAssembly(string location);

		protected virtual void OnQueryExecution(object sender, QueryExecutionEventArgs args)
		{
			throw new NotImplementedException();
		}

		protected virtual void OnQueryOptimizationFailure(object sender, QueryOptimizationFailureEventArgs args)
		{
			throw new ApplicationException(args.Reason.Message, args.Reason);
		}

		private static string GetResourceAsString(string resourceName)
		{
			using (Stream stream = Assembly.GetExecutingAssembly().GetManifestResourceStream(resourceName))
			{
				return new StreamReader(stream).ReadToEnd();
			}
		}
		
		private Type GetTestCaseType(string assemblyName)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyName);
			return assembly.GetType(ResourceName, true);
		}

		private IObjectContainer OpenDatabase()
		{
			if (File.Exists(DatabaseFile)) File.Delete(DatabaseFile);
			IObjectContainer container = Db4oFactory.OpenFile(DatabaseFile);
			NativeQueryHandler handler = ((ObjectContainerBase)container).GetNativeQueryHandler();
			handler.QueryExecution += OnQueryExecution;
			handler.QueryOptimizationFailure += OnQueryOptimizationFailure;
			return container;
		}
		
		protected string EmitAssemblyFromResource()
		{
			string assemblyName = ResourceName + ".dll";
			CopyParentAssemblyToTemp(typeof(IObjectContainer));
            CopyParentAssemblyToTemp(typeof(Assert));
			CopyParentAssemblyToTemp(GetType());
			string path = Path.Combine(Path.GetTempPath(), assemblyName);
			CompilationServices.EmitAssembly(path,
			                                 GetResourceAsString("Db4oAdmin.Tests.Resources." + ResourceName + ".cs"));
			return path;
		}

		private static void CopyParentAssemblyToTemp(Type type)
		{
			ShellUtilities.CopyFileToFolder(type.Module.FullyQualifiedName, Path.GetTempPath());
		}
	}
}