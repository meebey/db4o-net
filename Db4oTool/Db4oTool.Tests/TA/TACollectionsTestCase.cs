/* Copyright (C) 2010   Versant Inc.   http://www.db4o.com */

using System.Collections.Generic;
using System.Reflection;
using System.Reflection.Emit;
using Db4objects.Db4o.Collections;
using Mono.Reflection;

namespace Db4oTool.Tests.TA
{
	partial class TACollectionsTestCase : TATestCaseBase
	{
		public void TestMethodWithIListParameter()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(Assembly assembly)
			{
				Instruction instruction = FindInstruction(assembly, "InitInterface", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(typeof(ActivatableList<string>)));
			});
		}

		public void TestMethodWithListParameter()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(Assembly assembly)
			{
				Instruction instruction = FindInstruction(assembly, "InitConcrete", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(typeof(List<string>)));
			});
		}

		public void TestListOfTIsChangedToActivatableCounterpart()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(Assembly assembly)
			{
				Instruction instruction = FindInstruction(assembly, "ParameterLessConstructor", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(typeof(ActivatableList<string>)));
			});
		}

		public void TestAssignmentToConcreteListAreIgnoredAndEmitWarning()
		{
			AssertWarning(	delegate(Assembly assembly)
							{
								Instruction instruction = FindInstruction(assembly, "AssignmentToConcreteList", OpCodes.Newobj);
								AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(typeof(List<string>)));
							},
							"Assignment to concrete collection System.Collections.Generic.List`1<System.String> ignored (offset: 0x06).");
		}

		public void TestMethodReturningConcreteListIsIgnored()
		{
			AssertWarning(	delegate(Assembly assembly)
							{
								Instruction instruction = FindInstruction(assembly, "PublicCreateConcreteList", OpCodes.Newobj);
								AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(typeof(List<string>)));
							},
							"Assignment to concrete collection System.Collections.Generic.List`1<System.String> ignored (offset: 0x06).");
		}

		public void TestLocalsAsIList()
		{
			InstrumentAndRunInIsolatedAppDomain(delegate(Assembly assembly)
			{
				Instruction instruction = FindInstruction(assembly, "LocalsAsIList", OpCodes.Newobj);
				AssertInstruction(instruction, OpCodes.Newobj, ParameterLessContructorFor(typeof(ActivatableList<int>)));
			});
		}

		public void TestMethodReturningNewListAsIList()
		{
		}
		
		public void TestPrivateMethodReturningNewListAsConcreteList()
		{
		}

		public void TestPublicMethodReturningNewListAsConcreteList()
		{
		}

		public void TestCastToListIsReplaced()
		{
			AssertCast("CastFollowedByParameterLessMethod");
			AssertCast("CastFollowedByMethodWithSingleArgument");
		}

		public void _TestCastResultNotConsumedByMethodCall()
		{
			AssertError("CastNotFollowedByConcreteMethodCall");
		}
	}
}
