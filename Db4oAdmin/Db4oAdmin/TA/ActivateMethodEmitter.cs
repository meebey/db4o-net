using Db4oAdmin.Core;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Mono.Cecil;
using Mono.Cecil.Cil;

namespace Db4oAdmin.TA
{
	class ActivateMethodEmitter : MethodEmitter
	{
		public ActivateMethodEmitter(InstrumentationContext context, FieldDefinition field) : base(context, field)
		{
		}

		public MethodDefinition Emit()
		{
			MethodDefinition activate = NewExplicitMethod(typeof(IActivatable).GetMethod("Activate"));

			CilWorker cil = activate.Body.CilWorker;
			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldfld, _activatorField);

			Instruction ret = cil.Create(OpCodes.Ret);

			cil.Emit(OpCodes.Brfalse, ret);

			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldfld, _activatorField);
			cil.Emit(OpCodes.Callvirt, _context.Import(typeof(IActivator).GetMethod("Activate")));

			cil.Append(ret);

			return activate;
		}
	}
}
