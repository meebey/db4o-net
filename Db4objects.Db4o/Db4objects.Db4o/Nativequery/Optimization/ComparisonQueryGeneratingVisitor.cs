using System;
using System.Reflection;
using Db4objects.Db4o.Nativequery.Expr.Cmp;
using Db4objects.Db4o.Nativequery.Expr.Cmp.Field;
using Db4objects.Db4o.Nativequery.Optimization;
using Sharpen.Lang.Reflect;

namespace Db4objects.Db4o.Nativequery.Optimization
{
	internal sealed class ComparisonQueryGeneratingVisitor : IComparisonOperandVisitor
	{
		private object _predicate;

		private object _value = null;

		public object Value()
		{
			return _value;
		}

		public void Visit(ConstValue operand)
		{
			_value = operand.Value();
		}

		public void Visit(FieldValue operand)
		{
			operand.Parent().Accept(this);
			Type clazz = ((operand.Parent() is StaticFieldRoot) ? (Type)_value : _value.GetType
				());
			try
			{
				FieldInfo field = ReflectUtil.FieldFor(clazz, operand.FieldName());
				_value = field.GetValue(_value);
			}
			catch (Exception exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
			}
		}

		internal object Add(object a, object b)
		{
			if (a is double || b is double)
			{
				return ((double)a) + ((double)b);
			}
			if (a is float || b is float)
			{
				return ((float)a) + ((float)b);
			}
			if (a is long || b is long)
			{
				return ((long)a) + ((long)b);
			}
			return ((int)a) + ((int)b);
		}

		internal object Subtract(object a, object b)
		{
			if (a is double || b is double)
			{
				return ((double)a) - ((double)b);
			}
			if (a is float || b is float)
			{
				return ((float)a) - ((float)b);
			}
			if (a is long || b is long)
			{
				return ((long)a) - ((long)b);
			}
			return ((int)a) - ((int)b);
		}

		internal object Multiply(object a, object b)
		{
			if (a is double || b is double)
			{
				return ((double)a) * ((double)b);
			}
			if (a is float || b is float)
			{
				return ((float)a) * ((float)b);
			}
			if (a is long || b is long)
			{
				return ((long)a) * ((long)b);
			}
			return ((int)a) * ((int)b);
		}

		internal object Divide(object a, object b)
		{
			if (a is double || b is double)
			{
				return ((double)a) / ((double)b);
			}
			if (a is float || b is float)
			{
				return ((float)a) / ((float)b);
			}
			if (a is long || b is long)
			{
				return ((long)a) / ((long)b);
			}
			return ((int)a) / ((int)b);
		}

		public void Visit(ArithmeticExpression operand)
		{
			operand.Left().Accept(this);
			object left = _value;
			operand.Right().Accept(this);
			object right = _value;
			switch (operand.Op().Id())
			{
				case ArithmeticOperator.ADD_ID:
				{
					_value = Add(left, right);
					break;
				}

				case ArithmeticOperator.SUBTRACT_ID:
				{
					_value = Subtract(left, right);
					break;
				}

				case ArithmeticOperator.MULTIPLY_ID:
				{
					_value = Multiply(left, right);
					break;
				}

				case ArithmeticOperator.DIVIDE_ID:
				{
					_value = Divide(left, right);
					break;
				}
			}
		}

		public void Visit(CandidateFieldRoot root)
		{
		}

		public void Visit(PredicateFieldRoot root)
		{
			_value = _predicate;
		}

		public void Visit(StaticFieldRoot root)
		{
			try
			{
				_value = Sharpen.Runtime.GetType(root.ClassName());
			}
			catch (TypeLoadException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public void Visit(ArrayAccessValue operand)
		{
			operand.Parent().Accept(this);
			object parent = _value;
			operand.Index().Accept(this);
			int index = (int)_value;
			_value = Sharpen.Runtime.GetArrayValue(parent, index);
		}

		public void Visit(MethodCallValue operand)
		{
			operand.Parent().Accept(this);
			object receiver = _value;
			object[] @params = new object[operand.Args().Length];
			for (int paramIdx = 0; paramIdx < operand.Args().Length; paramIdx++)
			{
				operand.Args()[paramIdx].Accept(this);
				@params[paramIdx] = _value;
			}
			Type clazz = receiver.GetType();
			if (operand.Parent().Root() is StaticFieldRoot && clazz.Equals(typeof(Type)))
			{
				clazz = (Type)receiver;
			}
			MethodInfo method = ReflectUtil.MethodFor(clazz, operand.MethodName(), operand.ParamTypes
				());
			try
			{
				_value = method.Invoke(receiver, @params);
			}
			catch (Exception exc)
			{
				Sharpen.Runtime.PrintStackTrace(exc);
				_value = null;
			}
		}

		public ComparisonQueryGeneratingVisitor(object predicate) : base()
		{
			this._predicate = predicate;
		}
	}
}
