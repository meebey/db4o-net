/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;

using Db4objects.Db4o;
using Db4objects.Db4o.Linq;
using Db4objects.Db4o.Query;

using Cecil.FlowAnalysis;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.CodeStructure;
using Cecil.FlowAnalysis.Utilities;

using Mono.Cecil;
using Mono.Cecil.Cil;
using Db4objects.Db4o.Linq.Internals;

namespace Db4objects.Db4o.Linq.CodeAnalysis
{
	internal class CodeQueryBuilder : AbstractCodeStructureVisitor
	{
		private QueryBuilderRecorder _recorder;

		public CodeQueryBuilder(QueryBuilderRecorder recorder)
		{
			_recorder = recorder;
		}

		public override void Visit(ArgumentReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(AssignExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(BinaryExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(CastExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(FieldReferenceExpression node)
		{
			_recorder.Add(ctx => ctx.PushQuery(ctx.RootQuery.Descend(node.Field.Name)));
		}

		public override void Visit(LiteralExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(MethodInvocationExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(MethodReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(PropertyReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(ThisReferenceExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(UnaryExpression node)
		{
			CannotOptimize(node);
		}

		public override void Visit(VariableReferenceExpression node)
		{
			CannotOptimize(node);
		}

		private static void CannotOptimize(Expression expression)
		{
			throw new QueryOptimizationException(ExpressionPrinter.ToString(expression));
		}
	}
}
