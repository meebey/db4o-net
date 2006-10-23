namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericArrayReflector : Db4objects.Db4o.Reflect.IReflectArray
	{
		private readonly Db4objects.Db4o.Reflect.IReflectArray _delegate;

		public GenericArrayReflector(Db4objects.Db4o.Reflect.Generic.GenericReflector reflector
			)
		{
			_delegate = reflector.GetDelegate().Array();
		}

		public virtual int[] Dimensions(object arr)
		{
			return _delegate.Dimensions(arr);
		}

		public virtual int Flatten(object a_shaped, int[] a_dimensions, int a_currentDimension
			, object[] a_flat, int a_flatElement)
		{
			return _delegate.Flatten(a_shaped, a_dimensions, a_currentDimension, a_flat, a_flatElement
				);
		}

		public virtual object Get(object onArray, int index)
		{
			if (onArray is Db4objects.Db4o.Reflect.Generic.GenericArray)
			{
				return ((Db4objects.Db4o.Reflect.Generic.GenericArray)onArray)._data[index];
			}
			return _delegate.Get(onArray, index);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetComponentType(Db4objects.Db4o.Reflect.IReflectClass
			 claxx)
		{
			claxx = claxx.GetDelegate();
			if (claxx is Db4objects.Db4o.Reflect.Generic.GenericClass)
			{
				return claxx;
			}
			return _delegate.GetComponentType(claxx);
		}

		public virtual int GetLength(object array)
		{
			if (array is Db4objects.Db4o.Reflect.Generic.GenericArray)
			{
				return ((Db4objects.Db4o.Reflect.Generic.GenericArray)array).GetLength();
			}
			return _delegate.GetLength(array);
		}

		public virtual bool IsNDimensional(Db4objects.Db4o.Reflect.IReflectClass a_class)
		{
			if (a_class is Db4objects.Db4o.Reflect.Generic.GenericArrayClass)
			{
				return false;
			}
			return _delegate.IsNDimensional(a_class.GetDelegate());
		}

		public virtual object NewInstance(Db4objects.Db4o.Reflect.IReflectClass componentType
			, int length)
		{
			componentType = componentType.GetDelegate();
			if (componentType is Db4objects.Db4o.Reflect.Generic.GenericClass)
			{
				return new Db4objects.Db4o.Reflect.Generic.GenericArray(((Db4objects.Db4o.Reflect.Generic.GenericClass
					)componentType).ArrayClass(), length);
			}
			return _delegate.NewInstance(componentType, length);
		}

		public virtual object NewInstance(Db4objects.Db4o.Reflect.IReflectClass componentType
			, int[] dimensions)
		{
			return _delegate.NewInstance(componentType.GetDelegate(), dimensions);
		}

		public virtual void Set(object onArray, int index, object element)
		{
			if (onArray is Db4objects.Db4o.Reflect.Generic.GenericArray)
			{
				((Db4objects.Db4o.Reflect.Generic.GenericArray)onArray)._data[index] = element;
				return;
			}
			_delegate.Set(onArray, index, element);
		}

		public virtual int Shape(object[] a_flat, int a_flatElement, object a_shaped, int[]
			 a_dimensions, int a_currentDimension)
		{
			return _delegate.Shape(a_flat, a_flatElement, a_shaped, a_dimensions, a_currentDimension
				);
		}
	}
}
