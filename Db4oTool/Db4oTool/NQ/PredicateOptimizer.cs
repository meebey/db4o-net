/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Diagnostics;
using Db4objects.Db4o.Instrumentation.Cecil;
using Db4objects.Db4o.NativeQueries.Instrumentation;
using Db4oTool.Core;
using Db4objects.Db4o.NativeQueries.Expr;
using Db4objects.Db4o.NativeQueries;
using Mono.Cecil;

namespace Db4oTool.NQ
{
	public class PredicateOptimizer : AbstractAssemblyInstrumentation
	{
		int _predicateCount;

		protected override void BeforeAssemblyProcessing()
		{
			_predicateCount = 0;
		}
		
		protected override void  AfterAssemblyProcessing()
		{
			string format = _predicateCount == 1
			                	? "{0} predicate class processed."
			                	: "{0} predicate classes processed.";
			TraceInfo(format, _predicateCount);
		}
		
		protected override void ProcessType(TypeDefinition type)
		{
			if (IsPredicateClass(type))
			{
				InstrumentPredicateClass(type);
			}
		}

		private void InstrumentPredicateClass(TypeDefinition type)
		{
			++_predicateCount;
			
			MethodDefinition match = GetMatchMethod(type);
			IExpression e = GetExpression(match);
			if (null == e) return;

			OptimizePredicate(type, match, e);
		}

		private void OptimizePredicate(TypeDefinition type, MethodDefinition match, IExpression e)
		{
			TraceInfo("Optimizing '{0}' ({1})", type, e);

			new SODAMethodBuilder(new CecilTypeEditor(type)).InjectOptimization(e);
		}

		private IExpression GetExpression(MethodDefinition match)
		{
			try
			{
				return QueryExpressionBuilder.FromMethodDefinition(match);
			}
			catch (Exception x)
			{	
				TraceWarning("WARNING: Predicate '{0}' could not be optimized. {1}", match.DeclaringType, x.Message);
				TraceVerbose("{0}", x);
			}
			return null;
		}

		private MethodDefinition GetMatchMethod(TypeDefinition type)
		{
			MethodDefinition[] methods = type.Methods.GetMethod("Match");
			Debug.Assert(1 == methods.Length);
			return methods[0];
		}

		private bool IsPredicateClass(TypeReference typeRef)
		{
			TypeDefinition type = typeRef as TypeDefinition;
			if (null == type) return false;
			TypeReference baseType = type.BaseType;
			if (null == baseType) return false;
			if (typeof(Db4objects.Db4o.Query.Predicate).FullName == baseType.FullName) return true;
			return IsPredicateClass(baseType);
		}
	}
}
