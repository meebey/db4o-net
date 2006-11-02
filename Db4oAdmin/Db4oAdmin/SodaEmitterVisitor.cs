using System;
using System.Reflection;
using Cecil.FlowAnalysis.CodeStructure;
using Db4objects.Db4o.Nativequery.Expr;
using Db4objects.Db4o.Nativequery.Expr.Cmp;
using Db4objects.Db4o.Nativequery.Expr.Cmp.Field;
using Db4objects.Db4o.Query;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oAdmin
{
	// const values
	// value type candidates
	// code compiled in release mode
	// queries with delegates instead com.db4o.query.Predicate subclasses
	// arithmetic expressions
	// static fields
	// arrays
	// method calls
	class SodaEmitterVisitor : IExpressionVisitor, IComparisonOperandVisitor
	{
		private CilWorker _worker;
		private InstrumentationContext _context;
		private MethodReference _Query_Descend;
		private MethodReference _Query_Constrain;
		private int _depth;
		private MethodReference _Constraint_Or;
		private MethodReference _Constraint_And;
		private MethodReference _Constraint_Not;
		private MethodReference _Constraint_Greater;
		private MethodReference _Constraint_Smaller;
		private MethodReference _Constraint_EndsWith;
		private MethodReference _Constraint_StartsWith;
		private MethodReference _Constraint_Contains;

		public SodaEmitterVisitor(InstrumentationContext context, MethodDefinition method)
		{
			_context = context;
			_worker = method.Body.CilWorker;

			_Query_Descend = ImportQueryMethod("Descend", typeof(string));
			_Query_Constrain = ImportQueryMethod("Constrain", typeof(object));
			_Constraint_Or = ImportConstraintMethod("Or", typeof(IConstraint));
			_Constraint_And = ImportConstraintMethod("And", typeof(IConstraint));
			_Constraint_Not = ImportConstraintMethod("Not");
			_Constraint_Greater = ImportConstraintMethod("Greater");
			_Constraint_Smaller = ImportConstraintMethod("Smaller");
			_Constraint_StartsWith = ImportConstraintMethod("StartsWith", typeof(bool));
			_Constraint_EndsWith = ImportConstraintMethod("EndsWith", typeof(bool));
			_Constraint_Contains = ImportConstraintMethod("Contains");
		}

		private MethodReference ImportConstraintMethod(string methodName, params Type[] signature)
		{
			return ImportMethod(typeof(IConstraint), methodName, signature);
		}

		private MethodReference ImportQueryMethod(string methodName, params Type[] signature)
		{
			return ImportMethod(typeof(IQuery), methodName, signature);
		}

		private MethodReference ImportMethod(Type type, string methodName, Type[] signature)
		{
			MethodInfo method = type.GetMethod(methodName, signature);
			if (null == method)
			{
				throw new ArgumentException(
					string.Format("Method '{2}.{0}({1})' not found.", methodName, ToCommaSeparatedList(signature), type));
			}
			return _context.Import(method);
		}

		public void Visit(AndExpression expression)
		{
			EmitBinaryExpression(expression, _Constraint_And);
		}

		public void Visit(OrExpression expression)
		{
			EmitBinaryExpression(expression, _Constraint_Or);
		}

		private void EmitBinaryExpression(BinaryExpression expression, MethodReference op)
		{
			EnterExpression();
			expression.Left().Accept(this);
			expression.Right().Accept(this);
			_worker.Emit(OpCodes.Callvirt, op);
			LeaveExpression();
		}

		public void Visit(NotExpression expression)
		{	
			EnterExpression();
			expression.Expr().Accept(this);
			_worker.Emit(OpCodes.Callvirt, _Constraint_Not);
			LeaveExpression();
		}

		public void Visit(ComparisonExpression expression)
		{
			EnterExpression();
			
			expression.Left().Accept(this);
			expression.Right().Accept(this);
			
			EmitBoxIfNeeded(expression.Left());
			EmitConstraint(expression.Op());
			
			LeaveExpression();
		}

		private void EmitConstraint(ComparisonOperator op)
		{
			_worker.Emit(OpCodes.Callvirt, _Query_Constrain);
			switch (op.Id())
			{
				case ComparisonOperator.GREATER_ID:
					_worker.Emit(OpCodes.Callvirt, _Constraint_Greater);
					break;
				case ComparisonOperator.SMALLER_ID:
					_worker.Emit(OpCodes.Callvirt, _Constraint_Smaller);
					break;
				case ComparisonOperator.STARTSWITH_ID:
					PushCaseSensitiveFlag();
					_worker.Emit(OpCodes.Callvirt, _Constraint_StartsWith);
					break;
				case ComparisonOperator.ENDSWITH_ID:
					PushCaseSensitiveFlag();
					_worker.Emit(OpCodes.Callvirt, _Constraint_EndsWith);
					break;
				case ComparisonOperator.CONTAINS_ID:
					_worker.Emit(OpCodes.Callvirt, _Constraint_Contains);
					break;
				case ComparisonOperator.EQUALS_ID:
					break;
				default:
					throw new NotImplementedException(op.ToString());
			}
		}

		private void EmitBoxIfNeeded(FieldValue left)
		{
			FieldReference field = GetFieldReference(left);
			if (!field.FieldType.IsValueType) return;
				
			_worker.Emit(OpCodes.Box, field.FieldType);
		}

		private void PushCaseSensitiveFlag()
		{
			if (_context.Configuration.CaseSensitive)
			{
				_worker.Emit(OpCodes.Ldc_I4_1);
			}
			else
			{
				_worker.Emit(OpCodes.Ldc_I4_0);
			}
		}

		private void EnterExpression()
		{
			_depth++;
		}

		private void LeaveExpression()
		{
			_depth--;
			if (0 == _depth) _worker.Emit(OpCodes.Pop);
		}

		public void Visit(BoolConstExpression expression)
		{
			throw new NotImplementedException();
		}

		public void Visit(ArithmeticExpression operand)
		{
			throw new NotImplementedException();
		}

		public void Visit(ConstValue operand)
		{
			object value = operand.Value();
			Type type = value.GetType();
			TypeCode code = Type.GetTypeCode(type);
			switch (code)
			{
				case TypeCode.SByte:
					_worker.Emit(OpCodes.Ldc_I4_S, (SByte)value);
					break;
				case TypeCode.Int32:
					_worker.Emit(OpCodes.Ldc_I4, (Int32)value);
					break;
				case TypeCode.Int64:
					_worker.Emit(OpCodes.Ldc_I8, (Int64)value);
					break;
				case TypeCode.String:
					_worker.Emit(OpCodes.Ldstr, (String)value);
					break;
				default:
					throw new NotImplementedException(code.ToString());
			}
		}

		public void Visit(FieldValue operand)
		{   
		    operand.Parent().Accept(this);
		    
            IComparisonOperandAnchor root = operand.Root();
		    if (root is CandidateFieldRoot)
			{
				// query.Descend(operand.FieldName());
			    _worker.Emit(OpCodes.Ldstr, operand.FieldName());
			    _worker.Emit(OpCodes.Callvirt, _Query_Descend);
			}
			else if (root is PredicateFieldRoot)
			{
				FieldReference field = GetFieldReference(operand);
				_worker.Emit(OpCodes.Ldfld, field);
			}
		}

		private static FieldReference GetFieldReference(FieldValue operand)
		{
			return (FieldReference)((IFieldReferenceExpression) operand.Tag()).Field;
		}

		public void Visit(CandidateFieldRoot root)
		{
			// being visited as part of a FieldValue
            _worker.Emit(OpCodes.Ldarg_1);
		}

		public void Visit(PredicateFieldRoot root)
		{
            _worker.Emit(OpCodes.Ldarg_0);
		}

		public void Visit(StaticFieldRoot root)
		{
			throw new NotImplementedException();
		}

		public void Visit(ArrayAccessValue operand)
		{
			throw new NotImplementedException();
		}

		public void Visit(MethodCallValue value)
		{
			throw new NotImplementedException();
		}

		private string ToCommaSeparatedList(Type[] signature)
		{
			string[] values = new string[signature.Length];
			for (int i = 0; i < values.Length; ++i) values[i] = signature[i].ToString();
			return string.Join(", ", values);
		}
	}
}