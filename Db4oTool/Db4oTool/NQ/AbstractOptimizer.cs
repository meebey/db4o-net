using System;
using Db4objects.Db4o.Instrumentation.Cecil;
using Db4objects.Db4o.NativeQueries;
using Db4objects.Db4o.NativeQueries.Expr;
using Db4objects.Db4o.NativeQueries.Instrumentation;
using Db4oTool.Core;
using Mono.Cecil;

namespace Db4oTool.NQ
{
	public class AbstractOptimizer : AbstractAssemblyInstrumentation
	{
		public void OptimizePredicate(TypeDefinition type, MethodDefinition match, IExpression e)
		{
			TraceInfo("Optimizing '{0}' ({1})", type, e);

			new SODAMethodBuilder(new CecilTypeEditor(type)).InjectOptimization(e);
		}

		public IExpression GetExpression(MethodDefinition match)
		{
			try
			{
				return QueryExpressionBuilder.FromMethodDefinition(match);
			}
			catch (Exception x)
			{
				TraceWarning("WARNING: Predicate '{0}' could not be optimized. {1}", match, x.Message);
				TraceVerbose("{0}", x);
			}
			return null;
		}
	}
}