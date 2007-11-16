/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Db4oAdmin.Core;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Internal;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes=Mono.Cecil.FieldAttributes;
using MethodBody=Mono.Cecil.Cil.MethodBody;

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

		private bool RequiresTA(TypeDefinition type)
		{
			if (type.IsValueType) return false;
			if (type.IsInterface) return false;
			if (type.Name == "<Module>") return false;
			if (IsDelegate(type)) return false;
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

			if (!HasFieldAccesses(method)) return;
			
			method.Body.Modify();

			InstrumentFieldAccesses(method);

			method.Body.Optimize();
		}

		private bool HasFieldAccesses(MethodDefinition method)
		{
			return FieldAccesses(method.Body).GetEnumerator().MoveNext();
		}

		private void InstrumentFieldAccesses(MethodDefinition method)
		{
			CilWorker cil = method.Body.CilWorker;
			foreach (Instruction instruction in FieldAccesses(method.Body))
			{
				ProcessFieldAccess(cil, instruction);
			}
		}

		private IEnumerable<Instruction> FieldAccesses(MethodBody body)
		{	
			Instruction instruction = body.Instructions[0];
			while (instruction != null)
			{
				if (IsActivatableFieldAccess(instruction))
				{
					yield return instruction;
				}
				instruction = instruction.Next;
			}
		}

		private bool IsActivatableFieldAccess(Instruction instruction)
		{
			if (!IsFieldAccess(instruction)) return false;
			return IsActivatableField((FieldReference) instruction.Operand);
		}

		private void ProcessFieldAccess(CilWorker cil, Instruction instruction)
		{
			FieldReference field = (FieldReference)instruction.Operand;
			Instruction insertionPoint = GetInsertionPoint(instruction);

			cil.InsertBefore(insertionPoint, cil.Create(OpCodes.Dup));
			cil.InsertBefore(insertionPoint, cil.Create(OpCodes.Callvirt, Import(ActivateMethod())));
		}

		private Instruction GetInsertionPoint(Instruction instruction)
		{
			return instruction.Previous.OpCode == OpCodes.Volatile
				? instruction.Previous
				: instruction;
		}

		private static MethodInfo ActivateMethod()
		{
			return typeof(IActivatable).GetMethod("Activate");
		}

		private bool IsActivatableField(FieldReference field)
		{
			if (field.Name.Contains("$")) return false;

			TypeDefinition declaringType = ResolveTypeReference(field.DeclaringType);
			if (declaringType == null) return false;

            if (IsTransient(declaringType, field)) return false;
			if (!Accept(declaringType)) return false;

			if (!ImplementsActivatable(declaringType)) return false;

			if (IsPointer(field.FieldType)) return false;

			TypeDefinition fieldType = ResolveTypeReference(field.FieldType);
			if (null == fieldType)
			{	
				// we dont know the field type but it doesn't hurt
				// to call Activate
				// filtering would be only an optimization
				return true;
			}
			return !IsDelegate(fieldType);
		}

		private bool IsPointer(TypeReference type)
		{
			return type is PointerType;
		}

		private bool IsDelegate(TypeDefinition type)
		{
			TypeReference baseType = type.BaseType;
			if (null == baseType) return false;

			string fullName = baseType.FullName;
			return fullName == "System.Delegate"
				|| fullName == "System.MulticastDelegate";
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
