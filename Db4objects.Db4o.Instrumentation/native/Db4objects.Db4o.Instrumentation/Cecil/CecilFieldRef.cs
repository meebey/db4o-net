using System;
using Db4objects.Db4o.Instrumentation.Api;
using Mono.Cecil;

namespace Db4objects.Db4o.Instrumentation.Cecil
{
	public class CecilFieldRef : IFieldRef
	{
		private readonly FieldReference _field;

		public CecilFieldRef(FieldReference field)
		{
			_field = field;
		}

		public FieldReference Field
		{
			get { return _field; }
		}

		public TypeReference FieldType
		{
			get { return _field.FieldType; }
		}

		public ITypeRef Type
		{
			get { return new CecilTypeRef(FieldType); }
		}

		public string Name
		{
			get { return _field.Name; }
		}

		public override string ToString()
		{
			return Name;
		}
	}

	public class CecilTypeRef : ITypeRef
	{
		private readonly TypeReference _type;

		public CecilTypeRef(TypeReference type)
		{
			_type = type;
		}

		public bool IsPrimitive
		{
			get
			{
				switch (_type.FullName)
				{
					case "System.Int32":
					case "System.Boolean":
						return true;
				}
				return false;
			}
		}

		public ITypeRef ElementType
		{
			get { throw new NotImplementedException(); }
		}

		public string Name
		{
			get { return NormalizeNestedTypeNotation(_type.FullName); }
		}

		private static string NormalizeNestedTypeNotation(string fullName)
		{
			return fullName.Replace('/', '+');
		}

		public override string ToString()
		{
			return Name;
		}
	}
}
