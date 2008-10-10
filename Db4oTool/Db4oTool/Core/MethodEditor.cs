using System;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oTool.Core
{
	class MethodEditor
	{
		private readonly CilWorker _worker;
		private readonly MethodBody _body;

		public MethodEditor(MethodDefinition method)
		{	
			_body = method.Body;
			_worker = method.Body.CilWorker;
		}

		public Instruction Create(OpCode opcode, VariableDefinition variable)
		{
			return _worker.Create(opcode, variable);
		}

		public void InsertBefore(Instruction target, Instruction instruction)
		{
			_worker.InsertBefore(target, instruction);
			UpdateInstructionReferences(target, instruction);
		}

		private void UpdateInstructionReferences(Instruction oldTarget, Instruction newTarget)
		{
			UpdateInstructionReferences(_body.Instructions, oldTarget, newTarget);
			UpdateInstructionReferences(_body.ExceptionHandlers, oldTarget, newTarget);
		}

		private static void UpdateInstructionReferences(InstructionCollection collection, Instruction oldTarget, Instruction newTarget)
		{
			foreach (Instruction instr in collection)
			{
				if (instr.OpCode == OpCodes.Switch)
				{
					Instruction[] labels = (Instruction[])instr.Operand;
					ReplaceAll(labels, oldTarget, newTarget);
				}
				else if (instr.Operand == oldTarget)
				{
					instr.Operand = newTarget;
				}
			}
		}

		private static void UpdateInstructionReferences(ExceptionHandlerCollection handlers, Instruction oldTarget, Instruction newTarget)
		{
			foreach (ExceptionHandler handler in handlers)
			{
				if (handler.TryEnd == oldTarget)
				{
					handler.TryEnd = newTarget;
				}
				if (handler.TryStart == oldTarget)
				{
					handler.TryStart = newTarget;
				}
				else if (handler.HandlerStart == oldTarget)
				{
					handler.HandlerStart = newTarget;
				}
				else if (handler.FilterStart == oldTarget)
				{
					handler.FilterStart = newTarget;
				}
			}
		}

		private static void ReplaceAll(Instruction[] labels, Instruction oldTarget, Instruction newTarget)
		{
			for (int i = 0; i < labels.Length; ++i)
			{
				if (labels[i] == oldTarget)
				{
					labels[i] = newTarget;
				}
			}
		}

		internal Instruction Create(OpCode opCode, ParameterDefinition parameterDefinition)
		{
			return _worker.Create(opCode, parameterDefinition);
		}

		public Instruction Create(OpCode opCode)
		{
			return _worker.Create(opCode);
		}

		internal Instruction Create(OpCode opCode, int value)
		{
			return _worker.Create(opCode, value);
		}

		public Instruction Create(OpCode opCode, MethodReference reference)
		{
			return _worker.Create(opCode, reference);
		}

		public VariableDefinition AddVariable(TypeReference type)
		{
			_body.InitLocals = true;

			VariableDefinition variable = new VariableDefinition(type);
			_body.Variables.Add(variable);

			return variable;
		}
	}
}
