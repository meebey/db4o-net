/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using Db4objects.Db4o.Instrumentation.Api;
using Db4objects.Db4o.NativeQueries.Expr.Cmp;
using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;
using Db4objects.Db4o.NativeQueries.Instrumentation;

namespace Db4objects.Db4o.NativeQueries.Instrumentation
{
	internal class ComparisonBytecodeGeneratingVisitor : IComparisonOperandVisitor
	{
		private IMethodBuilder _methodBuilder;

		private ITypeRef _predicateClass;

		private bool _inArithmetic = false;

		private ITypeRef _opClass = null;

		private ITypeRef _staticRoot = null;

		public ComparisonBytecodeGeneratingVisitor(IMethodBuilder methodBuilder, ITypeRef
			 predicateClass)
		{
			this._methodBuilder = methodBuilder;
			this._predicateClass = predicateClass;
		}

		public virtual void Visit(ConstValue operand)
		{
			object value = operand.Value();
			if (value != null)
			{
				_opClass = TypeRef(value.GetType());
			}
			_methodBuilder.Ldc(value);
			if (value != null)
			{
				Box(_opClass, !_inArithmetic);
			}
		}

		private ITypeRef TypeRef(Type type)
		{
			return _methodBuilder.References.ForType(type);
		}

		public virtual void Visit(FieldValue fieldValue)
		{
			ITypeRef lastFieldClass = fieldValue.Field.Type;
			bool needConversion = lastFieldClass.IsPrimitive;
			fieldValue.Parent().Accept(this);
			if (_staticRoot != null)
			{
				_methodBuilder.LoadStaticField(fieldValue.Field);
				_staticRoot = null;
				return;
			}
			_methodBuilder.LoadField(fieldValue.Field);
			Box(lastFieldClass, !_inArithmetic && needConversion);
		}

		public virtual void Visit(CandidateFieldRoot root)
		{
			_methodBuilder.LoadArgument(1);
		}

		public virtual void Visit(PredicateFieldRoot root)
		{
			_methodBuilder.LoadArgument(0);
		}

		public virtual void Visit(StaticFieldRoot root)
		{
			_staticRoot = root.Type;
		}

		public virtual void Visit(ArrayAccessValue operand)
		{
			ITypeRef cmpType = DeduceFieldClass(operand.Parent()).ElementType;
			operand.Parent().Accept(this);
			bool outerInArithmetic = _inArithmetic;
			_inArithmetic = true;
			operand.Index().Accept(this);
			_inArithmetic = outerInArithmetic;
			_methodBuilder.LoadArrayElement(cmpType);
			Box(cmpType, !_inArithmetic);
		}

		public virtual void Visit(MethodCallValue operand)
		{
			IMethodRef method = operand.Method;
			ITypeRef retType = method.ReturnType;
			bool needConversion = retType.IsPrimitive;
			operand.Parent().Accept(this);
			bool oldInArithmetic = _inArithmetic;
			for (int paramIdx = 0; paramIdx < operand.Args.Length; paramIdx++)
			{
				_inArithmetic = operand.Method.ParamTypes[paramIdx].IsPrimitive;
				operand.Args[paramIdx].Accept(this);
			}
			_inArithmetic = oldInArithmetic;
			_methodBuilder.Invoke(method);
			Box(retType, !_inArithmetic && needConversion);
		}

		public virtual void Visit(ArithmeticExpression operand)
		{
			bool oldInArithmetic = _inArithmetic;
			_inArithmetic = true;
			operand.Left().Accept(this);
			operand.Right().Accept(this);
			ITypeRef operandType = ArithmeticType(operand);
			switch (operand.Op().Id())
			{
				case ArithmeticOperator.ADD_ID:
				{
					_methodBuilder.Add(operandType);
					break;
				}

				case ArithmeticOperator.SUBTRACT_ID:
				{
					_methodBuilder.Subtract(operandType);
					break;
				}

				case ArithmeticOperator.MULTIPLY_ID:
				{
					_methodBuilder.Multiply(operandType);
					break;
				}

				case ArithmeticOperator.DIVIDE_ID:
				{
					_methodBuilder.Divide(operandType);
					break;
				}

				default:
				{
					throw new Exception("Unknown operand: " + operand.Op());
					break;
				}
			}
			Box(_opClass, !oldInArithmetic);
			_inArithmetic = oldInArithmetic;
		}

		private void Box(ITypeRef boxedType, bool canApply)
		{
			if (!canApply)
			{
				return;
			}
			_methodBuilder.Box(boxedType);
		}

		private ITypeRef DeduceFieldClass(IComparisonOperand fieldValue)
		{
			TypeDeducingVisitor visitor = new TypeDeducingVisitor(_methodBuilder.References, 
				_predicateClass);
			fieldValue.Accept(visitor);
			return visitor.OperandClass();
		}

		private ITypeRef ArithmeticType(IComparisonOperand operand)
		{
			if (operand is ConstValue)
			{
				return TypeRef(((ConstValue)operand).Value().GetType());
			}
			if (operand is FieldValue)
			{
				try
				{
					return DeduceFieldClass(operand);
				}
				catch (Exception e)
				{
					Sharpen.Runtime.PrintStackTrace(e);
					return null;
				}
			}
			if (operand is ArithmeticExpression)
			{
				ArithmeticExpression expr = (ArithmeticExpression)operand;
				ITypeRef left = ArithmeticType(expr.Left());
				ITypeRef right = ArithmeticType(expr.Right());
				if (left == DoubleRef() || right == DoubleRef())
				{
					return DoubleRef();
				}
				if (left == FloatRef() || right == FloatRef())
				{
					return FloatRef();
				}
				if (left == LongRef() || right == LongRef())
				{
					return LongRef();
				}
				return TypeRef(typeof(int));
			}
			return null;
		}

		private ITypeRef LongRef()
		{
			return TypeRef(typeof(long));
		}

		private ITypeRef FloatRef()
		{
			return TypeRef(typeof(float));
		}

		private ITypeRef DoubleRef()
		{
			return TypeRef(typeof(double));
		}
	}
}
