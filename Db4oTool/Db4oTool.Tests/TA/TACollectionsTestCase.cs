/* Copyright (C) 2010   Versant Inc.   http://www.db4o.com */

using System;
using System.Collections.Generic;
using Db4objects.Db4o.Collections;
using Db4oTool.Core;
using Db4oUnit;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oTool.Tests.TA
{
	partial class TACollectionsTestCase : TATestCaseBase
	{
		public void TestMethodWithIListParameter()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(AssemblyDefinition assembly)
			{
				Instruction instruction = FindInstruction(assembly, "InitInterface", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(Import(assembly, typeof(ActivatableList<string>))));
			});
		}

		public void TestMethodWithListParameter()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(AssemblyDefinition assembly)
			{
				Instruction instruction = FindInstruction(assembly, "InitConcrete", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(Import(assembly, typeof(List<string>))));
			});
		}

		public void TestListOfTIsChangedToActivatableCounterpart()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(AssemblyDefinition assembly)
			{
				Instruction instruction = FindInstruction(assembly, "ParameterLessConstructor", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(Import(assembly, typeof(ActivatableList<string>))));
			});
		}

		public void TestAssignmentToConcreteListAreIgnoredAndEmitWarning()
		{
			AssertWarning(	delegate(AssemblyDefinition assembly)
							{
								Instruction instruction = FindInstruction(assembly, "AssignmentToConcreteList", OpCodes.Newobj);
								AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(Import(assembly,typeof(List<string>))));
							},
							"Assignment to concrete collection System.Collections.Generic.List`1<System.String> ignored (offset: 0x06).");
		}

		public void TestMethodReturningConcreteListIsIgnored()
		{
			AssertWarning(	delegate(AssemblyDefinition assembly)
							{
								Instruction instruction = FindInstruction(assembly, "PublicCreateConcreteList", OpCodes.Newobj);
								AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(Import(assembly,typeof(List<string>))));
							},
							"Assignment to concrete collection System.Collections.Generic.List`1<System.String> ignored (offset: 0x06).");
		}

		public void TestLocalsAsIList()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(AssemblyDefinition assembly)
			{
				Instruction instruction = FindInstruction(assembly, "LocalsAsIList", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(Import(assembly,typeof(ActivatableList<int>))));
			});
		}

		public void TestMethodReturningNewListAsIList()
		{
            InstrumentAndRunInIsolatedAppDomain(delegate(AssemblyDefinition assembly)
            {
				Instruction instruction = FindInstruction(assembly, "CreateList", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(Import(assembly, typeof(ActivatableList<DateTime>))));
            });
		}

		public void TestCastToListIsReplaced()
		{
			AssertCast("CastFollowedByParameterLessMethod");
			AssertCast("CastFollowedByMethodWithSingleArgument");
		}

		public void TestCastResultNotConsumedByMethodCall()
		{
            try
            {
                InstrumentAssembly("CastNotFollowedByConcreteMethodCall".ToUpperInvariant());
                Assert.Fail("An exception should be thrown in the call above");
            }
            catch(InvalidOperationException e)
            {
                Assert.IsTrue(e.Message.Contains("Cast to List<T> are allowed only for property access or method call"));
            }
		}

        protected override Configuration Configuration(string assemblyLocation)
        {
            Configuration conf = base.Configuration(assemblyLocation);
            conf.PreserveDebugInfo = true;
            return conf;
        }
	}
}
