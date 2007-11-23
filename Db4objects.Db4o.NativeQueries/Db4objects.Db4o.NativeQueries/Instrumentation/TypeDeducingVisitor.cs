/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Instrumentation.Api;
using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;
using Db4objects.Db4o.NativeQueries.Optimization;

namespace Db4objects.Db4o.NativeQueries.Instrumentation
{
	internal class TypeDeducingVisitor : IComparisonOperandVisitor
	{
		private Type _predicateClass;

		private Type _clazz;

		private ITypeLoader _typeLoader;

		public TypeDeducingVisitor(Type predicateClass, ITypeLoader typeLoader)
		{
			this._predicateClass = predicateClass;
			this._typeLoader = typeLoader;
			_clazz = null;
		}

		public virtual void Visit(PredicateFieldRoot root)
		{
			_clazz = _predicateClass;
		}

		public virtual void Visit(CandidateFieldRoot root)
		{
		}

		public virtual void Visit(StaticFieldRoot root)
		{
			try
			{
				_clazz = _typeLoader.LoadType(root.ClassName());
			}
			catch (InstrumentationException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public virtual Type OperandClass()
		{
			return _clazz;
		}

		public virtual void Visit(ArithmeticExpression operand)
		{
		}

		public virtual void Visit(ConstValue operand)
		{
			_clazz = operand.Value().GetType();
		}

		public virtual void Visit(FieldValue operand)
		{
			operand.Parent().Accept(this);
			try
			{
				_clazz = FieldFor(_clazz, operand.FieldName()).GetType();
			}
			catch (Exception e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public virtual void Visit(ArrayAccessValue operand)
		{
			operand.Parent().Accept(this);
			_clazz = _clazz.GetElementType();
		}

		internal virtual FieldInfo FieldFor(Type clazz, string fieldName)
		{
			try
			{
				return Sharpen.Runtime.GetDeclaredField(clazz, fieldName);
			}
			catch (Exception)
			{
			}
			return null;
		}

		public virtual void Visit(MethodCallValue operand)
		{
			operand.Parent().Accept(this);
			MethodInfo method = ReflectUtil.MethodFor(_clazz, operand.MethodName(), operand.ParamTypes
				());
			_clazz = method.ReturnType;
		}
	}
}
