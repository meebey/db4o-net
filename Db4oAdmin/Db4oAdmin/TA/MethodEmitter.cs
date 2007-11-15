using System;
using System.Reflection;
using Db4oAdmin.Core;
using Mono.Cecil;
using MethodAttributes=Mono.Cecil.MethodAttributes;

namespace Db4oAdmin.TA
{
	internal class MethodEmitter
	{
		protected FieldReference _activatorField;
		protected InstrumentationContext _context;

		public MethodEmitter(InstrumentationContext context, FieldReference field)
		{	
			_context = context;
			_activatorField = FieldReferenceFor(field);
		}

		private static FieldReference FieldReferenceFor(FieldReference field)
		{
			if (0 == field.DeclaringType.GenericParameters.Count) return field;
			return new FieldReference(field.Name, TypeReferenceFor(field.DeclaringType), field.FieldType);
		}

		private static TypeReference TypeReferenceFor(TypeReference type)
		{
			GenericInstanceType instance = new GenericInstanceType(type);
			foreach (GenericParameter param in type.GenericParameters)
			{
				instance.GenericArguments.Add(param);
			}
			return instance;
		}

		protected MethodDefinition NewExplicitMethod(MethodInfo method)
		{
			MethodAttributes attributes = MethodAttributes.SpecialName|MethodAttributes.Private|MethodAttributes.Virtual;
			MethodDefinition definition = new MethodDefinition(method.DeclaringType.FullName + "." + method.Name, attributes, Import(method.ReturnType));
			definition.Overrides.Add(_context.Import(method));
			return definition;
		}

		protected TypeReference Import(Type type)
		{
			return _context.Import(type);
		}
	}
}