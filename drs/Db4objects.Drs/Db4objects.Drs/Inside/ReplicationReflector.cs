/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

namespace Db4objects.Drs.Inside
{
	public class ReplicationReflector
	{
		private static Db4objects.Drs.Inside.ReplicationReflector instance = new Db4objects.Drs.Inside.ReplicationReflector
			();

		private readonly Db4objects.Db4o.Reflect.IReflector _reflector;

		private readonly Db4objects.Db4o.Reflect.IReflectArray _arrayReflector;

		private ReplicationReflector()
		{
			Db4objects.Db4o.Ext.IExtObjectContainer tempOcToGetReflector = Db4objects.Db4o.Ext.ExtDb4oFactory
				.OpenMemoryFile(new Db4objects.Db4o.Ext.MemoryFile()).Ext();
			//      FIXME: Find a better way without depending on ExtDb4o.  :P
			_reflector = tempOcToGetReflector.Reflector();
			_arrayReflector = _reflector.Array();
			tempOcToGetReflector.Close();
		}

		public static Db4objects.Drs.Inside.ReplicationReflector GetInstance()
		{
			return instance;
		}

		public virtual object[] ArrayContents(object array)
		{
			int[] dim = _arrayReflector.Dimensions(array);
			object[] result = new object[Volume(dim)];
			_arrayReflector.Flatten(array, dim, 0, result, 0);
			//TODO Optimize add a visit(Visitor) method to ReflectArray or navigate the array to avoid copying all this stuff all the time.
			return result;
		}

		private int Volume(int[] dim)
		{
			int result = dim[0];
			for (int i = 1; i < dim.Length; i++)
			{
				result = result * dim[i];
			}
			return result;
		}

		internal virtual Db4objects.Db4o.Reflect.IReflectClass ForObject(object obj)
		{
			return _reflector.ForObject(obj);
		}

		internal virtual Db4objects.Db4o.Reflect.IReflectClass GetComponentType(Db4objects.Db4o.Reflect.IReflectClass
			 claxx)
		{
			return _arrayReflector.GetComponentType(claxx);
		}

		internal virtual int[] ArrayDimensions(object obj)
		{
			return _arrayReflector.Dimensions(obj);
		}

		public virtual object NewArrayInstance(Db4objects.Db4o.Reflect.IReflectClass componentType
			, int[] dimensions)
		{
			return _arrayReflector.NewInstance(componentType, dimensions);
		}

		public virtual int ArrayShape(object[] a_flat, int a_flatElement, object a_shaped
			, int[] a_dimensions, int a_currentDimension)
		{
			return _arrayReflector.Shape(a_flat, a_flatElement, a_shaped, a_dimensions, a_currentDimension
				);
		}

		public virtual Db4objects.Db4o.Reflect.IReflector Reflector()
		{
			return _reflector;
		}
	}
}
