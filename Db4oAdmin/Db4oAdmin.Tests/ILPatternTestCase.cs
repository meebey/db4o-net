using Db4oUnit;
using Mono.Cecil.Cil;
using Mono.Cecil;

namespace Db4oAdmin.Tests
{
	public class ILPatternTestCase : ITestCase
	{
		public void TestSequenceBackwardsMatch()
		{
			ILPattern sequence = ILPattern.Sequence(OpCodes.Stsfld, OpCodes.Ldsfld);

			Instruction lastInstruction = CreateTestMethodAndReturnLastInstruction(TestSequence1);
			Assert.IsTrue(sequence.BackwardsMatch(lastInstruction));

			sequence = ILPattern.Sequence(OpCodes.Ldsfld, OpCodes.Stsfld);
			Assert.IsTrue(!sequence.BackwardsMatch(lastInstruction));
		}
		
		public void TestComplexSequenceBackwardsMatch()
		{
			ILPattern sequence = ILPattern.Sequence(
				ILPattern.Optional(OpCodes.Ret),
				ILPattern.Instruction(OpCodes.Stsfld),
				ILPattern.Alternation(OpCodes.Ldfld, OpCodes.Ldsfld));

			Instruction lastInstruction = CreateTestMethodAndReturnLastInstruction(TestSequence1);
			Assert.IsTrue(sequence.BackwardsMatch(lastInstruction));

			lastInstruction = CreateTestMethodAndReturnLastInstruction(TestSequence2);
			Assert.IsTrue(sequence.BackwardsMatch(lastInstruction));
		}

		delegate void CilWorkerAction(CilWorker worker);

		private static Instruction CreateTestMethodAndReturnLastInstruction(CilWorkerAction action)
		{
			MethodDefinition method = CreateTestMethod(action);
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
			worker.Emit(OpCodes.Ldsfld, blank);
			worker.Emit(OpCodes.Stsfld, blank);
			worker.Emit(OpCodes.Ret);
		}

		private static void TestSequence2(CilWorker worker)
		{
			FieldDefinition blank = new FieldDefinition("Test", null, FieldAttributes.Public);
			worker.Emit(OpCodes.Ldfld, blank);
			worker.Emit(OpCodes.Stsfld, blank);
			worker.Emit(OpCodes.Ret);
			worker.Emit(OpCodes.Nop);
		}
	}
}
