/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com

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
using System;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Reflect;
using Db4objects.Drs;
using Db4objects.Drs.Db4o;

namespace Db4objects.Drs.Inside
{
	public class ReplicationReflector
	{
		private IInternalObjectContainer _container;

		public ReplicationReflector(IReplicationProvider providerA, IReplicationProvider 
			providerB)
		{
			_container = ContainerFrom(providerA);
			if (_container == null)
			{
				_container = ContainerFrom(providerB);
			}
			if (_container == null)
			{
				_container = (IInternalObjectContainer)ExtDb4oFactory.OpenMemoryFile(new MemoryFile
					()).Ext();
			}
		}

		public virtual object[] ArrayContents(object array)
		{
			int[] dim = ArrayReflector().Dimensions(array);
			object[] result = new object[Volume(dim)];
			ArrayReflector().Flatten(array, dim, 0, result, 0);
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

		public virtual IReflectClass ForObject(object obj)
		{
			return _container.Reflector().ForObject(obj);
		}

		public virtual IReflectClass ForClass(Type clazz)
		{
			return _container.Reflector().ForClass(clazz);
		}

		internal virtual IReflectClass GetComponentType(IReflectClass claxx)
		{
			return ArrayReflector().GetComponentType(claxx);
		}

		internal virtual int[] ArrayDimensions(object obj)
		{
			return ArrayReflector().Dimensions(obj);
		}

		public virtual object NewArrayInstance(IReflectClass componentType, int[] dimensions
			)
		{
			return ArrayReflector().NewInstance(componentType, dimensions);
		}

		public virtual int ArrayShape(object[] a_flat, int a_flatElement, object a_shaped
			, int[] a_dimensions, int a_currentDimension)
		{
			return ArrayReflector().Shape(a_flat, a_flatElement, a_shaped, a_dimensions, a_currentDimension
				);
		}

		public virtual bool IsSecondClass(IReflectClass clazz)
		{
			ClassMetadata classMetadata = _container.ClassMetadataForReflectClass(clazz);
			if (classMetadata == null)
			{
				return false;
			}
			return classMetadata.IsSecondClass();
		}

		private IInternalObjectContainer ContainerFrom(IReplicationProvider provider)
		{
			if (!(provider is IDb4oReplicationProvider))
			{
				return null;
			}
			IDb4oReplicationProvider db4oProvider = (IDb4oReplicationProvider)provider;
			IExtObjectContainer container = db4oProvider.GetObjectContainer();
			if (!(container is IInternalObjectContainer))
			{
				return null;
			}
			return (IInternalObjectContainer)container;
		}

		private IReflectArray ArrayReflector()
		{
			return _container.Reflector().Array();
		}
	}
}
