/* Copyright (C) 2010   Versant Inc.   http://www.db4o.com */

using System;
using System.Reflection;
using System.Reflection.Emit;
using Db4objects.Db4o;
using Db4objects.Db4o.Collections;
using Db4oTool.Tests.Core;
using Db4oUnit;
using Mono.Cecil;
using Mono.Reflection;

namespace Db4oTool.Tests.TA
{
	partial class TACollectionsTestCase : ITestLifeCycle
	{
		private static void AssertInstruction(Instruction actual, OpCode opCode, ConstructorInfo expectedCtor)
		{
			Assert.AreEqual(opCode, actual.OpCode);
			ConstructorInfo actualCtor = (ConstructorInfo) actual.Operand;
			Assert.AreEqual(expectedCtor.DeclaringType.Name, actualCtor.DeclaringType.Name, opCode.ToString());
			Assert.AreEqual(expectedCtor, actualCtor, opCode.ToString());
		}

		protected void AssertWarning(Action<Assembly> action, string expectedWarning)
		{
			string output = InstrumentAndRunInIsolatedAppDomain(action);
			Assert.IsTrue(
				output.Contains(expectedWarning),
				string.Format("Expected warning '{0}' not emitted\r\n\r\nActual instrumentation output:\r\n{1}", expectedWarning, output));
		}

		protected void AssertError(string methodName)
		{
			//CompilerOptions options = CompilerOptions.DefineSymbol(methodName);
			//InstrumentAndRunInIsolatedAppDomain(options);
			Assert.Fail("CHECK FOR ERRORS");
		}

		private static void AssertCast(string testMethodName)
		{
			InstrumentAndRunInIsolatedAppDomain(new CastAsserter(testMethodName).AssertIt);
		}

		protected static MethodBase InstrumentedMethod(Assembly assembly, string name)
		{
			return assembly.GetType(TestResource).GetMethod(name);
		}

		protected static ConstructorInfo ParameterLessContructorFor(Type type)
		{
			ConstructorInfo constructor = type.GetConstructor(new Type[0]);
			Assert.AreEqual(0, constructor.GetParameters().Length);

			return constructor;
		}

		private static string InstrumentAndRunInIsolatedAppDomain(Action<Assembly> action)
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

		public static Instruction FindInstruction(Assembly assembly, string testMethodName, OpCode testInstruction)
		{
			DelegatingILPattern pattern = new DelegatingILPattern(ILPattern.Sequence(ILPattern.Optional(OpCodes.Nop), ILPattern.OpCode(OpCodes.Ldarg_0)));

			MatchContext result = ILPattern.Match(InstrumentedMethod(assembly, testMethodName), pattern);
			Instruction current = pattern.LastMatchingInstruction(result);

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
		
		private const string TestResource = "TACollectionsScenarios";
	}

	[Serializable]
	internal class CastAsserter
	{
		public CastAsserter(string testMethodName)
		{
			_testMethodName = testMethodName;
		}
		
		public void AssertIt(Assembly assembly)
		{
			Instruction current = TACollectionsTestCase.FindInstruction(assembly, _testMethodName, OpCodes.Castclass);

			Type castTarget = ((Type)current.Operand).GetGenericTypeDefinition();
			Assert.AreEqual(typeof(ActivatableList<>), castTarget);
		}
		
		private readonly string _testMethodName;
	}

	[Serializable]
	class IsolatedAppDomainTestRunner
	{
		public IsolatedAppDomainTestRunner(string assemblyPath, Action<Assembly> test)
		{
			_assemblyPath = assemblyPath;
			_test = test;
		}

		public void Run()
		{
			Assembly instrumentedAssembly = Assembly.LoadFrom(_assemblyPath);
			_test(instrumentedAssembly);
		}

		private readonly string _assemblyPath;
		private readonly Action<Assembly> _test;
	}

	class DelegatingILPattern : ILPattern
	{
		public DelegatingILPattern(ILPattern delegating)
		{
			_delegating = delegating;
		}

		public Instruction LastMatchingInstruction(MatchContext context)
		{
			return GetLastMatchingInstruction(context);
		}

		public override void Match(MatchContext context)
		{
			_delegating.Match(context);
		}

		private readonly ILPattern _delegating;
	}
}
