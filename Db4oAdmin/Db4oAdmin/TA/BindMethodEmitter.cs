using System.Reflection;
using Db4oAdmin.Core;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Mono.Cecil;
using Mono.Cecil.Cil;
using ParameterAttributes=Mono.Cecil.ParameterAttributes;

namespace Db4oAdmin.TA
{
	class BindMethodEmitter : MethodEmitter
	{
		public BindMethodEmitter(InstrumentationContext context, FieldReference field) : base(context, field)
		{
		}

		public MethodDefinition Emit()
		{
			MethodDefinition bind = NewExplicitMethod(typeof(IActivatable).GetMethod("Bind"));
			bind.Parameters.Add(new ParameterDefinition("activator", 1, ParameterAttributes.None, ActivatorType()));
			CilWorker cil = bind.Body.CilWorker;

			cil.Emit(OpCodes.Ldarg_0);
			cil.Emit(OpCodes.Ldarg_1);
			cil.Emit(OpCodes.Stfld, _activatorField);

			cil.Emit(OpCodes.Ret);
			return bind;
		}

		private TypeReference ActivatorType()
		{
			return Import(typeof(IActivator));
		}
	}
}
