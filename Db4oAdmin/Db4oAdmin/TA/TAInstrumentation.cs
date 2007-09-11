/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.IO;
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
			if (HasInstrumentedBaseType(type)) return;

			type.Interfaces.Add(Import(typeof(Db4objects.Db4o.TA.IActivatable)));

			FieldDefinition activatorField = CreateActivatorField();
			type.Fields.Add(activatorField);

			type.Methods.Add(CreateActivateMethod(activatorField));
			type.Methods.Add(CreateBindMethod(activatorField));
		}

		private bool HasInstrumentedBaseType(TypeDefinition type)
		{   
			// is the baseType in the same assembly?
            TypeDefinition baseType = ResolveTypeReference(type.BaseType);
            if (baseType == null) return false;
			return RequiresTA(baseType);
		}

        private TypeDefinition ResolveTypeReference(TypeReference typeRef)
        {   
            TypeDefinition type = typeRef as TypeDefinition;
            if (null != type) return type;

            AssemblyNameReference assemblyRef = typeRef.Scope as AssemblyNameReference;
            if (IsSystemAssembly(assemblyRef)) return null;

            AssemblyDefinition assembly = LoadAssembly(assemblyRef);
            if (null == assembly) return null;
            return FindType(assembly, typeRef);
        }

        private bool IsSystemAssembly(AssemblyNameReference assemblyRef)
        {
            switch (assemblyRef.Name)
            {
                case "mscorlib":
                case "corlib":
                case "System":
                    return true;
            }
            return false;
        }

        private AssemblyDefinition LoadAssembly(AssemblyNameReference assemblyRef)
        {
            string assemblyPath = Path.Combine(Path.GetDirectoryName(_context.AssemblyLocation), assemblyRef.Name + ".dll");
            if (!File.Exists(assemblyPath)) return TryAssemblyResolver(assemblyRef);
            return AssemblyFactory.GetAssembly(assemblyPath);
        }

        private AssemblyDefinition TryAssemblyResolver(AssemblyNameReference assemblyRef)
        {
            return _context.Assembly.Resolver.Resolve(assemblyRef);
        }

        private TypeDefinition FindType(AssemblyDefinition assembly, TypeReference typeRef)
        {
            foreach (ModuleDefinition m in assembly.Modules)
            {
                foreach (TypeDefinition t in m.Types)
                {
                    if (t.FullName == typeRef.FullName) return t;
                }
            }
            return null;
        }

		private static bool RequiresTA(TypeDefinition type)
		{
			if (type.IsValueType) return false;
			if (type.IsInterface) return false;
			if (type.Name == "<Module>") return false;
			if (ByAttributeFilter.ContainsCustomAttribute(type, CompilerGeneratedAttribute)) return false;
			return true;
		}

		private MethodDefinition CreateActivateMethod(FieldDefinition activatorField)
		{
			MethodDefinition activate = new MethodDefinition(ActivateMethodName, MethodAttributes.Public, VoidType());
		
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
			TypeDefinition type = ResolveTypeReference(field.DeclaringType);
			if (type == null) return null;

			MethodDefinition[] methods = type.Methods.GetMethod(ActivateMethodName);
			if (methods.Length != 1) return null;

			return methods[0];
		}

		private static bool IsFieldAccess(Instruction instruction)
		{
			return instruction.OpCode == OpCodes.Ldfld
				|| instruction.OpCode == OpCodes.Ldflda;
		}

		private static bool IsPrivate(MethodDefinition method)
		{
			return method.IsCompilerControlled
				|| method.IsPrivate;
		}
	}
}
