/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Reflection;
using System.Runtime.CompilerServices;
using Db4oAdmin.Core;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Internal;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes=Mono.Cecil.FieldAttributes;

namespace Db4oAdmin.TA
{   
	public class TAInstrumentation : AbstractAssemblyInstrumentation
	{
		public static readonly string CompilerGeneratedAttribute = typeof(CompilerGeneratedAttribute).FullName;

		private const string IT_TRANSPARENT_ACTIVATION = "TA";

        private CustomAttribute _instrumentationAttribute;
		private CecilReflector _reflector;

        protected override void BeforeAssemblyProcessing()
		{
			_reflector = new CecilReflector(_context);
            CreateTagAttribute();
        }

		protected override void ProcessModule(ModuleDefinition module)
		{
            if (AlreadyTAInstrumented())
            {
                _context.TraceWarning("Assembly already instrumented for Transparent Activation.");
                return;
            }
            MarkAsInstrumented();

            ProcessTypes(module.Types, MakeActivatable);
			ProcessTypes(module.Types, NoFiltering, ProcessMethods);
		}

        private void CreateTagAttribute()
        {
            _instrumentationAttribute = new CustomAttribute(ImportConstructor(typeof(TagAttribute)));
            _instrumentationAttribute.ConstructorParameters.Add(IT_TRANSPARENT_ACTIVATION);
        }

        private MethodReference ImportConstructor(System.Type type)
        {
            return _context.Import(type.GetConstructor(new System.Type[] { typeof(string) }));
        }

        private bool IsTATag(CustomAttribute ca)
        {
            return ca.Constructor.DeclaringType == _instrumentationAttribute.Constructor.DeclaringType &&
                   ca.Constructor.Parameters.Count == 1;
        }
        private bool AlreadyTAInstrumented()
        {
            foreach (CustomAttribute ca in _context.Assembly.CustomAttributes)
            {
                if (IsTATag(ca)) return true;
            }

            return false;
        }

        private void MarkAsInstrumented()
        {
            _context.Assembly.CustomAttributes.Add(_instrumentationAttribute);
        }

        private void MakeActivatable(TypeDefinition type)
		{
			if (!RequiresTA(type)) return;
			if (ImplementsActivatable(type)) return;
			if (HasInstrumentedBaseType(type)) return;

			type.Interfaces.Add(Import(typeof(Db4objects.Db4o.TA.IActivatable)));

			FieldDefinition activatorField = CreateActivatorField();
			type.Fields.Add(activatorField);

			type.Methods.Add(CreateActivateMethod(activatorField));
			type.Methods.Add(CreateBindMethod(activatorField));
		}

		private bool HasInstrumentedBaseType(TypeDefinition type)
		{
			// is the base type defined in the same assembly?
			TypeDefinition baseType = type.BaseType as TypeDefinition;
            if (baseType == null) return false;
			return RequiresTA(baseType);
		}

        private TypeDefinition ResolveTypeReference(TypeReference typeRef)
        {
        	return _reflector.ResolveTypeReference(typeRef);
        }

		private static bool RequiresTA(TypeDefinition type)
		{
			if (type.IsValueType) return false;
			if (type.IsInterface) return false;
			if (type.Name == "<Module>") return false;
			if (ByAttributeFilter.ContainsCustomAttribute(type, CompilerGeneratedAttribute)) return false;
			return true;
		}

		private bool ImplementsActivatable(TypeDefinition type)
		{
			return _reflector.Implements(type, typeof(IActivatable));
		}

		private MethodDefinition CreateActivateMethod(FieldDefinition activatorField)
		{
			return new ActivateMethodEmitter(_context, activatorField).Emit();
		}

		private FieldDefinition CreateActivatorField()
		{
			return new FieldDefinition("db4o$$ta$$activator", ActivatorType(), FieldAttributes.Private|FieldAttributes.NotSerialized);
		}

		private MethodDefinition CreateBindMethod(FieldReference activatorField)
		{
			return new BindMethodEmitter(_context, activatorField).Emit();
		}

		private TypeReference ActivatorType()
		{
			return Import(typeof(IActivator));
		}

		protected override void ProcessMethod(Mono.Cecil.MethodDefinition method)
		{
			if (!method.HasBody || method.IsCompilerControlled) return;

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

			cil.InsertBefore(instruction, cil.Create(OpCodes.Dup));
			cil.InsertBefore(instruction, cil.Create(OpCodes.Callvirt, Import(ActivateMethod())));
		}

		private static MethodInfo ActivateMethod()
		{
			return typeof(IActivatable).GetMethod("Activate");
		}

		private bool IsActivatableField(FieldReference field)
		{
			if (field.Name.Contains("$")) return false;

			TypeDefinition type = ResolveTypeReference(field.DeclaringType);
			if (type == null) return false;

            if (IsTransient(type, field)) return false;

			if (!Accept(type)) return false;

			return ImplementsActivatable(type);
		}

	    private bool IsTransient(TypeDefinition type, FieldReference fieldRef)
	    {
	        FieldDefinition field = type.Fields.GetField(fieldRef.Name);
            if (field == null) return true;
	        return field.IsNotSerialized;
	    }

	    private static bool IsFieldAccess(Instruction instruction)
		{
			return instruction.OpCode == OpCodes.Ldfld
				|| instruction.OpCode == OpCodes.Ldflda;
		}
	}
}
