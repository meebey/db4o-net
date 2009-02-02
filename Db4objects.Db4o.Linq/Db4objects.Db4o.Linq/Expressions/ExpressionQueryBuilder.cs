/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Linq.Expressions;
using System.Reflection;

using Db4objects.Db4o.Linq.Caching;
using Db4objects.Db4o.Linq.CodeAnalysis;
using Db4objects.Db4o.Linq.Internals;
using Db4objects.Db4o.Query;

namespace Db4objects.Db4o.Linq.Expressions
{
	internal abstract class ExpressionQueryBuilder : ExpressionVisitor
	{
		private QueryBuilderRecorder _recorder;

		public QueryBuilderRecorder Recorder
		{
			get { return _recorder; }
		}

		public ExpressionQueryBuilder()
		{
		}

		public IQueryBuilderRecord Process(Expression expression)
		{
			return ProcessExpression(SubtreeEvaluator.Evaluate(Normalize(expression)));
		}

		private Expression Normalize(Expression expression)
		{
			return new ExpressionTreeNormalizer().Normalize(expression);
		}

		protected abstract ICachingStrategy<Expression, IQueryBuilderRecord> GetCachingStrategy();

		private IQueryBuilderRecord ProcessExpression(Expression expression)
		{
			return GetCachingStrategy().Produce(expression, CreateRecord);
		}

		private IQueryBuilderRecord CreateRecord(Expression expression)
		{
			_recorder = new QueryBuilderRecorder();
			Visit(expression);
			return _recorder.Record;
		}

		private static bool IsParameter(Expression expression)
		{
			return expression.NodeType == ExpressionType.Parameter;
		}

		protected static bool IsParameterReference(Expression expression)
		{
			var unary = expression as UnaryExpression;
			if (unary != null) return IsParameterReference(unary.Operand);

			var me = expression as MemberExpression;
			if (me != null) return IsParameter(me.Expression);

			var call = expression as MethodCallExpression;
			if (call != null && call.Object != null) return IsParameter(call.Object);

			return false;
		}

		protected static bool IsFieldAccessExpression(MemberExpression m)
		{
			return m.Member is FieldInfo;
		}

		protected static bool IsPropertyAccessExpression(MemberExpression m)
		{
			return m.Member is PropertyInfo;
		}

		protected static MethodInfo GetGetMethod(MemberExpression m)
		{
			return ((PropertyInfo)m.Member).GetGetMethod();
		}

		protected void ProcessMemberAccess(MemberExpression m)
		{
			if (IsFieldAccessExpression(m))
			{
				_recorder.Add(ctx => ctx.PushQuery(ctx.RootQuery.Descend(m.Member.Name)));
				return;
			}
			else if (IsPropertyAccessExpression(m))
			{
				AnalyseMethod(_recorder, GetGetMethod(m));
				return;
			}

			CannotOptimize(m);
		}

		protected void AnalyseMethod(QueryBuilderRecorder recorder, MethodInfo method)
		{
			AnalyseMethod(recorder, method, new object[0]);
		}

		protected void AnalyseMethod(QueryBuilderRecorder recorder, MethodInfo method, object[] parameters)
		{
			try
			{
				MethodAnalyser.FromMethod(method, parameters).AugmentQuery(recorder);
			}
			catch (Exception e)
			{
				throw new QueryOptimizationException(e.Message, e);
			}
		}

		protected static void CannotOptimize(Expression e)
		{
			throw new QueryOptimizationException(e.ToString());
		}

		protected static void CannotOptimize(ElementInit init)
		{
			throw new QueryOptimizationException(init.ToString());
		}

		protected static void CannotOptimize(MemberBinding binding)
		{
			throw new QueryOptimizationException(binding.ToString());
		}

		protected override void VisitBinding(MemberBinding binding)
		{
			CannotOptimize(binding);
		}

		protected override void VisitConditional(ConditionalExpression conditional)
		{
			CannotOptimize(conditional);
		}

		protected override void VisitElementInitializer(ElementInit initializer)
		{
			CannotOptimize(initializer);
		}

		protected override void VisitInvocation(InvocationExpression invocation)
		{
			CannotOptimize(invocation);
		}

		protected override void VisitListInit(ListInitExpression init)
		{
			CannotOptimize(init);
		}

		protected override void VisitNew(NewExpression nex)
		{
			CannotOptimize(nex);
		}

		protected override void VisitNewArray(NewArrayExpression newArray)
		{
			CannotOptimize(newArray);
		}
	}
}
