namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericField : Db4objects.Db4o.Reflect.IReflectField, Db4objects.Db4o.Foundation.IDeepClone
	{
		private readonly string _name;

		private readonly Db4objects.Db4o.Reflect.Generic.GenericClass _type;

		private readonly bool _primitive;

		private readonly bool _array;

		private readonly bool _nDimensionalArray;

		private int _index = -1;

		public GenericField(string name, Db4objects.Db4o.Reflect.IReflectClass clazz, bool
			 primitive, bool array, bool nDimensionalArray)
		{
			_name = name;
			_type = (Db4objects.Db4o.Reflect.Generic.GenericClass)clazz;
			_primitive = primitive;
			_array = array;
			_nDimensionalArray = nDimensionalArray;
		}

		public virtual object DeepClone(object obj)
		{
			Db4objects.Db4o.Reflect.IReflector reflector = (Db4objects.Db4o.Reflect.IReflector
				)obj;
			Db4objects.Db4o.Reflect.IReflectClass newReflectClass = null;
			if (_type != null)
			{
				newReflectClass = reflector.ForName(_type.GetName());
			}
			return new Db4objects.Db4o.Reflect.Generic.GenericField(_name, newReflectClass, _primitive
				, _array, _nDimensionalArray);
		}

		public virtual object Get(object onObject)
		{
			return ((Db4objects.Db4o.Reflect.Generic.GenericObject)onObject).Get(_index);
		}

		public virtual string GetName()
		{
			return _name;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetFieldType()
		{
			if (_array)
			{
				return _type.ArrayClass();
			}
			return _type;
		}

		public virtual bool IsPublic()
		{
			return true;
		}

		public virtual bool IsPrimitive()
		{
			return _primitive;
		}

		public virtual bool IsStatic()
		{
			return false;
		}

		public virtual bool IsTransient()
		{
			return false;
		}

		public virtual void Set(object onObject, object value)
		{
			((Db4objects.Db4o.Reflect.Generic.GenericObject)onObject).Set(_index, value);
		}

		public virtual void SetAccessible()
		{
		}

		internal virtual void SetIndex(int index)
		{
			_index = index;
		}

		public virtual object IndexEntry(object orig)
		{
			return orig;
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass IndexType()
		{
			return GetFieldType();
		}
	}
}
