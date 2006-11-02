/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */
using System;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.Inside.Query;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oAdmin
{
	class QueryInvocationProcessor
	{
		private InstrumentationContext _context;

		private TypeReference _System_Predicate;
		private TypeReference _System_Object;
		private TypeReference _System_Void;

		private TypeReference _YapStream;
		private MethodReference _NativeQueryHandler_ExecuteInstrumentedDelegateQuery;
		private MethodReference _NativeQueryHandler_ExecuteInstrumentedStaticDelegateQuery;

		private ILPattern _staticFieldPattern = CreateStaticFieldPattern();

		private ILPattern _predicateCreationPattern = ILPattern.Sequence(OpCodes.Newobj, OpCodes.Ldftn);

		public QueryInvocationProcessor(InstrumentationContext context)
		{
			_context = context;
			_YapStream = context.Import(typeof(YapStream));
			_System_Predicate = context.Import(typeof(System.Predicate<>));
			_System_Object = context.Import(typeof(object));
			_System_Void = context.Import(typeof(void));
			_NativeQueryHandler_ExecuteInstrumentedDelegateQuery = context.Import(typeof(NativeQueryHandler).GetMethod("ExecuteInstrumentedDelegateQuery", BindingFlags.Public | BindingFlags.Static));
			_NativeQueryHandler_ExecuteInstrumentedStaticDelegateQuery = context.Import(typeof(NativeQueryHandler).GetMethod("ExecuteInstrumentedStaticDelegateQuery", BindingFlags.Public | BindingFlags.Static));
		}

		public void Process(MethodDefinition parent, Instruction queryInvocation)
		{
			CilWorker worker = parent.Body.CilWorker;
			if (IsCachedStaticFieldPattern(queryInvocation))
			{	
				_context.TraceVerbose("static delegate field pattern found in {0}", parent.Name);
				ProcessCachedStaticFieldPattern(worker, queryInvocation);
			}
			else if (IsPredicateCreationPattern(queryInvocation))
			{
				_context.TraceVerbose("simple delegate pattern found in {0}", parent.Name);
				ProcessPredicateCreationPattern(worker, queryInvocation);
			}
			else
			{
				_context.TraceWarning("Unknown query invocation pattern on method: {0}!", parent);
			}
		}

		private void ProcessPredicateCreationPattern(CilWorker worker, Instruction queryInvocation)
		{
			MethodReference predicateMethod = GetMethodReferenceFromInlinePredicatePattern(queryInvocation);

			Instruction ldftn = GetNthPrevious(queryInvocation, 2);
			worker.InsertBefore(ldftn, worker.Create(OpCodes.Dup));

			worker.InsertBefore(queryInvocation, worker.Create(OpCodes.Ldtoken, predicateMethod));

			// At this point the stack is like this:
			//     runtime method handle, delegate reference, target object, ObjectContainer
			worker.Replace(queryInvocation,
						   worker.Create(OpCodes.Call,
										 InstantiateGenericMethod(
											 _NativeQueryHandler_ExecuteInstrumentedDelegateQuery,
											 GetQueryCallExtent(queryInvocation))));
		}

		private void ProcessCachedStaticFieldPattern(CilWorker worker, Instruction queryInvocation)
		{
			MethodReference predicateMethod = GetMethodReferenceFromStaticFieldPattern(queryInvocation);
			worker.InsertBefore(queryInvocation, worker.Create(OpCodes.Ldtoken, predicateMethod));

			// At this point the stack is like this:
			//     runtime method handle, delegate reference, ObjectContainer
			worker.Replace(queryInvocation,
						   worker.Create(OpCodes.Call,
										 InstantiateGenericMethod(
											 _NativeQueryHandler_ExecuteInstrumentedStaticDelegateQuery,
											 GetQueryCallExtent(queryInvocation))));
		}

		private GenericInstanceType InstantiateGenericType(TypeReference genericTypeDefinition, params TypeReference[] arguments)
		{
			GenericInstanceType type = new GenericInstanceType(genericTypeDefinition);
			foreach (TypeReference argument in arguments)
			{
				type.GenericArguments.Add(argument);
			}
			return type;
		}

		private MethodReference GetMethodReferenceFromInlinePredicatePattern(Instruction queryInvocation)
		{
			return (MethodReference)GetNthPrevious(queryInvocation, 2).Operand;
		}

		private bool IsPredicateCreationPattern(Instruction queryInvocation)
		{
			return _predicateCreationPattern.BackwardsMatch(queryInvocation);
		}

		private MethodReference InstantiateGenericMethod(MethodReference methodReference, TypeReference extent)
		{
			GenericInstanceMethod instance = new GenericInstanceMethod(methodReference);
			instance.GenericArguments.Add(extent);
			return instance;
		}

		private TypeReference GetQueryCallExtent(Instruction queryInvocation)
		{
			GenericInstanceMethod method = (GenericInstanceMethod)queryInvocation.Operand;
			return method.GenericArguments[0];
		}

		private MethodReference GetMethodReferenceFromStaticFieldPattern(Instruction instr)
		{
			return (MethodReference)GetFirstPrevious(instr, OpCodes.Ldftn).Operand;
		}

		private Instruction GetFirstPrevious(Instruction instr, OpCode opcode)
		{
			Instruction previous = instr;
			while (previous != null)
			{
				if (previous.OpCode == opcode) return previous;
				previous = previous.Previous;
			}
			throw new ArgumentException("No previous " + opcode + " instruction found");
		}

		private Instruction GetNthPrevious(Instruction instr, int n)
		{
			Instruction previous = instr;
			for (int i = 0; i < n; ++i)
			{
				previous = previous.Previous;
			}
			return previous;
		}
		
		private static ILPattern CreateStaticFieldPattern()
		{
			// ldsfld (br_s)? stsfld newobj ldftn ldnull (brtrue_s | brtrue) ldsfld
			return ILPattern.Sequence(
				ILPattern.Instruction(OpCodes.Ldsfld),
				ILPattern.Optional(OpCodes.Br_S),
				ILPattern.Instruction(OpCodes.Stsfld),
				ILPattern.Instruction(OpCodes.Newobj),
				ILPattern.Instruction(OpCodes.Ldftn),
				ILPattern.Instruction(OpCodes.Ldnull),
				ILPattern.Alternation(OpCodes.Brtrue, OpCodes.Brtrue_S),
				ILPattern.Instruction(OpCodes.Ldsfld));
		}

		private bool IsCachedStaticFieldPattern(Instruction instr)
		{
			return _staticFieldPattern.BackwardsMatch(instr);
		}
	}
}