/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query;
using Db4oUnit;

namespace Db4oAdmin.Tests.Core
{
	public abstract class AbstractInstrumentationTestCase : ITestSuiteBuilder
	{
		public const string DatabaseFile = "subject.db4o";
		
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
			try
			{
				return BuildFromInstrumentedAssembly();
			}
			catch (Exception x)
			{
				return new TestSuite(TestSuiteLabel, new ITest[] { new FailingTest(TestSuiteLabel, x) });
			}
		}

		private TestSuite BuildFromInstrumentedAssembly()
		{
            return new TestSuite(TestSuiteLabel, new List<ITest>(ProduceTestCases()).ToArray());
		}

        private IEnumerable<ITest> ProduceTestCases()
        {
            Assembly[] references = Dependencies;
            foreach (string resource in Resources)
            {
                string assemblyPath = EmitAssemblyFromResource(resource, references);
                Assert.IsTrue(File.Exists(assemblyPath));

            	string instrumentedAssembly = CopyAssemblyAndPdbToTemp(assemblyPath);
				InstrumentAssembly(instrumentedAssembly);

                Type type = GetTestCaseType(instrumentedAssembly, resource);
                TestSuite suite = type.IsSubclassOf(typeof(InstrumentedTestCase))
                                    ? new InstrumentationTestSuiteBuilder(this, type).Build()
                                    : new ReflectionTestSuiteBuilder(type).Build();
                yield return suite;
                yield return new VerifyAssemblyTest(instrumentedAssembly);

                references = ArrayServices.Append(references, type.Assembly);
            }
        }

		private string CopyAssemblyAndPdbToTemp(string path)
		{	
			CopyToTemp(Path.ChangeExtension(path, ".pdb"));
			return CopyToTemp(path);
		}

		protected string TestSuiteLabel
		{
			get { return GetType().FullName;  }
		}
		
		protected abstract string[] Resources { get; }

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
		
		private Type GetTestCaseType(string assemblyName, string resource)
		{
			Assembly assembly = Assembly.LoadFrom(assemblyName);
			return assembly.GetType(resource, true);
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
		
		protected string EmitAssemblyFromResource(string resource, Assembly[] references)
		{
			CopyDependenciesToTemp();
			string assemblyFileName = Path.Combine(Path.Combine(Path.GetTempPath(), "build"), resource + ".dll");
			string resourceName = GetType().Namespace + ".Resources." + resource + ".cs";
			string sourceFileName = Path.Combine(Path.GetTempPath(), resourceName);
			File.WriteAllText(sourceFileName, GetResourceAsString(resourceName));
			CompilationServices.EmitAssembly(assemblyFileName, references, sourceFileName);
			return assemblyFileName;
		}

		virtual protected void CopyDependenciesToTemp()
		{
			foreach (Assembly dependency in Dependencies)
			{
				CopyAssemblyToTemp(dependency);
			}
		}

		protected virtual Assembly[] Dependencies
		{
			get
			{
				return new Assembly[]
					{
						typeof(IObjectContainer).Assembly,
						typeof(Assert).Assembly,
						GetType().Assembly
					};
			}
		}

		protected static void CopyParentAssemblyToTemp(Type type)
		{
			CopyAssemblyToTemp(type.Assembly);
		}

		private static void CopyAssemblyToTemp(Assembly assembly)
		{
			CopyToTemp(assembly.ManifestModule.FullyQualifiedName);
		}

		private static string CopyToTemp(string fname)
		{
			return ShellUtilities.CopyFileToFolder(fname, Path.GetTempPath());
		}
	}
}