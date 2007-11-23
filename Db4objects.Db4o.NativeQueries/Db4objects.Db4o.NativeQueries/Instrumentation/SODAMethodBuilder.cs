/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections;
using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Instrumentation.Api;
using Db4objects.Db4o.Internal.Query;
using Db4objects.Db4o.NativeQueries.Expr;
using Db4objects.Db4o.NativeQueries.Expr.Cmp;
using Db4objects.Db4o.NativeQueries.Expr.Cmp.Operand;
using Db4objects.Db4o.NativeQueries.Instrumentation;
using Db4objects.Db4o.NativeQueries.Optimization;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.NativeQueries.Instrumentation
{
	public class SODAMethodBuilder
	{
		private const bool LOG_BYTECODE = false;

		private IMethodRef descendRef;

		private IMethodRef constrainRef;

		private IMethodRef greaterRef;

		private IMethodRef smallerRef;

		private IMethodRef containsRef;

		private IMethodRef startsWithRef;

		private IMethodRef endsWithRef;

		private IMethodRef notRef;

		private IMethodRef andRef;

		private IMethodRef orRef;

		private IMethodRef identityRef;

		private readonly ITypeEditor _editor;

		private IMethodBuilder _builder;

		private class SODAExpressionBuilder : IExpressionVisitor
		{
			private Type predicateClass;

			public SODAExpressionBuilder(SODAMethodBuilder _enclosing, Type predicateClass)
			{
				this._enclosing = _enclosing;
				this.predicateClass = predicateClass;
			}

			public virtual void Visit(AndExpression expression)
			{
				expression.Left().Accept(this);
				expression.Right().Accept(this);
				this._enclosing.Invoke(this._enclosing.andRef);
			}

			public virtual void Visit(BoolConstExpression expression)
			{
				this.LoadQuery();
			}

			private void LoadQuery()
			{
				this._enclosing.LoadArgument(1);
			}

			public virtual void Visit(OrExpression expression)
			{
				expression.Left().Accept(this);
				expression.Right().Accept(this);
				this._enclosing.Invoke(this._enclosing.orRef);
			}

			public virtual void Visit(ComparisonExpression expression)
			{
				this.LoadQuery();
				this.Descend(this.FieldNames(expression.Left()));
				expression.Right().Accept(this.ComparisonEmitter());
				this.Constrain(expression.Op());
			}

			private void Descend(IEnumerator fieldNames)
			{
				while (fieldNames.MoveNext())
				{
					this.Descend(fieldNames.Current);
				}
			}

			private ComparisonBytecodeGeneratingVisitor ComparisonEmitter()
			{
				return new ComparisonBytecodeGeneratingVisitor(this._enclosing._editor.Loader(), 
					this._enclosing._builder, this.predicateClass);
			}

			private void Constrain(ComparisonOperator op)
			{
				this._enclosing.Invoke(this._enclosing.constrainRef);
				if (op.Equals(ComparisonOperator.EQUALS))
				{
					return;
				}
				if (op.Equals(ComparisonOperator.IDENTITY))
				{
					this._enclosing.Invoke(this._enclosing.identityRef);
					return;
				}
				if (op.Equals(ComparisonOperator.GREATER))
				{
					this._enclosing.Invoke(this._enclosing.greaterRef);
					return;
				}
				if (op.Equals(ComparisonOperator.SMALLER))
				{
					this._enclosing.Invoke(this._enclosing.smallerRef);
					return;
				}
				if (op.Equals(ComparisonOperator.CONTAINS))
				{
					this._enclosing.Invoke(this._enclosing.containsRef);
					return;
				}
				if (op.Equals(ComparisonOperator.STARTSWITH))
				{
					this._enclosing.Ldc(1);
					this._enclosing.Invoke(this._enclosing.startsWithRef);
					return;
				}
				if (op.Equals(ComparisonOperator.ENDSWITH))
				{
					this._enclosing.Ldc(1);
					this._enclosing.Invoke(this._enclosing.endsWithRef);
					return;
				}
				throw new Exception("Cannot interpret constraint: " + op);
			}

			private void Descend(object fieldName)
			{
				this._enclosing.Ldc(fieldName);
				this._enclosing.Invoke(this._enclosing.descendRef);
			}

			public virtual void Visit(NotExpression expression)
			{
				expression.Expr().Accept(this);
				this._enclosing.Invoke(this._enclosing.notRef);
			}

			private IEnumerator FieldNames(FieldValue fieldValue)
			{
				Collection4 coll = new Collection4();
				IComparisonOperand curOp = fieldValue;
				while (curOp is FieldValue)
				{
					FieldValue curField = (FieldValue)curOp;
					coll.Prepend(curField.FieldName());
					curOp = curField.Parent();
				}
				return coll.GetEnumerator();
			}

			private readonly SODAMethodBuilder _enclosing;
		}

		public SODAMethodBuilder(ITypeEditor editor)
		{
			_editor = editor;
			BuildMethodReferences();
		}

		public virtual void InjectOptimization(IExpression expr)
		{
			_editor.AddInterface(typeof(IDb4oEnhancedFilter));
			_builder = _editor.NewPublicMethod(NativeQueriesPlatform.OPTIMIZE_QUERY_METHOD_NAME
				, typeof(void), new Type[] { typeof(IQuery) });
			Type predicateClass = _editor.ActualType();
			expr.Accept(new SODAMethodBuilder.SODAExpressionBuilder(this, predicateClass));
			_builder.Pop();
			_builder.EndMethod();
		}

		private void LoadArgument(int index)
		{
			_builder.LoadArgument(index);
		}

		private void Invoke(IMethodRef method)
		{
			_builder.Invoke(method);
		}

		private void Ldc(object value)
		{
			_builder.Ldc(value);
		}

		private void BuildMethodReferences()
		{
			descendRef = CreateMethodReference(typeof(IQuery), "descend", new Type[] { typeof(
				string) }, typeof(IQuery));
			constrainRef = CreateMethodReference(typeof(IQuery), "constrain", new Type[] { typeof(
				object) }, typeof(IConstraint));
			greaterRef = CreateMethodReference(typeof(IConstraint), "greater", new Type[] {  }
				, typeof(IConstraint));
			smallerRef = CreateMethodReference(typeof(IConstraint), "smaller", new Type[] {  }
				, typeof(IConstraint));
			containsRef = CreateMethodReference(typeof(IConstraint), "contains", new Type[] { 
				 }, typeof(IConstraint));
			startsWithRef = CreateMethodReference(typeof(IConstraint), "startsWith", new Type
				[] { typeof(bool) }, typeof(IConstraint));
			endsWithRef = CreateMethodReference(typeof(IConstraint), "endsWith", new Type[] { 
				typeof(bool) }, typeof(IConstraint));
			notRef = CreateMethodReference(typeof(IConstraint), "not", new Type[] {  }, typeof(
				IConstraint));
			andRef = CreateMethodReference(typeof(IConstraint), "and", new Type[] { typeof(IConstraint
				) }, typeof(IConstraint));
			orRef = CreateMethodReference(typeof(IConstraint), "or", new Type[] { typeof(IConstraint
				) }, typeof(IConstraint));
			identityRef = CreateMethodReference(typeof(IConstraint), "identity", new Type[] { 
				 }, typeof(IConstraint));
		}

		private IMethodRef CreateMethodReference(Type parent, string name, Type[] args, Type
			 ret)
		{
			return _editor.References().ForMethod(parent, name, args, ret);
		}
	}
}
