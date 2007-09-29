/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com

This file is part of the db4o open source object database.

db4o is free software; you can redistribute it and/or modify it under
the terms of version 2 of the GNU General Public License as published
by the Free Software Foundation and as clarified by db4objects' GPL 
interpretation policy, available at
http://www.db4o.com/about/company/legalpolicies/gplinterpretation/
Alternatively you can write to db4objects, Inc., 1900 S Norfolk Street,
Suite 350, San Mateo, CA 94403, USA.

db4o is distributed in the hope that it will be useful, but WITHOUT ANY
WARRANTY; without even the implied warranty of MERCHANTABILITY or
FITNESS FOR A PARTICULAR PURPOSE.  See the GNU General Public License
for more details.

You should have received a copy of the GNU General Public License along
with this program; if not, write to the Free Software Foundation, Inc.,
59 Temple Place - Suite 330, Boston, MA  02111-1307, USA. */
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
