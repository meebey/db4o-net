/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using Db4oAdmin.Core;
using Db4objects.Db4o.Activation;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oAdmin.TA
{
	public class TAInstrumentation : AbstractAssemblyInstrumentation
	{
		private MethodDefinition _activateMethod;

		protected override void ProcessType(TypeDefinition type)
		{
			if (type.IsValueType || type.IsInterface || type.IsAbstract) return;
			if (type.Name == "<Module>") return;

			FieldDefinition activatorField = CreateActivatorField();
			_activateMethod = CreateActivateMethod(activatorField);
			type.Methods.Add(_activateMethod);

			ProcessMethods(type.Methods);

			type.Interfaces.Add(Import(typeof(Db4objects.Db4o.TA.IActivatable)));

			type.Fields.Add(activatorField);
			type.Methods.Add(CreateBindMethod(activatorField));
		}

		private MethodDefinition CreateActivateMethod(FieldDefinition activatorField)
		{
			MethodDefinition activate = new MethodDefinition("db4o$$ta$$activate", MethodAttributes.Family, VoidType());
		
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
			if (method == _activateMethod) return;
			if (!method.HasBody || IsPrivate(method) || method.IsStatic) return;

			Instruction firstInstruction = method.Body.Instructions[0];

			CilWorker cil = method.Body.CilWorker;
			cil.InsertBefore(firstInstruction, cil.Create(OpCodes.Ldarg_0));
			cil.InsertBefore(firstInstruction, cil.Create(OpCodes.Call, _activateMethod));
		}

		private static bool IsPrivate(MethodDefinition method)
		{
			return method.IsCompilerControlled
				|| method.IsPrivate;
		}
	}
}
