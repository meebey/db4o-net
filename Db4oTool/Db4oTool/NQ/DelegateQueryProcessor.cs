/* Copyright (C) 2004 - 2006  db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using Db4objects.Db4o;
using Db4objects.Db4o.Instrumentation.Api;
using Db4objects.Db4o.Instrumentation.Cecil;
using Db4objects.Db4o.NativeQueries.Expr;
using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;
using Db4oTool.Core;
using Db4objects.Db4o.Internal.Query;
using Mono.Cecil;
using Mono.Cecil.Cil;
using TypeAttributes=Mono.Cecil.TypeAttributes;
using FieldAttributes=Mono.Cecil.FieldAttributes;
using MethodAttributes=Mono.Cecil.MethodAttributes;
using ParameterAttributes=Mono.Cecil.ParameterAttributes;

namespace Db4oTool.NQ
{
	class DelegateQueryProcessor
	{
		private readonly InstrumentationContext _context;

		private readonly ILPattern _staticFieldPattern = CreateStaticFieldPattern();

		private readonly ILPattern _predicateCreationPattern = ILPattern.Sequence(OpCodes.Newobj, OpCodes.Ldftn);

		private readonly DelegateOptimizer _optimizer;
	    private readonly CecilReflector _reflector;

	    public DelegateQueryProcessor(InstrumentationContext context, DelegateOptimizer optimizer)
		{
			_context = context;
			_optimizer = optimizer;
            _reflector = new CecilReflector(_context);
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
		    MethodReference predicateReference = GetMethodReferenceFromInlinePredicatePattern(queryInvocation);
		    MethodDefinition predicateMethod = Resolve(predicateReference);

		    IExpression expression = _optimizer.GetExpression(predicateMethod);
		    if (expression == null) { return; }

            IDictionary<FieldReference, FieldDefinition> fields;
            TypeDefinition syntheticPredicate = NewSyntheticPredicateFor(expression, predicateMethod, out fields);

            expression.Accept(new UpdateFieldReferences(fields));
                
            _optimizer.OptimizePredicate(syntheticPredicate, predicateMethod, expression);

            RemovePreviousInstrunctions(worker, queryInvocation, 2);

            InjectSyntheticPredicateInstantiation(
                    queryInvocation,
                    worker,
                    syntheticPredicate,
                    fields.Keys,
                    predicateReference.DeclaringType);

		    ReplaceByExecuteEnhancedFilter(queryInvocation);
            
            //Debug.Write(Formatter.FormatMethodBody(worker.GetBody().Method));

		    //Debug.Write(Formatter.FormatMethodBody((MethodDefinition) FindConstructor(syntheticPredicate, 1)));

		    //Debug.Write(Formatter.FormatMethodBody(syntheticPredicate.Methods[0]));
		}

		private TypeDefinition NewSyntheticPredicateFor(IExpression expression, IMemberReference predicateMethod, out IDictionary<FieldReference, FieldDefinition> fields)
	    {
	        TypeDefinition syntheticPredicate = NewSyntheticPredicateFor(predicateMethod);

	        IList<IFieldRef> queryFields = CollectAccessedFields(expression);
	        fields = AddFields(syntheticPredicate, queryFields);
	        if (RequiresSyntheticPredicateInitialization(fields))
	        {
                AddConstructor(syntheticPredicate, fields);
	        }

	        return syntheticPredicate;
	    }

	    private static bool RequiresSyntheticPredicateInitialization<T>(ICollection<T> fields)
	    {
	        return fields.Count > 0;
	    }

	    private void InjectSyntheticPredicateInstantiation(Instruction queryInvocation, CilWorker cil, TypeDefinition syntheticPredicate, ICollection<FieldReference> fieldValuesReferences, TypeReference closureType)
	    {
            VariableDefinition closureObjVar = new VariableDefinition(closureType);
            cil.GetBody().Variables.Add(closureObjVar);

            Instruction ip = cil.Create(OpCodes.Stloc, closureObjVar);
            cil.InsertBefore(queryInvocation, ip);

	        int ctorIndex = 0;
            if (RequiresSyntheticPredicateInitialization(fieldValuesReferences))
            {
                PushParameters(cil, closureObjVar, ip, fieldValuesReferences);
                ctorIndex = 1;
            }

            Instruction newObj = cil.Create(OpCodes.Newobj, FindConstructor(syntheticPredicate, ctorIndex));
            cil.InsertBefore(queryInvocation, newObj);
	    }

	    private void PushParameters(CilWorker worker, VariableDefinition closureObj, Instruction ip, IEnumerable<FieldReference> fieldValuesReferences)
	    {
	        foreach (FieldReference fieldReference in fieldValuesReferences)
	        {
	            Instruction instruction = worker.Create(OpCodes.Ldloc, closureObj);
	            worker.InsertAfter(ip, instruction);

                if (IsPublicField(fieldReference))
                {
                    worker.InsertAfter(instruction, worker.Create(OpCodes.Ldfld, fieldReference));
                }
                else
                {
                    ip = PushFieldContentsUsingReflection(fieldReference, worker, instruction);
                }
	            ip = ip.Next.Next;
	        }
	    }

	    private bool IsPublicField(FieldReference reference)
	    {
	        TypeDefinition parentType = _reflector.ResolveTypeReference(reference.DeclaringType);
	        return (parentType.Fields.GetField(reference.Name).Attributes & FieldAttributes.Public) == FieldAttributes.Public;
	    }

	    /**
         * Expects that the object reference is already in the stack
         */
        private Instruction PushFieldContentsUsingReflection(FieldReference fieldReference, CilWorker cil, Instruction ip)
	    {
            Instruction ldstr = cil.Create(OpCodes.Ldstr, fieldReference.Name);
            cil.InsertAfter(ip, ldstr);
            cil.InsertAfter(
                    ldstr, 
                    cil.Create(OpCodes.Call, ImportReflectionGetter(fieldReference.FieldType)));

            return ip;
	    }

	    private MethodReference ImportReflectionGetter(TypeReference extent)
	    {
            Type queryPlatformType = typeof(Db4objects.Db4o.Query.PredicatePlatform);
	        MethodReference getFieldMethod =  _context.Import(queryPlatformType.GetMethod("GetField", new Type[] {typeof (Object), typeof (string)}));
            return InstantiateGenericMethod(getFieldMethod, extent);
	    }

	    private static void RemovePreviousInstrunctions(CilWorker worker, Instruction instruction, int n)
	    {
	        while (n-- > 0)
	        {
                worker.Remove(instruction.Previous);
	        }
        }

	    private void AddConstructor(TypeDefinition type, IDictionary<FieldReference, FieldDefinition> fields)
	    {
	        MethodAttributes methodAttributes = MethodAttributes.SpecialName | Mono.Cecil.MethodAttributes.RTSpecialName | MethodAttributes.Public;
	        MethodDefinition ctor = new MethodDefinition(MethodDefinition.Ctor, methodAttributes, Import(typeof(void)));
            
            AddMethodParameters(ctor, fields.Values);

            CilWorker cil = ctor.Body.CilWorker;
            cil.Emit(OpCodes.Ldarg_0);
            cil.Emit(OpCodes.Call, DefaultObjectConstructor());

	        EmitFieldInitialization(ctor, fields.Values, ctor.Parameters);

            cil.Emit(OpCodes.Ret);

            type.Constructors.Add(ctor);
        }

	    private static void EmitFieldInitialization(MethodDefinition ctor, IEnumerable<FieldDefinition> fields, ParameterDefinitionCollection parameters)
	    {
            CilWorker cil = ctor.Body.CilWorker;

	        int i = 0;
            foreach (FieldDefinition fieldReference in fields)
            {
                cil.Emit(OpCodes.Ldarg_0);
                cil.Emit(OpCodes.Ldarg, parameters[i++]);
                cil.Emit(OpCodes.Stfld, fieldReference);
            }
	    }

	    private static void AddMethodParameters(IMethodSignature method, IEnumerable<FieldDefinition> fields)
	    {
            int i = 0;
            foreach (FieldDefinition parameter in fields)
            {
                method.Parameters.Add(
                    new ParameterDefinition(parameter.Name, i, ParameterAttributes.None, parameter.FieldType));
            }
	    }

	    private static IDictionary<FieldReference,FieldDefinition> AddFields(TypeDefinition type, IEnumerable<IFieldRef> fields)
	    {
            Dictionary<FieldReference, FieldDefinition> fieldMap = new Dictionary<FieldReference, FieldDefinition>();
            foreach (IFieldRef field in fields)
	        {
	            CecilFieldRef cecilFieldRef = (CecilFieldRef) field;
	            FieldDefinition fieldDefinition = ((FieldDefinition) cecilFieldRef.Reference).Clone();
                fieldMap.Add(cecilFieldRef.Reference, fieldDefinition);
	            type.Fields.Add(fieldDefinition);
            }

	        return fieldMap;
	    }

	    private static IList<IFieldRef> CollectAccessedFields(IExpression expression)
	    {
	        FieldCollectorVisitor fieldCollector = new FieldCollectorVisitor();
            expression.Accept(fieldCollector);

	        return fieldCollector.Fields;
	    }

	    private void ProcessCachedStaticFieldPattern(CilWorker worker, Instruction queryInvocation)
		{
			MethodReference predicateReference = GetMethodReferenceFromStaticFieldPattern(queryInvocation);
			MethodDefinition predicateMethod = Resolve(predicateReference);

			IExpression expression = _optimizer.GetExpression(predicateMethod);
			if (null == expression) return;

			TypeDefinition syntheticPredicate = NewSyntheticPredicateFor(predicateMethod);
			_optimizer.OptimizePredicate(syntheticPredicate, predicateMethod, expression);

			Instruction newObj = worker.Create(OpCodes.Newobj, FindConstructor(syntheticPredicate, 0));
			worker.Replace(queryInvocation.Previous, newObj);

			ReplaceByExecuteEnhancedFilter(queryInvocation);
		}

		private TypeDefinition NewSyntheticPredicateFor(IMemberReference predicate)
		{
			ModuleDefinition module = MainModule();
			TypeDefinition type = new TypeDefinition("Db4o$Predicate$" + module.Types.Count, predicate.DeclaringType.Namespace, TypeAttributes.Sealed|TypeAttributes.NotPublic, Import(typeof(object)));

			type.Constructors.Add(CreateDefaultConstructor());
		
			module.Types.Add(type);

			return type;
		}

		private MethodDefinition CreateDefaultConstructor()
		{
			MethodDefinition ctor = new MethodDefinition(
											MethodDefinition.Ctor,
											MethodAttributes.SpecialName | MethodAttributes.RTSpecialName | MethodAttributes.Public,
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

		private static MethodReference FindConstructor(TypeDefinition type, int index)
		{
			return type.Constructors[index];
		}

		private MethodReference ExecuteEnhancedFilterMethod()
		{
			return _context.Import(typeof(NativeQueryHandler).GetMethod("ExecuteEnhancedFilter", new Type[] { typeof(IObjectContainer), typeof(IDb4oEnhancedFilter) }));
		}

		private static MethodDefinition Resolve(MethodReference reference)
		{
			return (MethodDefinition) reference;
		}

		private static MethodReference GetMethodReferenceFromInlinePredicatePattern(Instruction queryInvocation)
		{
			return (MethodReference)GetNthPrevious(queryInvocation, 2).Operand;
		}

		private bool IsPredicateCreationPattern(Instruction queryInvocation)
		{
			return _predicateCreationPattern.IsBackwardsMatch(queryInvocation);
		}

		private static MethodReference InstantiateGenericMethod(MethodReference methodReference, TypeReference extent)
		{
			GenericInstanceMethod instance = new GenericInstanceMethod(methodReference);
			instance.GenericArguments.Add(extent);
			return instance;
		}

		private static TypeReference GetQueryCallExtent(Instruction queryInvocation)
		{
			GenericInstanceMethod method = (GenericInstanceMethod)queryInvocation.Operand;
			return method.GenericArguments[0];
		}

		private static MethodReference GetMethodReferenceFromStaticFieldPattern(Instruction instr)
		{
			return (MethodReference)GetFirstPrevious(instr, OpCodes.Ldftn).Operand;
		}

		private static Instruction GetFirstPrevious(Instruction instr, OpCode opcode)
		{
			Instruction previous = instr;
			while (previous != null)
			{
				if (previous.OpCode == opcode) return previous;
				previous = previous.Previous;
			}
			throw new ArgumentException("No previous " + opcode + " instruction found");
		}

		private static Instruction GetNthPrevious(Instruction instr, int n)
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

    public class AbstractExpressionVisitor : IExpressionVisitor, IComparisonOperandVisitor
    {
        #region IExpressionVisitor

        public virtual void Visit(AndExpression expression)
        {
            VisitBinaryExpression(expression);
        }

        public virtual void Visit(OrExpression expression)
        {
            VisitBinaryExpression(expression);
        }

        public virtual void Visit(NotExpression expression)
        {
            expression.Expr().Accept(this);
        }

        public virtual void Visit(ComparisonExpression expression)
        {
            expression.Right().Accept(this);
        }

        public virtual void Visit(BoolConstExpression expression)
        {
        }

        #endregion

        private void VisitBinaryExpression(BinaryExpression expression)
        {
            expression.Right().Accept(this);
            expression.Left().Accept(this);
        }

        #region IComparisonOperandVisitor

        public virtual void Visit(ArithmeticExpression operand)
        {
            operand.Left().Accept(this);
            operand.Right().Accept(this);
        }

        public virtual void Visit(ConstValue operand)
        {
        }

        public virtual void Visit(FieldValue operand)
        {
        }

        public virtual void Visit(CandidateFieldRoot root)
        {
        }

        public virtual void Visit(PredicateFieldRoot root)
        {
        }

        public virtual void Visit(StaticFieldRoot root)
        {
        }

        public virtual void Visit(ArrayAccessValue operand)
        {
        }

        public virtual void Visit(MethodCallValue value)
        {
        }

        #endregion
    }

    internal class UpdateFieldReferences : AbstractExpressionVisitor
    {
        public UpdateFieldReferences(IDictionary<FieldReference, FieldDefinition> fields)
        {
            _fields = fields;
        }

        public override void Visit(FieldValue operand)
        {
            CecilFieldRef cecilFieldRef = (CecilFieldRef)operand.Field;
            cecilFieldRef.Reference = _fields[Resolve(operand.Field)];
        }

        private static FieldReference Resolve(IFieldRef fieldRef)
        {
            return ((CecilFieldRef) fieldRef).Field;
        }

        private readonly IDictionary<FieldReference, FieldDefinition> _fields;
    }

    internal class FieldCollectorVisitor : AbstractExpressionVisitor
    {
        private readonly IList<IFieldRef> _fields = new List<IFieldRef>();

        public IList<IFieldRef> Fields
        {
            get { return _fields; }
        }

        #region IExpressionVisitor
        
        public override void Visit(BoolConstExpression expression)
        {
            //TODO: ???
        }

        #endregion

        #region IComparisonOperandVisitor

        public override void Visit(FieldValue operand)
        {
            _fields.Add(operand.Field);
        }

        #endregion
    }
}