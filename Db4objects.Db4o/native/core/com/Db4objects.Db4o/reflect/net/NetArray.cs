using System;

namespace Db4objects.Db4o.Reflect.Net
{
	/// <remarks>Reflection implementation for Array to map to .NET reflection.</remarks>
	public class NetArray : Db4objects.Db4o.Reflect.ReflectArray
	{
		private readonly Db4objects.Db4o.Reflect.Reflector _reflector;

		internal NetArray(Db4objects.Db4o.Reflect.Reflector reflector)
		{
			_reflector = reflector;
		}

		public virtual int[] Dimensions(object obj)
		{
			System.Array array = (System.Array)obj;
			int[] dim = new int[array.Rank];
			for (int i = 0; i < dim.Length; i++)
			{
				dim[i] = array.GetLength(i);
			}
			return dim;
		}

		public virtual int Flatten(
			object shaped,
			int[] dimensions,
			int currentDimension,
			object[] flat,
			int flatElement)
		{
			int[] currentDimensions = new int[dimensions.Length];
			Flatten1((System.Array)shaped, dimensions, 0, currentDimensions, flat, 0);
			return 0;
		}

		protected virtual int Flatten1(
			System.Array shaped,
			int[] allDimensions,
			int currentDimension,
			int[] currentDimensions,
			object[] flat,
			int flatElement)
		{
			if (currentDimension == (allDimensions.Length - 1))
			{
				for (currentDimensions[currentDimension] = 0; currentDimensions[currentDimension] < allDimensions[currentDimension]; currentDimensions[currentDimension]++)
				{
					flat[flatElement++] = shaped.GetValue(currentDimensions);
				}
			}
			else
			{
				for (currentDimensions[currentDimension] = 0; currentDimensions[currentDimension] < allDimensions[currentDimension]; currentDimensions[currentDimension]++)
				{
					flatElement =
						Flatten1(
						shaped,
						allDimensions,
						currentDimension + 1,
						currentDimensions,
						flat,
						flatElement);
				}
			}
			return flatElement;
		}

		public virtual object Get(object onArray, int index)
		{
			return ((System.Array)onArray).GetValue(index);
		}

		public virtual Db4objects.Db4o.Reflect.ReflectClass GetComponentType(Db4objects.Db4o.Reflect.ReflectClass
			 a_class)
		{
			return a_class.GetComponentType();
		}

		public virtual int GetLength(object array)
		{
			return ((System.Array)array).GetLength(0);
		}

		public virtual bool IsNDimensional(Db4objects.Db4o.Reflect.ReflectClass a_class)
		{
			Type type = GetNetType(a_class);
			return GetArrayRank(type) > 1;
		}

		private static Type GetNetType(ReflectClass a_class)
		{
			return ((NetClass)a_class).GetNetType();
		}

		private int GetArrayRank(Type type)
		{
#if CF_1_0 || CF_2_0
			return ((Sharpen.Lang.ArrayTypeReference)Sharpen.Lang.TypeReference.FromType(type)).Rank;
#else
			return type.GetArrayRank();
#endif
		}

		public virtual object NewInstance(Db4objects.Db4o.Reflect.ReflectClass componentType, int
			 length)
		{
			return System.Array.CreateInstance(GetNetType(componentType), length);
		}

		public virtual object NewInstance(Db4objects.Db4o.Reflect.ReflectClass componentType, int[]
			 dimensions)
		{
			return System.Array.CreateInstance(GetNetType(componentType), dimensions);
		}

		public virtual void Set(object onArray, int index, object element)
		{
			((System.Array)onArray).SetValue(element, index);
		}

		public virtual int Shape(
			object[] flat,
			int flatElement,
			object shaped,
			int[] allDimensions,
			int currentDimension)
		{
			int[] currentDimensions = new int[allDimensions.Length];
			Shape1(flat, 0, (System.Array)shaped, allDimensions, 0, currentDimensions);
			return 0;
		}

		public virtual int Shape1(
			object[] flat,
			int flatElement,
			System.Array shaped,
			int[] allDimensions,
			int currentDimension,
			int[] currentDimensions)
		{
			if (currentDimension == (allDimensions.Length - 1))
			{
				for (currentDimensions[currentDimension] = 0; currentDimensions[currentDimension] < allDimensions[currentDimension]; currentDimensions[currentDimension]++)
				{
					shaped.SetValue(flat[flatElement++], currentDimensions);
				}
			}
			else
			{
				for (currentDimensions[currentDimension] = 0; currentDimensions[currentDimension] < allDimensions[currentDimension]; currentDimensions[currentDimension]++)
				{
					flatElement =
						Shape1(
						flat,
						flatElement,
						shaped,
						allDimensions,
						currentDimension + 1,
						currentDimensions
						);
				}
			}
			return flatElement;
		}
	}
}
