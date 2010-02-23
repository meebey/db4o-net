/* Copyright (C) 2010  Versant Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using Db4objects.Db4o.Collections;
using Db4oTool.Core;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oTool.TA
{
	internal class TACollectionsStep : TAInstrumentationStepBase
	{
		public override void Process(MethodDefinition method)
		{
			InstrumentListInstantiation(method);

			InstrumentListCasts(method);
		}

		private void InstrumentListCasts(MethodDefinition methodDefinition)
		{
			foreach (Instruction cast in ListCasts(methodDefinition.Body))
			{
				StackAnalysisResult result = StackAnalyzer.IsConsumedBy(MethodCallOnList, cast, methodDefinition.DeclaringType.Module);
				if (!result.Match)
				{
					continue;
					//TODO: Warning ? Error?
				}

				ReplaceCastAndCalleDeclaringType(cast, result.Consumer, typeof(ActivatableList<>));
			}
		}

		private void ReplaceCastAndCalleDeclaringType(Instruction cast, Instruction originalCall, Type replacementType)
		{
			GenericInstanceType originalTypeReference = (GenericInstanceType)cast.Operand;

			GenericInstanceType replacementReferenceType = NewGenericInstanceTypeWithArgumentsFrom(Context.Import(replacementType), originalTypeReference);
			cast.Operand = replacementReferenceType;
			originalCall.Operand = MethodReferenceFor((MethodReference)originalCall.Operand, replacementReferenceType);
		}

		private static MethodReference MethodReferenceFor(MethodReference source, TypeReference declaringType)
		{
			MethodReference newMethod = new MethodReference(source.Name, declaringType, source.ReturnType.ReturnType, true, false, MethodCallingConvention.Default);
			foreach (ParameterDefinition param in source.Parameters)
			{
				newMethod.Parameters.Add(param);
			}

			return newMethod;
		}

		private GenericInstanceType NewGenericInstanceTypeWithArgumentsFrom(TypeReference referenceType, GenericInstanceType argumentSource)
		{
			GenericInstanceType replacementTypeReference = new GenericInstanceType(referenceType);
			foreach (TypeReference argument in argumentSource.GenericArguments)
			{
				replacementTypeReference.GenericArguments.Add(argument);
			}
			return replacementTypeReference;
		}

		private static bool MethodCallOnList(Instruction candidate)
		{
			if (candidate.OpCode != OpCodes.Call && candidate.OpCode != OpCodes.Callvirt) return false;

			MethodDefinition callee = ((MethodReference)candidate.Operand).Resolve();
			return callee.DeclaringType.Resolve().FullName == callee.DeclaringType.Module.Import(typeof(List<>)).FullName;
		}

		private IEnumerable<Instruction> ListCasts(MethodBody body)
		{
			return InstrumentationUtil.Where(body, delegate(Instruction candidate)
			{
				if (candidate.OpCode != OpCodes.Castclass) return false;
				GenericInstanceType target = candidate.Operand as GenericInstanceType;

				return target != null && target.Resolve() == Context.Import(typeof(List<>)).Resolve();
			});
		}

		private void InstrumentListInstantiation(MethodDefinition methodDefinition)
		{
			foreach (Instruction newObj in ListInstantiations(methodDefinition.Body))
			{
				StackAnalysisResult stackAnalysis = StackAnalyzer.IsConsumedBy(delegate { return true; }, newObj, methodDefinition.DeclaringType.Module);
				if (IsAssignmentToConcreteType(stackAnalysis))
				{
					Context.TraceWarning("[{0}] Assignment to concrete collection {1} ignored (offset: 0x{2:X2}).", methodDefinition, InstantiatedType(newObj), newObj.Next.Offset);
					continue;
				}

				ReplaceContructorWithConstructorFrom(newObj, typeof(ActivatableList<>));
			}
		}

		private static string InstantiatedType(Instruction newObj)
		{
			MethodReference originalCtor = (MethodReference)newObj.Operand;

			GenericInstanceType originalType = (GenericInstanceType)originalCtor.DeclaringType;

			return originalType.FullName;
		}

		private void ReplaceContructorWithConstructorFrom(Instruction newObj, Type type)
		{
			MethodReference originalCtor = (MethodReference)newObj.Operand;

			GenericInstanceType originalList = (GenericInstanceType)originalCtor.DeclaringType;
			GenericInstanceType declaringType = new GenericInstanceType(Context.Import(type));

			foreach (TypeReference argument in originalList.GenericArguments)
			{
				declaringType.GenericArguments.Add(argument);
			}

			MethodReference newCtor = new MethodReference(".ctor", declaringType, Context.Import(typeof(void)), originalCtor.HasThis, originalCtor.ExplicitThis, MethodCallingConvention.Default);
			foreach (ParameterDefinition parameter in originalCtor.Parameters)
			{
				newCtor.Parameters.Add(parameter);
			}

			newObj.Operand = newCtor;
		}

		private bool IsAssignmentToConcreteType(StackAnalysisResult stackAnalysis)
		{
			TypeReference assignmentTargetType;
			if (InstrumentationUtil.IsCallInstruction(stackAnalysis.Consumer))
			{
				assignmentTargetType = stackAnalysis.AssignedParameter().ParameterType;
			}
			else
			{
				FieldReference assignmentTarget = stackAnalysis.Consumer.Operand as FieldReference;
				if (assignmentTarget != null)
				{
					assignmentTargetType = assignmentTarget.FieldType;
				}
				else
				{
					VariableReference variableDefinition = stackAnalysis.Consumer.Operand as VariableReference;
					if (variableDefinition != null)
					{
						assignmentTargetType = variableDefinition.VariableType;
					}
					else
					{
						throw new InvalidOperationException();
					}
				}
			}
			return assignmentTargetType.GetOriginalType() == Context.Import(typeof(List<>));
		}

		private static IEnumerable<Instruction> ListInstantiations(MethodBody methodBody)
		{
			return InstrumentationUtil.Where(methodBody, delegate(Instruction candidate)
			{
				if (candidate.OpCode != OpCodes.Newobj) return false;
				MethodReference ctor = (MethodReference)candidate.Operand;
				TypeDefinition declaringType = ctor.DeclaringType.Resolve();

				return declaringType.HasGenericParameters && declaringType.FullName == typeof(List<>).FullName;
			});
		}
	}
}
