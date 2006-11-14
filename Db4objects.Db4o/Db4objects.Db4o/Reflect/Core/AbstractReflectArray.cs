namespace Db4objects.Db4o.Reflect.Core
{
	/// <exclude></exclude>
	public abstract class AbstractReflectArray : Db4objects.Db4o.Reflect.IReflectArray
	{
		protected readonly Db4objects.Db4o.Reflect.IReflector _reflector;

		public AbstractReflectArray(Db4objects.Db4o.Reflect.IReflector reflector)
		{
			_reflector = reflector;
		}

		public abstract object NewInstance(Db4objects.Db4o.Reflect.IReflectClass componentType
			, int[] dimensions);

		public abstract object NewInstance(Db4objects.Db4o.Reflect.IReflectClass componentType
			, int length);

		public virtual int[] Dimensions(object arr)
		{
			int count = 0;
			Db4objects.Db4o.Reflect.IReflectClass claxx = _reflector.ForObject(arr);
			while (claxx.IsArray())
			{
				count++;
				claxx = claxx.GetComponentType();
			}
			int[] dim = new int[count];
			for (int i = 0; i < count; i++)
			{
				try
				{
					dim[i] = GetLength(arr);
					arr = Get(arr, 0);
				}
				catch
				{
					return dim;
				}
			}
			return dim;
		}

		public virtual int Flatten(object a_shaped, int[] a_dimensions, int a_currentDimension
			, object[] a_flat, int a_flatElement)
		{
			if (a_currentDimension == (a_dimensions.Length - 1))
			{
				for (int i = 0; i < a_dimensions[a_currentDimension]; i++)
				{
					a_flat[a_flatElement++] = GetNoExceptions(a_shaped, i);
				}
			}
			else
			{
				for (int i = 0; i < a_dimensions[a_currentDimension]; i++)
				{
					a_flatElement = Flatten(GetNoExceptions(a_shaped, i), a_dimensions, a_currentDimension
						 + 1, a_flat, a_flatElement);
				}
			}
			return a_flatElement;
		}

		public virtual object Get(object onArray, int index)
		{
			return Sharpen.Runtime.GetArrayValue(onArray, index);
		}

		public virtual Db4objects.Db4o.Reflect.IReflectClass GetComponentType(Db4objects.Db4o.Reflect.IReflectClass
			 a_class)
		{
			while (a_class.IsArray())
			{
				a_class = a_class.GetComponentType();
			}
			return a_class;
		}

		public virtual int GetLength(object array)
		{
			return Sharpen.Runtime.GetArrayLength(array);
		}

		private object GetNoExceptions(object onArray, int index)
		{
			try
			{
				return Get(onArray, index);
			}
			catch
			{
				return null;
			}
		}

		public virtual bool IsNDimensional(Db4objects.Db4o.Reflect.IReflectClass a_class)
		{
			return a_class.GetComponentType().IsArray();
		}

		public virtual void Set(object onArray, int index, object element)
		{
			if (element == null)
			{
				try
				{
					Sharpen.Runtime.SetArrayValue(onArray, index, element);
				}
				catch
				{
				}
			}
			else
			{
				Sharpen.Runtime.SetArrayValue(onArray, index, element);
			}
		}

		public virtual int Shape(object[] a_flat, int a_flatElement, object a_shaped, int[]
			 a_dimensions, int a_currentDimension)
		{
			if (a_currentDimension == (a_dimensions.Length - 1))
			{
				for (int i = 0; i < a_dimensions[a_currentDimension]; i++)
				{
					Set(a_shaped, i, a_flat[a_flatElement++]);
				}
			}
			else
			{
				for (int i = 0; i < a_dimensions[a_currentDimension]; i++)
				{
					a_flatElement = Shape(a_flat, a_flatElement, Get(a_shaped, i), a_dimensions, a_currentDimension
						 + 1);
				}
			}
			return a_flatElement;
		}
	}
}
