/* Copyright (C) 2007 - 2008  db4objects Inc.  http://www.db4o.com */

using System;
using System.Collections.Generic;
using System.Reflection;

using Db4objects.Db4o;
using Db4objects.Db4o.Linq.Caching;
using Db4objects.Db4o.Query;

using Cecil.FlowAnalysis;
using Cecil.FlowAnalysis.ActionFlow;
using Cecil.FlowAnalysis.CodeStructure;

using Mono.Cecil;

namespace Db4objects.Db4o.Linq.CodeAnalysis
{
	internal class MethodAnalyser
	{
		private static ICachingStrategy<MethodDefinition, ActionFlowGraph> _graphCache =
			new SingleItemCachingStrategy<MethodDefinition, ActionFlowGraph>();

		private ActionFlowGraph _graph;
		private Expression _queryExpression;
		private object[] _parameters;

		public bool IsFieldAccess
		{
			get { return _queryExpression != null && _queryExpression is FieldReferenceExpression; }
		}

		private MethodAnalyser(ActionFlowGraph graph, object[] parameters)
		{
			if (graph == null) throw new ArgumentNullException("graph");
			if (parameters == null) throw new ArgumentNullException("parameters");

			_graph = graph;
			_parameters = parameters;
			_queryExpression = QueryExpressionFinder.FindIn(graph);
		}

		public void AugmentQuery(QueryBuilderRecorder recorder)
		{
			if (_queryExpression == null) throw new QueryOptimizationException("No query expression");

			_queryExpression.Accept(new CodeQueryBuilder(recorder));
		}

		public static MethodAnalyser FromMethod(MethodInfo info, object[] parameters)
		{
			return GetAnalyserFor(ResolveMethod(info), parameters);
		}

		private static MethodDefinition ResolveMethod(MethodInfo info)
		{
			if (info == null) throw new ArgumentNullException("info");

			var method = MetadataResolver.Instance.ResolveMethod(info);

			if (method == null) throw new QueryOptimizationException(
				string.Format("Cannot resolve method {0}", info));

			return method;
		}

		private static MethodAnalyser GetAnalyserFor(MethodDefinition method, object[] parameters)
		{
			var graph = GetCachedGraph(method);
			if (graph != null) return new MethodAnalyser(graph, parameters);

			graph = CreateActionFlowGraph(method);

			CacheGraph(method, graph);

			return new MethodAnalyser(graph, parameters);
		}

		private static ActionFlowGraph GetCachedGraph(MethodDefinition method)
		{
			return _graphCache.Get(method);
		}

		private static void CacheGraph(MethodDefinition method, ActionFlowGraph graph)
		{
			_graphCache.Add(method, graph);
		}

		private static ActionFlowGraph CreateActionFlowGraph(MethodDefinition method)
		{
			return FlowGraphFactory.CreateActionFlowGraph(FlowGraphFactory.CreateControlFlowGraph(method));
		}
	}
}
