/* Copyright (C) 2007   db4objects Inc.   http://www.db4o.com */
using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;
using Db4oTool.Core;
using Db4objects.Db4o.Activation;
using Db4objects.Db4o.TA;
using Db4objects.Db4o.Internal;
using Mono.Cecil;
using Mono.Cecil.Cil;
using FieldAttributes=Mono.Cecil.FieldAttributes;
using MethodBody=Mono.Cecil.Cil.MethodBody;

namespace Db4oTool.TA
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

        private MethodReference ImportConstructor(Type type)
        {
            return _context.Import(type.GetConstructor(new Type[] { typeof(string) }));
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

			type.Interfaces.Add(Import(typeof(IActivatable)));

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
			if (IsStaticClass(type)) return false;
			if (type.IsInterface) return false;
			if (type.Name == "<Module>") return false;
			if (IsDelegate(type)) return false;
			if (ByAttributeFilter.ContainsCustomAttribute(type, CompilerGeneratedAttribute)) return false;
			if (!HasSerializableFields(type) && type.BaseType.FullName == "System.Object") return false;
			return true;
		}

		private static bool IsStaticClass(TypeDefinition type)
		{
			return type.IsAbstract && type.IsSealed;
		}

		private bool HasSerializableFields(TypeDefinition type)
		{
			foreach (FieldDefinition field in type.Fields)
				if (IsSerializable(field))
					return true;
			return false;
		}

		private bool IsSerializable(FieldDefinition field)
		{
			TypeDefinition fieldType = ResolveTypeReference(field.FieldType);
			if (field.IsNotSerialized || (fieldType != null && (IsDelegate(fieldType) || IsWin32Handle(fieldType))))
			{
				return false;
			}
			return !IsPointer(field.FieldType);
		}

		private static bool IsWin32Handle(TypeReference type)
		{
			if (type == null) return false;

			if (type.FullName == "System.Runtime.InteropServices.SafeHandle" || type.FullName == "System.IntPtr") return true;

			TypeDefinition typeDefinition = type as TypeDefinition;
			if (typeDefinition == null) return false;

			TypeReference baseType = typeDefinition.BaseType;
			return IsWin32Handle(baseType);
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

		protected override void ProcessMethod(MethodDefinition method)
		{
			if (!method.HasBody || method.IsCompilerControlled) return;

			if (!HasFieldAccesses(method)) return;
			
			method.Body.Simplify();

			InstrumentFieldAccesses(method);

			method.Body.Optimize();
		}

		private bool HasFieldAccesses(MethodDefinition method)
		{
			return FieldAccesses(method.Body).GetEnumerator().MoveNext();
		}

		private void InstrumentFieldAccesses(MethodDefinition method)
		{
//			CilWorker cil = method.Body.CilWorker;
			MethodEditor editor = new MethodEditor(method);
			foreach (Instruction instruction in FieldAccesses(method.Body))
			{
				ProcessFieldAccess(editor, instruction);
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

		private void ProcessFieldAccess(MethodEditor cil, Instruction instruction)
		{
            if (IsFieldGetter(instruction))
            {
                ProcessFieldGetter(instruction, cil);
            }
            else
            {
                ProcessFieldSetter(instruction, cil);
            }
		}

	    private void ProcessFieldSetter(Instruction instruction, MethodEditor cil)
	    {
            VariableDefinition oldStackTop = SaveStackTop(cil, instruction);

	        Instruction insertionPoint = GetInsertionPoint(instruction);
			InsertActivateCall(cil, insertionPoint, ActivationPurpose.Write);
			cil.InsertBefore(insertionPoint, cil.Create(OpCodes.Ldloc, oldStackTop));
        }

	    private static VariableDefinition SaveStackTop(MethodEditor cil, Instruction instruction)
	    {
	    	VariableDefinition oldStackTop = cil.AddVariable(Resolve(instruction).FieldType);
	    	
	        cil.InsertBefore(GetInsertionPoint(instruction), cil.Create(OpCodes.Stloc, oldStackTop));

            return oldStackTop;
	    }

		private static FieldReference Resolve(Instruction instruction)
	    {
	        return (FieldReference)instruction.Operand;
	    }

	    private static bool IsFieldGetter(Instruction instruction)
	    {
	        return instruction.OpCode == OpCodes.Ldfld || instruction.OpCode == OpCodes.Ldflda;
	    }

	    private void ProcessFieldGetter(Instruction instruction, MethodEditor cil)
	    {
	        Instruction insertionPoint = GetInsertionPoint(instruction);

	    	InsertActivateCall(cil, insertionPoint, ActivationPurpose.Read);
	    }

		private void InsertActivateCall(MethodEditor cil, Instruction insertionPoint, ActivationPurpose activationPurpose)
		{
			Instruction previous = insertionPoint.Previous;
			if (previous.OpCode == OpCodes.Ldarg)
			{
				Instruction newLoadInstruction = cil.Create(previous.OpCode, (ParameterDefinition)previous.Operand);
				InsertActivateCall(cil,
					previous,
					newLoadInstruction,
					activationPurpose);
			}
			else
			{
				InsertActivateCall(cil,
					insertionPoint,
					cil.Create(OpCodes.Dup),
					activationPurpose);
			}
		}

		private void InsertActivateCall(MethodEditor cil, Instruction insertionPoint, Instruction loadReferenceInstruction, ActivationPurpose activationPurpose)
		{
			cil.InsertBefore(insertionPoint, loadReferenceInstruction);
			cil.InsertBefore(insertionPoint, cil.Create(OpCodes.Ldc_I4, (int)activationPurpose));
			cil.InsertBefore(insertionPoint, cil.Create(OpCodes.Callvirt, ActivateMethodRef()));
		}

		private MethodReference ActivateMethodRef()
		{
			return Import(ActivateMethod());
		}

		public FieldReference Import(FieldInfo field)
		{
			return _context.Assembly.MainModule.Import(field);
		}

		private static Instruction GetInsertionPoint(Instruction instruction)
		{
			return instruction.Previous.OpCode == OpCodes.Volatile
				? instruction.Previous
				: instruction;
		}

		private static MethodInfo ActivateMethod()
		{
			return typeof(IActivatable).GetMethod("Activate", new Type[] { typeof(ActivationPurpose) });
		}

		private bool IsActivatableField(FieldReference field)
		{
			if (DeclaredInNonActivatableType(field))
				return false;

			return IsActivatableFieldType(field.FieldType);
		}

		private bool IsActivatableFieldType(TypeReference fieldTypeRef)
		{
			if (IsPointer(fieldTypeRef)) return false;

			TypeDefinition fieldType = ResolveTypeReference(fieldTypeRef);
			if (null == fieldType)
			{	
				// we dont know the field type but it doesn't hurt
				// to call Activate
				// filtering would be only an optimization
				return true;
			}
			return !IsDelegate(fieldType);
		}

		private bool DeclaredInNonActivatableType(FieldReference field)
		{
			TypeDefinition declaringType = ResolveTypeReference(field.DeclaringType);
			if (declaringType == null) return true;

			if (IsTransient(declaringType, field)) return true;
			if (!Accept(declaringType)) return true;

			if (!ImplementsActivatable(declaringType)) return true;
			return false;
		}

		private static bool IsPointer(TypeReference type)
		{
			return type is PointerType;
		}

		private static bool IsDelegate(TypeDefinition type)
		{
			TypeReference baseType = type.BaseType;
			if (null == baseType) return false;

			string fullName = baseType.FullName;
			return fullName == "System.Delegate"
				|| fullName == "System.MulticastDelegate";
		}

		private static bool IsTransient(TypeDefinition type, IMemberReference fieldRef)
	    {
	        FieldDefinition field = type.Fields.GetField(fieldRef.Name);
            if (field == null) return true;
	        return field.IsNotSerialized;
	    }

	    private static bool IsFieldAccess(Instruction instruction)
		{
	        return instruction.OpCode == OpCodes.Ldfld
	               || instruction.OpCode == OpCodes.Ldflda
	               || instruction.OpCode == OpCodes.Stfld;
		}
	}
}
