/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Query;
using Db4oUnit;

namespace Db4oTool.Tests.Core
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

		public IEnumerator GetEnumerator()
		{
			try
			{
				return BuildFromInstrumentedAssembly();
			}
			catch (Exception x)
			{
				return new ITest[] { new FailingTest(TestSuiteLabel, x) }.GetEnumerator();
			}
		}

		private IEnumerator BuildFromInstrumentedAssembly()
		{
			return ProduceTestCases().GetEnumerator();
		}

        private IEnumerable<ITest> ProduceTestCases()
        {
            Assembly[] references = Dependencies;
            foreach (string resource in Resources)
            {
                string assemblyPath = EmitAssemblyFromResource(resource, references);
                Assert.IsTrue(File.Exists(assemblyPath));

            	InstrumentAssembly(assemblyPath);

                Type type = GetTestCaseType(assemblyPath, resource);
                IEnumerable suite = type.IsSubclassOf(typeof(InstrumentedTestCase))
                                    ? new InstrumentationTestSuiteBuilder(this, type)
                                    : new ReflectionTestSuiteBuilder(type);

                foreach (Object test in suite)
                {
                	yield return (ITest)test;
                }

				if (ShouldVerify(resource))
				{
					yield return new VerifyAssemblyTest(assemblyPath);
				}

                references = ArrayServices.Append(references, type.Assembly);
            }
        }

		protected virtual bool ShouldVerify(string resource)
		{
			return true;
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

        protected virtual string EmitAssemblyFromResource(string resource, Assembly[] references)
        {
            CopyDependenciesToTemp();
            string resourceName = ResourceServices.CompleteResourceName(GetType(), resource);
            return CompilationServices.EmitAssemblyFromResource(resourceName, references);
        }

		virtual protected void CopyDependenciesToTemp()
		{
			foreach (Assembly dependency in Dependencies)
			{
				ShellUtilities.CopyAssemblyToTemp(dependency);
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
	}
}