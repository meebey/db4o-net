namespace Db4objects.Db4o.Reflect.Generic
{
	public interface IReflectClassBuilder
	{
		Db4objects.Db4o.Reflect.IReflectClass CreateClass(string name, Db4objects.Db4o.Reflect.IReflectClass
			 superClass, int fieldCount);

		Db4objects.Db4o.Reflect.IReflectField CreateField(Db4objects.Db4o.Reflect.IReflectClass
			 parentType, string fieldName, Db4objects.Db4o.Reflect.IReflectClass fieldType, 
			bool isVirtual, bool isPrimitive, bool isArray, bool isNArray);

		void InitFields(Db4objects.Db4o.Reflect.IReflectClass clazz, Db4objects.Db4o.Reflect.IReflectField[]
			 fields);

		Db4objects.Db4o.Reflect.IReflectField[] FieldArray(int length);
	}
}
