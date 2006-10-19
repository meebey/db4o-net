namespace Db4objects.Db4o.Nativequery.Optimization
{
	internal sealed class ComparisonQueryGeneratingVisitor : Db4objects.Db4o.Nativequery.Expr.Cmp.IComparisonOperandVisitor
	{
		private object _predicate;

		private object _value = null;

		public object Value()
		{
			return _value;
		}

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ConstValue operand)
		{
			_value = operand.Value();
		}

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.FieldValue operand)
		{
			operand.Parent().Accept(this);
			System.Type clazz = ((operand.Parent() is Db4objects.Db4o.Nativequery.Expr.Cmp.Field.StaticFieldRoot
				) ? (System.Type)_value : _value.GetType());
			try
			{
				System.Reflection.FieldInfo field = Db4objects.Db4o.Nativequery.Optimization.ReflectUtil
					.FieldFor(clazz, operand.FieldName());
				_value = field.GetValue(_value);
			}
			catch (System.Exception exc)
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

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticExpression operand
			)
		{
			operand.Left().Accept(this);
			object left = _value;
			operand.Right().Accept(this);
			object right = _value;
			switch (operand.Op().Id())
			{
				case Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator.ADD_ID:
				{
					_value = Add(left, right);
					break;
				}

				case Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator.SUBTRACT_ID:
				{
					_value = Subtract(left, right);
					break;
				}

				case Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator.MULTIPLY_ID:
				{
					_value = Multiply(left, right);
					break;
				}

				case Db4objects.Db4o.Nativequery.Expr.Cmp.ArithmeticOperator.DIVIDE_ID:
				{
					_value = Divide(left, right);
					break;
				}
			}
		}

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.CandidateFieldRoot root
			)
		{
		}

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.PredicateFieldRoot root
			)
		{
			_value = _predicate;
		}

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.Field.StaticFieldRoot root
			)
		{
			try
			{
				_value = System.Type.GetType(root.ClassName());
			}
			catch (Sharpen.Lang.ClassNotFoundException e)
			{
				Sharpen.Runtime.PrintStackTrace(e);
			}
		}

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.ArrayAccessValue operand)
		{
			operand.Parent().Accept(this);
			object parent = _value;
			operand.Index().Accept(this);
			int index = (int)_value;
			_value = Sharpen.Runtime.GetArrayValue(parent, index);
		}

		public void Visit(Db4objects.Db4o.Nativequery.Expr.Cmp.MethodCallValue operand)
		{
			operand.Parent().Accept(this);
			object receiver = _value;
			object[] @params = new object[operand.Args().Length];
			for (int paramIdx = 0; paramIdx < operand.Args().Length; paramIdx++)
			{
				operand.Args()[paramIdx].Accept(this);
				@params[paramIdx] = _value;
			}
			System.Type clazz = receiver.GetType();
			if (operand.Parent().Root() is Db4objects.Db4o.Nativequery.Expr.Cmp.Field.StaticFieldRoot
				 && clazz.Equals(typeof(System.Type)))
			{
				clazz = (System.Type)receiver;
			}
			System.Reflection.MethodInfo method = Db4objects.Db4o.Nativequery.Optimization.ReflectUtil
				.MethodFor(clazz, operand.MethodName(), operand.ParamTypes());
			try
			{
				_value = method.Invoke(receiver, @params);
			}
			catch (System.Exception exc)
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
