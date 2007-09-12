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
			_activatorField = field;
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