/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Diagnostics;
using Db4objects.Db4o.NativeQueries.Expr;
using Mono.Cecil;

namespace Db4oTool.NQ
{
	public class PredicateOptimizer : AbstractOptimizer
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

		private static MethodDefinition GetMatchMethod(TypeDefinition type)
		{
			MethodDefinition[] methods = type.Methods.GetMethod("Match");
			Debug.Assert(1 == methods.Length);
			return methods[0];
		}

		private static bool IsPredicateClass(TypeReference typeRef)
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
