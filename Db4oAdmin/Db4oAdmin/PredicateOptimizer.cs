using System;
using System.Diagnostics;
using Cecil.FlowAnalysis.CecilUtilities;
using Db4objects.Db4o.Inside.Query;
using Db4objects.Db4o.Nativequery.Expr;
using Db4objects.Db4o.Query;
using Db4objects.Db4o.Tools.NativeQueries;
using Mono.Cecil;
using Mono.Cecil.Cil;
using MethodAttributes=Mono.Cecil.MethodAttributes;

namespace Db4oAdmin
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
			
			MethodDefinition optimizeQuery = CreateOptimizeQueryMethod();

			TypeReference extent = match.Parameters[0].ParameterType;
			EmitPrologue(optimizeQuery, extent);

			e.Accept(new SodaEmitterVisitor(_context, optimizeQuery));

			EmitEpilogue(optimizeQuery);

			type.Methods.Add(optimizeQuery);
			type.Interfaces.Add(Import(typeof(IDb4oEnhancedFilter)));

			DumpMethodBody(optimizeQuery);
		}

		private void DumpMethodBody(MethodDefinition m)
		{
			if (_context.TraceSwitch.TraceVerbose)
			{
				TraceVerbose(CecilFormatter.FormatMethodBody(m));
			}
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

		private static void EmitEpilogue(MethodDefinition method)
		{
			method.Body.CilWorker.Emit(OpCodes.Ret);
		}

		private void EmitPrologue(MethodDefinition method, TypeReference extent)
		{
			CilWorker worker = method.Body.CilWorker;
			
			// query.Constrain(extent);
			worker.Emit(OpCodes.Ldarg_1);
			worker.Emit(OpCodes.Ldtoken, extent);
			worker.Emit(OpCodes.Call, Import(typeof(Type).GetMethod("GetTypeFromHandle")));
			worker.Emit(OpCodes.Callvirt, Import(typeof(IQuery).GetMethod("Constrain")));
			worker.Emit(OpCodes.Pop);
		}

		private MethodDefinition CreateOptimizeQueryMethod()
		{
			// TODO: make sure importing typeof(void) is ok here for the
			// following scenario: CF 1.0 assembly being instrumented by
			// Db4oAdmin running under .net 2.0
			MethodDefinition method = new MethodDefinition("OptimizeQuery",
			                                               MethodAttributes.Virtual|MethodAttributes.Public,
														   Import(typeof(void)));
			method.Parameters.Add(new ParameterDefinition(Import(typeof(IQuery))));
			
			return method;
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
