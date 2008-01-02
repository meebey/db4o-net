using System;
using Db4oTool.Core;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oTool.TA
{
	class ActivateMethodEmitter : MethodEmitter
	{
		public ActivateMethodEmitter(InstrumentationContext context, FieldDefinition field) : base(context, field)
		{
		}

		public MethodDefinition Emit()
		{
			MethodDefinition activate = NewExplicitMethod(typeof(IActivatable).GetMethod("Activate", new Type[] { typeof(ActivationPurpose) }));

			CilWorker cil = activate.Body.CilWorker;
			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldfld, _activatorField);

			Instruction ret = cil.Create(OpCodes.Ret);

			cil.Emit(OpCodes.Brfalse, ret);

			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldfld, _activatorField);
			cil.Emit(OpCodes.Ldarg_1);
			cil.Emit(OpCodes.Callvirt, _context.Import(typeof(IActivator).GetMethod("Activate", new Type[] { typeof(ActivationPurpose) })));

			cil.Append(ret);

			return activate;
		}
	}
}
