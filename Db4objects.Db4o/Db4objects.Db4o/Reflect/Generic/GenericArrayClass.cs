/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;
using Db4objects.Db4o.Reflect.Generic;

namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericArrayClass : GenericClass
	{
		public GenericArrayClass(GenericReflector reflector, IReflectClass delegateClass, 
			string name, GenericClass superclass) : base(reflector, delegateClass, "(GA) " +
			 name, superclass)
		{
		}

		public override IReflectClass GetComponentType()
		{
			return GetDelegate();
		}

		public override bool IsArray()
		{
			return true;
		}

		public override bool IsInstance(object candidate)
		{
			if (!(candidate is GenericArray))
			{
				return false;
			}
			return IsAssignableFrom(((GenericArray)candidate)._clazz);
		}

		public override bool Equals(object obj)
		{
			if (!(obj is Db4objects.Db4o.Reflect.Generic.GenericArrayClass))
			{
				return false;
			}
			return base.Equals(obj);
		}
	}
}
