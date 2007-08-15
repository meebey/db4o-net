/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using Db4oAdmin.Core;
using Db4oUnit;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Db4oAdmin.Tests.Core
{
	public class ILPatternTestCase : ITestCase
	{
		public void TestSequenceBackwardsMatch()
		{
			ILPattern sequence = ILPattern.Sequence(OpCodes.Stsfld, OpCodes.Ldsfld);

			MethodDefinition method = CreateTestMethod(TestSequence1);
			Instruction lastInstruction = LastInstruction(method);
			ILPattern.MatchContext context = sequence.BackwardsMatch(lastInstruction);
			Assert.IsTrue(context.Success);
			Assert.AreSame(method.Body.Instructions[0], context.Instruction);
		}

		public void TestSequenceIsBackwardsMatch()
		{
			ILPattern sequence = ILPattern.Sequence(OpCodes.Stsfld, OpCodes.Ldsfld);

			Instruction lastInstruction = CreateTestMethodAndReturnLastInstruction(TestSequence1);
			Assert.IsTrue(sequence.IsBackwardsMatch(lastInstruction));

			sequence = ILPattern.Sequence(OpCodes.Ldsfld, OpCodes.Stsfld);
			Assert.IsTrue(!sequence.IsBackwardsMatch(lastInstruction));
		}
		
		public void TestComplexSequenceIsBackwardsMatch()
		{
			ILPattern sequence = ILPattern.Sequence(
				ILPattern.Optional(OpCodes.Ret),
				ILPattern.Instruction(OpCodes.Stsfld),
				ILPattern.Alternation(OpCodes.Ldfld, OpCodes.Ldsfld));

			Instruction lastInstruction = CreateTestMethodAndReturnLastInstruction(TestSequence1);
			Assert.IsTrue(sequence.IsBackwardsMatch(lastInstruction));

			lastInstruction = CreateTestMethodAndReturnLastInstruction(TestSequence2);
			Assert.IsTrue(sequence.IsBackwardsMatch(lastInstruction));
		}

		delegate void CilWorkerAction(CilWorker worker);

		private static Instruction CreateTestMethodAndReturnLastInstruction(CilWorkerAction action)
		{
			return LastInstruction(CreateTestMethod(action));
		}

		private static Instruction LastInstruction(MethodDefinition method)
		{
			return method.Body.Instructions[method.Body.Instructions.Count - 1];
		}

		static MethodDefinition CreateTestMethod(CilWorkerAction action)
		{
			MethodDefinition test = new MethodDefinition("Test", MethodAttributes.Public, null);
			action(test.Body.CilWorker);
			return test;
		}

		private static void TestSequence1(CilWorker worker)
		{
			FieldDefinition blank = new FieldDefinition("Test", null, FieldAttributes.Public);
			worker.Emit(OpCodes.Nop);
			worker.Emit(OpCodes.Ldsfld, blank);
			worker.Emit(OpCodes.Stsfld, blank);
			worker.Emit(OpCodes.Ret);
		}

		private static void TestSequence2(CilWorker worker)
		{
			FieldDefinition blank = new FieldDefinition("Test", null, FieldAttributes.Public);
			worker.Emit(OpCodes.Nop);
			worker.Emit(OpCodes.Ldfld, blank);
			worker.Emit(OpCodes.Stsfld, blank);
			worker.Emit(OpCodes.Ret);
			worker.Emit(OpCodes.Nop);
		}
	}
}
