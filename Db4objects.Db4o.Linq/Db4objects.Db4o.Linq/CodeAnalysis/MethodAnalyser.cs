/* Copyright (C) 2007 - 2008  Versant Inc.  http://www.db4o.com */

using System;
using System.Reflection;
using Db4objects.Db4o.Internal.Caching;
using Db4objects.Db4o.Linq.Caching;
using Cecil.FlowAnalysis;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.CodeStructure;

using Mono.Cecil;

namespace Db4objects.Db4o.Linq.CodeAnalysis
{
	public class MethodAnalyser
	{
		private static ICache4<MethodDefinition, ActionFlowGraph> _graphCache =
			CacheFactory<MethodDefinition, ActionFlowGraph>.For(CacheFactory.New2QXCache(5));

		private readonly Expression _queryExpression;

		public Expression QueryExpression
		{
			get { return _queryExpression;  }
		}

		public bool IsFieldAccess
		{
			get { return _queryExpression != null && _queryExpression is FieldReferenceExpression; }
		}

		private MethodAnalyser(ActionFlowGraph graph)
		{
			if (graph == null) throw new ArgumentNullException("graph");

			_queryExpression = QueryExpressionFinder.FindIn(graph);
		}

		public static MethodAnalyser FromMethod(MethodInfo info)
		{
			return GetAnalyserFor(ResolveMethod(info));
		}

		private static MethodDefinition ResolveMethod(MethodInfo info)
		{
			if (info == null) throw new ArgumentNullException("info");

			var method = MetadataResolver.Instance.ResolveMethod(info);

			if (method == null) throw new QueryOptimizationException(
				string.Format("Cannot resolve method {0}", info));

			return method;
		}

		private static MethodAnalyser GetAnalyserFor(MethodDefinition method)
		{
			var graph = _graphCache.Produce(method, CreateActionFlowGraph);
			return new MethodAnalyser(graph);
		}

		private static ActionFlowGraph CreateActionFlowGraph(MethodDefinition method)
		{
			return FlowGraphFactory.CreateActionFlowGraph(FlowGraphFactory.CreateControlFlowGraph(method));
		}
	}
}
