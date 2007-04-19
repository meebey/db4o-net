using Db4objects.Db4o.Foundation;
using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericField : IReflectField, IDeepClone
	{
		private readonly string _name;

		private readonly GenericClass _type;

		private readonly bool _primitive;

		private readonly bool _array;

		private readonly bool _nDimensionalArray;

		private int _index = -1;

		public GenericField(string name, IReflectClass clazz, bool primitive, bool array, 
			bool nDimensionalArray)
		{
			_name = name;
			_type = (GenericClass)clazz;
			_primitive = primitive;
			_array = array;
			_nDimensionalArray = nDimensionalArray;
		}

		public virtual object DeepClone(object obj)
		{
			IReflector reflector = (IReflector)obj;
			IReflectClass newReflectClass = null;
			if (_type != null)
			{
				newReflectClass = reflector.ForName(_type.GetName());
			}
			return new Db4objects.Db4o.Reflect.Generic.GenericField(_name, newReflectClass, _primitive
				, _array, _nDimensionalArray);
		}

		public virtual object Get(object onObject)
		{
			return ((GenericObject)onObject).Get(_index);
		}

		public virtual string GetName()
		{
			return _name;
		}

		public virtual IReflectClass GetFieldType()
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
			((GenericObject)onObject).Set(_index, value);
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

		public virtual IReflectClass IndexType()
		{
			return GetFieldType();
		}
	}
}
