/* Copyright (C) 2010   Versant Inc.   http://www.db4o.com */

using System;
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
		private static void AssertInstruction(Instruction actual, OpCode opCode, IMemberReference expectedCtor)
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

		private void AssertCast(string testMethodName)
		{
			InstrumentAndRunInIsolatedAppDomain(new CastAsserter(testMethodName).AssertIt);
		}

		protected static MethodBase InstrumentedMethod(Assembly assembly, string name)
		{
			return assembly.GetType(TestResource).GetMethod(name);
		}

		private static MethodReference ParameterLessContructorFor(TypeReference type)
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

		//private static void AssertConcreteMethodCallIsRedirected(Type concreteType, Type replacement, Instruction instruction)
		//{
		//    Assert.AreEqual(OpCodes.Call, instruction.OpCode);
		//    MethodInfo method = (MethodInfo) instruction.Operand;

		//    Assert.AreEqual(replacement, method.DeclaringType.GetGenericTypeDefinition());

		//    Assert.IsSmallerOrEqual(1, method.GetParameters().Length);

		//    Type @interface = method.GetParameters()[0].ParameterType;
		//    Assert.IsTrue(@interface.IsInterface);
		//    Assert.IsTrue(@interface.IsAssignableFrom(concreteType));

		//    ParameterInfo[] parameters = method.GetParameters();
		//    Type[] parameterTypes = new Type[parameters.Length - 1];
		//    for(int i = 0; i < parameterTypes.Length; i++)
		//    {
		//        parameterTypes[i] = parameters[i + 1].ParameterType;
		//    }

		//    MethodInfo found = concreteType.GetMethod(method.Name, BindingFlags.Public | BindingFlags.Instance | BindingFlags.DeclaredOnly, null, parameterTypes, null);
		//    Assert.IsNotNull(found, string.Format("Method {0} not found on type {1}", method.Name, concreteType.Name));
		//}

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

		private static TypeReference Import(AssemblyDefinition assembly, Type type)
		{
			return assembly.MainModule.Import(type);
		}
		
		private const string TestResource = "TACollectionsScenarios";

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

			TypeReference castTarget = ((TypeReference) current.Operand).Resolve();
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
