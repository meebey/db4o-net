/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Runtime.CompilerServices;
using Db4oAdmin.Core;
using Db4objects.Db4o.Activation;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oAdmin.TA
{
	public class TAInstrumentation : AbstractAssemblyInstrumentation
	{
		private const string ActivateMethodName = "db4o$$ta$$activate";

		public static readonly string CompilerGeneratedAttribute = typeof(CompilerGeneratedAttribute).FullName;

		protected override void ProcessModule(ModuleDefinition module)
		{
			ProcessTypes(module.Types, MakeActivatable);
			ProcessTypes(module.Types, ProcessMethods);
		}

		private void MakeActivatable(TypeDefinition type)
		{
			if (!RequiresTA(type)) return;

			type.Interfaces.Add(Import(typeof(Db4objects.Db4o.TA.IActivatable)));

			FieldDefinition activatorField = CreateActivatorField();
			type.Fields.Add(activatorField);

			type.Methods.Add(CreateActivateMethod(activatorField));
			type.Methods.Add(CreateBindMethod(activatorField));
		}

		private static bool RequiresTA(TypeDefinition type)
		{
			if (type.IsValueType) return false;
			if (type.IsInterface || type.IsAbstract) return false;
			if (type.Name == "<Module>") return false;
			if (ByAttributeFilter.ContainsCustomAttribute(type, CompilerGeneratedAttribute)) return false;
			return true;
		}

		private MethodDefinition CreateActivateMethod(FieldDefinition activatorField)
		{
			MethodDefinition activate = new MethodDefinition(ActivateMethodName, MethodAttributes.Family, VoidType());
		
			CilWorker cil = activate.Body.CilWorker;
			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldfld, activatorField);

			Instruction ret = cil.Create(OpCodes.Ret);

			cil.Emit(OpCodes.Brfalse, ret);

			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldfld, activatorField);
			cil.Emit(OpCodes.Callvirt, Import(typeof(IActivator).GetMethod("Activate")));

			cil.Append(ret);

			return activate;
		}

		private FieldDefinition CreateActivatorField()
		{
			return new FieldDefinition("db4o$$ta$$activator", ActivatorType(), FieldAttributes.Private|FieldAttributes.NotSerialized);
		}

		private MethodDefinition CreateBindMethod(FieldReference activatorField)
		{
			MethodDefinition bind = new MethodDefinition("Bind", MethodAttributes.Public | MethodAttributes.Virtual, VoidType());
			bind.Parameters.Add(new ParameterDefinition("activator", 1, ParameterAttributes.None, ActivatorType()));
			CilWorker cil = bind.Body.CilWorker;

			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldarg_1);
			cil.Emit(OpCodes.Stfld, activatorField);

			cil.Emit(OpCodes.Ret);
			return bind;
		}

		private TypeReference VoidType()
		{
			return Import(typeof(void));
		}

		private TypeReference ActivatorType()
		{
			return Import(typeof(IActivator));
		}

		protected override void ProcessMethod(Mono.Cecil.MethodDefinition method)
		{	
			if (!method.HasBody || IsPrivate(method) || method.IsStatic) return;

			CilWorker cil = method.Body.CilWorker;
			Instruction instruction = method.Body.Instructions[0];
			while (instruction != null)
			{
				if (IsFieldAccess(instruction))
				{
					ProcessFieldAccess(cil, instruction);
				}
				instruction = instruction.Next;
			}
		}

		private void ProcessFieldAccess(CilWorker cil, Instruction instruction)
		{
			FieldReference field = (FieldReference)instruction.Operand;
			if (!IsActivatableField(field))
			{
				return;
			}

			MethodReference activate = ActivateMethod(field);
			if (activate == null)
			{
				// TODO: better message here
				TraceWarning("Cant instrument access to field '{0}'", field);
				return;
			}

			cil.InsertBefore(instruction, cil.Create(OpCodes.Dup));
			cil.InsertBefore(instruction, cil.Create(OpCodes.Call, activate));
		}

		private bool IsActivatableField(FieldReference field)
		{
			// TODO: check for transient here
			return !field.Name.Contains("$");
		}

		private MethodReference ActivateMethod(FieldReference field)
		{	
			TypeDefinition type = field.DeclaringType as TypeDefinition;
			if (type == null) return null;

			MethodDefinition[] methods = type.Methods.GetMethod(ActivateMethodName);
			if (methods.Length != 1) return null;

			return methods[0];
		}

		private static bool IsFieldAccess(Instruction instruction)
		{
			return instruction.OpCode == OpCodes.Ldfld
				// TODO: write a test case for passing a field by reference
				; //|| instruction.OpCode == OpCodes.Ldflda;
		}

		private static bool IsPrivate(MethodDefinition method)
		{
			return method.IsCompilerControlled
				|| method.IsPrivate;
		}
	}
}
