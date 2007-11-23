/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Instrumentation.Api;
using Db4objects.Db4o.NativeQueries.Expr.Cmp;
using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;
using Db4objects.Db4o.NativeQueries.Instrumentation;
using Db4objects.Db4o.NativeQueries.Optimization;

namespace Db4objects.Db4o.NativeQueries.Instrumentation
{
	internal class ComparisonBytecodeGeneratingVisitor : IComparisonOperandVisitor
	{
		private IMethodBuilder _methodBuilder;

		private Type _predicateClass;

		private bool _inArithmetic = false;

		private Type _opClass = null;

		private Type _staticRoot = null;

		private ITypeLoader _typeLoader;

		public ComparisonBytecodeGeneratingVisitor(ITypeLoader typeLoader, IMethodBuilder
			 methodBuilder, Type predicateClass)
		{
			this._typeLoader = typeLoader;
			this._methodBuilder = methodBuilder;
			this._predicateClass = predicateClass;
		}

		public virtual void Visit(ConstValue operand)
		{
			object value = operand.Value();
			if (value != null)
			{
				_opClass = value.GetType();
			}
			_methodBuilder.Ldc(value);
			if (value != null)
			{
				Box(value.GetType(), !_inArithmetic);
			}
		}

		public virtual void Visit(FieldValue fieldValue)
		{
			Type lastFieldClass = DeduceFieldClass(fieldValue);
			Type parentClass = DeduceFieldClass(fieldValue.Parent());
			bool needConversion = lastFieldClass.IsPrimitive;
			fieldValue.Parent().Accept(this);
			if (_staticRoot != null)
			{
				_methodBuilder.LoadStaticField(FieldReference(_staticRoot, lastFieldClass, fieldValue
					.FieldName()));
				_staticRoot = null;
				return;
			}
			IFieldRef fieldRef = FieldReference(parentClass, lastFieldClass, fieldValue.FieldName
				());
			_methodBuilder.LoadField(fieldRef);
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
			_staticRoot = _typeLoader.LoadType(root.ClassName());
		}

		public virtual void Visit(ArrayAccessValue operand)
		{
			Type cmpType = DeduceFieldClass(operand.Parent()).GetElementType();
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
			Type rcvType = DeduceFieldClass(operand.Parent());
			MethodInfo method = ReflectUtil.MethodFor(rcvType, operand.MethodName(), operand.
				ParamTypes());
			Type retType = method.ReturnType;
			bool needConversion = retType.IsPrimitive;
			operand.Parent().Accept(this);
			bool oldInArithmetic = _inArithmetic;
			for (int paramIdx = 0; paramIdx < operand.Args().Length; paramIdx++)
			{
				_inArithmetic = operand.ParamTypes()[paramIdx].IsPrimitive;
				operand.Args()[paramIdx].Accept(this);
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
			Type operandType = ArithmeticType(operand);
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

		private void Box(Type boxedType, bool canApply)
		{
			if (!canApply)
			{
				return;
			}
			_methodBuilder.Box(boxedType);
		}

		private Type DeduceFieldClass(IComparisonOperand fieldValue)
		{
			TypeDeducingVisitor visitor = new TypeDeducingVisitor(_predicateClass, _typeLoader
				);
			fieldValue.Accept(visitor);
			return visitor.OperandClass();
		}

		private IFieldRef FieldReference(Type parentClass, Type fieldClass, string name)
		{
			return _methodBuilder.References().ForField(parentClass, fieldClass, name);
		}

		private Type ArithmeticType(IComparisonOperand operand)
		{
			if (operand is ConstValue)
			{
				return ((ConstValue)operand).Value().GetType();
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
				Type left = ArithmeticType(expr.Left());
				Type right = ArithmeticType(expr.Right());
				if (left == typeof(double) || right == typeof(double))
				{
					return typeof(double);
				}
				if (left == typeof(float) || right == typeof(float))
				{
					return typeof(float);
				}
				if (left == typeof(long) || right == typeof(long))
				{
					return typeof(long);
				}
				return typeof(int);
			}
			return null;
		}
	}
}
