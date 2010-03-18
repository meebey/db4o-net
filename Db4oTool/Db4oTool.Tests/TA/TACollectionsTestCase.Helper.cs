/* Copyright (C) 2010   Versant Inc.   http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Collections;
using Db4oTool.Tests.Core;
using Db4oUnit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oTool.Tests.TA
{
	partial class TACollectionsTestCase : ITestLifeCycle
	{
		internal static void AssertInstruction(Instruction actual, OpCode opCode, IMemberReference expectedCtor)
		{
			Assert.AreEqual(opCode, actual.OpCode);
			MethodReference actualCtor = (MethodReference)actual.Operand;
			Assert.AreEqual(expectedCtor.DeclaringType.Name, actualCtor.DeclaringType.Name, opCode.ToString());
			Assert.AreEqual(expectedCtor, actualCtor.Resolve(), opCode.ToString());
		}

		protected void AssertWarning(Action<AssemblyDefinition> action, string expectedWarning)
		{
			string output = InstrumentAndRunInIsolatedAppDomain(action);
			Assert.IsTrue(
				output.Contains(expectedWarning),
				string.Format("Expected warning '{0}' not emitted\r\n\r\nActual instrumentation output:\r\n{1}", expectedWarning, output));
		}

		protected void InstrumentAssembly(string symbolName)
		{
            CompilationServices.ExtraParameters.Using("/d:" + symbolName, delegate
            {
                InstrumentAssembly(GenerateAssembly(TestResource), true);
            });
		}

		private void AssertSuccessfulCast(string testMethodName)
		{
			InstrumentAndRunInIsolatedAppDomain(new CastAsserter(testMethodName).AssertIt);
		}

		protected static MethodBase InstrumentedMethod(Assembly assembly, string name)
		{
			return assembly.GetType(TestResource).GetMethod(name);
		}

		internal static MethodReference ParameterLessContructorFor(TypeReference type)
		{
			return type.Resolve().Constructors.GetConstructor(false, new Type[0]);
		}

		private string InstrumentAndRunInIsolatedAppDomain(Action<AssemblyDefinition> action)
		{
			AssemblyDefinition assembly = GenerateAssembly(TestResource);
			string instrumentationOutput = InstrumentAssembly(assembly, true);

			AppDomain testDomain = AppDomain.CreateDomain("TACollectionsDomain", AppDomain.CurrentDomain.Evidence, AppDomain.CurrentDomain.SetupInformation);

			try
			{
				testDomain.DoCallBack(new IsolatedAppDomainTestRunner(assembly.MainModule.Image.FileInformation.FullName, action).Run);
			}
			finally
			{
				if (testDomain != null)
				{
					AppDomain.Unload(testDomain);
				}
			}

			return instrumentationOutput;
		}

		public void SetUp()
		{
			ShellUtilities.CopyToTemp(typeof(IObjectContainer).Module.Assembly.Location);
		}

		public void TearDown()
		{
		}

		public static Instruction FindInstruction(AssemblyDefinition assembly, string testMethodName, OpCode testInstruction)
		{
			TypeDefinition testType = assembly.MainModule.Types["TACollectionsScenarios"];
			MethodDefinition testMethod = SingleMethod(testType.Methods.GetMethod(testMethodName));

			Instruction current = testMethod.Body.Instructions[0];

			Instruction instruction = current;
			while (instruction != null && instruction.OpCode != testInstruction)
			{
				instruction = instruction.Next;
			}

			Assert.IsNotNull(instruction);
			Assert.AreEqual(testInstruction, instruction.OpCode);
			current = instruction;
			return current;
		}

		private static MethodDefinition SingleMethod(MethodDefinition[] candidates)
		{
			if (candidates == null || candidates.Length > 1)
			{
				throw new ArgumentException("Must be exactly one method");
			}

			return candidates[0];
		}

		internal static TypeReference Import(AssemblyDefinition assembly, Type type)
		{
			return assembly.MainModule.Import(type);
		}
		
		private const string TestResource = "TACollectionsScenarios";

		private void AssertConstructorInstrumentation(string methodName)
		{
			InstrumentAndRunInIsolatedAppDomain(new ConstructorInstrumentationAsserter(methodName, typeof(ActivatableList<string>)).AssertIt);
		}

		private void AssertConstructorInstrumentationWarning(string methodName)
		{
			InstrumentAndRunInIsolatedAppDomain(new ConstructorInstrumentationAsserter(methodName, typeof(List<string>)).AssertIt);
		}

		private void AssertFailingCast(string testMethodName)
		{
			try
			{
				InstrumentAssembly(testMethodName.ToUpperInvariant());
				Assert.Fail("An exception should be thrown in the call above");
			}
			catch (InvalidOperationException e)
			{
				Assert.IsTrue(e.Message.Contains("Cast to List<T> are allowed only for property access or method call"));
			}
		}
	}

	[Serializable]
	internal class ConstructorInstrumentationAsserter
	{
		public ConstructorInstrumentationAsserter(string methodName, Type type)
		{
			_methodName = methodName;
			_type = type;
		}

		public void AssertIt(AssemblyDefinition assembly)
		{
			Instruction current = TACollectionsTestCase.FindInstruction(assembly, _methodName, OpCodes.Newobj);
			TACollectionsTestCase.AssertInstruction(current, OpCodes.Newobj, TACollectionsTestCase.ParameterLessContructorFor(TACollectionsTestCase.Import(assembly, _type)));
		}

		private readonly string _methodName;
		private readonly Type _type;
	}

	[Serializable]
	internal class CastAsserter
	{
		public CastAsserter(string testMethodName)
		{
			_testMethodName = testMethodName;
		}

		public void AssertIt(AssemblyDefinition assembly)
		{
			Instruction current = TACollectionsTestCase.FindInstruction(assembly, _testMethodName, OpCodes.Castclass);

			TypeReference castTarget = ((TypeReference)current.Operand).Resolve();
			Assert.AreEqual(assembly.MainModule.Import(typeof(ActivatableList<>)).Resolve(), castTarget);
		}

		private readonly string _testMethodName;
	}

	[Serializable]
	class IsolatedAppDomainTestRunner
	{
		public IsolatedAppDomainTestRunner(string assemblyPath, Action<AssemblyDefinition> test)
		{
			_assemblyPath = assemblyPath;
			_test = test;
		}

		public void Run()
		{
			AssemblyDefinition instrumentedAssembly = AssemblyFactory.GetAssembly(_assemblyPath);
			_test(instrumentedAssembly);
		}

		private readonly string _assemblyPath;
		private readonly Action<AssemblyDefinition> _test;
	}
}
