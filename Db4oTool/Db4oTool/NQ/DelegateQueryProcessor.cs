/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */
using System;
using System.Reflection;
using Db4objects.Db4o;
using Db4objects.Db4o.NativeQueries.Expr;
using Db4objects.Db4o.Query;
using Db4oTool.Core;
using Db4objects.Db4o.Internal.Query;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TypeAttributes=Mono.Cecil.TypeAttributes;

namespace Db4oTool.NQ
{
	class DelegateQueryProcessor
	{
		private InstrumentationContext _context;

		private ILPattern _staticFieldPattern = CreateStaticFieldPattern();

		private ILPattern _predicateCreationPattern = ILPattern.Sequence(OpCodes.Newobj, OpCodes.Ldftn);

		private readonly DelegateOptimizer _optimizer;

		public DelegateQueryProcessor(InstrumentationContext context, DelegateOptimizer optimizer)
		{
			_context = context;
			_optimizer = optimizer;
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

//			Instruction ldftn = GetNthPrevious(queryInvocation, 2);
//			worker.InsertBefore(ldftn, worker.Create(OpCodes.Dup));
//
//			worker.InsertBefore(queryInvocation, worker.Create(OpCodes.Ldtoken, predicateMethod));
//
//			// At this point the stack is like this:
//			//     runtime method handle, delegate reference, target object, ObjectContainer
//			worker.Replace(queryInvocation,
//			               worker.Create(OpCodes.Call,
//			                             InstantiateGenericMethod(
//			                             	_NativeQueryHandler_ExecuteInstrumentedDelegateQuery,
//			                             	GetQueryCallExtent(queryInvocation))));
		}

		private void ProcessCachedStaticFieldPattern(CilWorker worker, Instruction queryInvocation)
		{
			MethodReference predicateReference = GetMethodReferenceFromStaticFieldPattern(queryInvocation);
			MethodDefinition predicateMethod = Resolve(predicateReference);

			IExpression expression = _optimizer.GetExpression(predicateMethod);
			if (null == expression) return;

			TypeDefinition syntheticPredicate = NewSyntheticPredicateFor(predicateMethod);
			_optimizer.OptimizePredicate(syntheticPredicate, predicateMethod, expression);

			Instruction newObj = worker.Create(OpCodes.Newobj, FirstConstructor(syntheticPredicate));
			worker.Replace(queryInvocation.Previous, newObj);

			ReplaceByExecuteEnhancedFilter(queryInvocation);

			Console.WriteLine(Cecil.FlowAnalysis.Utilities.Formatter.FormatMethodBody(worker.GetBody().Method));
		}

		private TypeDefinition NewSyntheticPredicateFor(MethodDefinition predicate)
		{
			ModuleDefinition module = MainModule();
			TypeDefinition type = new TypeDefinition("Db4o$Predicate$" + module.Types.Count, predicate.DeclaringType.Namespace, TypeAttributes.Sealed|TypeAttributes.NotPublic, Import(typeof(object)));

			type.Constructors.Add(CreateDefaultConstructor());
		
			module.Types.Add(type);

			return type;
		}

		private MethodDefinition CreateDefaultConstructor()
		{
			MethodDefinition ctor = new MethodDefinition(MethodDefinition.Ctor,
				Mono.Cecil.MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName | Mono.Cecil.MethodAttributes.Public,
				Import(typeof(void)));
			CilWorker worker = ctor.Body.CilWorker;
			worker.Emit(OpCodes.Ldarg_0);
			worker.Emit(OpCodes.Call, DefaultObjectConstructor());
			worker.Emit(OpCodes.Ret);
			return ctor;
		}

		private MethodReference DefaultObjectConstructor()
		{
			return _context.Import(typeof(object).GetConstructors()[0]);
		}

		private TypeReference Import(Type type)
		{
			return _context.Import(type);
		}

		private ModuleDefinition MainModule()
		{
			return _context.Assembly.MainModule;
		}

		private void ReplaceByExecuteEnhancedFilter(Instruction queryInvocation)
		{
			queryInvocation.OpCode = OpCodes.Call;
			queryInvocation.Operand = InstantiateGenericMethod(ExecuteEnhancedFilterMethod(), GetQueryCallExtent(queryInvocation));
		}

		private MethodReference FirstConstructor(TypeDefinition type)
		{
			return type.Constructors[0];
		}

		private MethodReference ExecuteEnhancedFilterMethod()
		{
			return _context.Import(typeof(NativeQueryHandler).GetMethod("ExecuteEnhancedFilter", new Type[] { typeof(IObjectContainer), typeof(IDb4oEnhancedFilter) }));
		}

		private MethodDefinition Resolve(MethodReference reference)
		{
			return (MethodDefinition) reference;
		}

		private MethodReference GetMethodReferenceFromInlinePredicatePattern(Instruction queryInvocation)
		{
			return (MethodReference)GetNthPrevious(queryInvocation, 2).Operand;
		}

		private bool IsPredicateCreationPattern(Instruction queryInvocation)
		{
			return _predicateCreationPattern.IsBackwardsMatch(queryInvocation);
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
			return _staticFieldPattern.IsBackwardsMatch(instr);
		}
	}
}