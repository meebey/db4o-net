namespace Db4objects.Db4o.Reflect.Generic
{
	public class GenericClassBuilder : Db4objects.Db4o.Reflect.Generic.IReflectClassBuilder
	{
		private Db4objects.Db4o.Reflect.Generic.GenericReflector _reflector;

		private Db4objects.Db4o.Reflect.IReflector _delegate;

		public GenericClassBuilder(Db4objects.Db4o.Reflect.Generic.GenericReflector reflector
			, Db4objects.Db4o.Reflect.IReflector delegate_) : base()
		{
			_reflector = reflector;
			_delegate = delegate_;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass CreateClass(string name, Db4objects.Db4o.Reflect.IReflectClass
			 superClass, int fieldCount)
		{
			Db4objects.Db4o.Reflect.IReflectClass nativeClass = _delegate.ForName(name);
			Db4objects.Db4o.Reflect.Generic.GenericClass clazz = new Db4objects.Db4o.Reflect.Generic.GenericClass
				(_reflector, nativeClass, name, (Db4objects.Db4o.Reflect.Generic.GenericClass)superClass
				);
			clazz.SetDeclaredFieldCount(fieldCount);
			return clazz;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectField CreateField(Db4objects.Db4o.Reflect.IReflectClass
			 parentType, string fieldName, Db4objects.Db4o.Reflect.IReflectClass fieldType, 
			bool isVirtual, bool isPrimitive, bool isArray, bool isNArray)
		{
			if (isVirtual)
			{
				return new Db4objects.Db4o.Reflect.Generic.GenericVirtualField(fieldName);
			}
			return new Db4objects.Db4o.Reflect.Generic.GenericField(fieldName, fieldType, isPrimitive
				, isArray, isNArray);
		}

		public virtual void InitFields(Db4objects.Db4o.Reflect.IReflectClass clazz, Db4objects.Db4o.Reflect.IReflectField[]
			 fields)
		{
			((Db4objects.Db4o.Reflect.Generic.GenericClass)clazz).InitFields((Db4objects.Db4o.Reflect.Generic.GenericField[]
				)fields);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass ArrayClass(Db4objects.Db4o.Reflect.IReflectClass
			 clazz)
		{
			return ((Db4objects.Db4o.Reflect.Generic.GenericClass)clazz).ArrayClass();
		}

		public virtual Db4objects.Db4o.Reflect.IReflectField[] FieldArray(int length)
		{
			return new Db4objects.Db4o.Reflect.Generic.GenericField[length];
		}
	}
}
