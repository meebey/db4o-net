namespace Db4objects.Db4o.Reflect.Generic
{
	/// <exclude></exclude>
	public class GenericArrayClass : Db4objects.Db4o.Reflect.Generic.GenericClass
	{
		public GenericArrayClass(Db4objects.Db4o.Reflect.Generic.GenericReflector reflector
			, Db4objects.Db4o.Reflect.IReflectClass delegateClass, string name, Db4objects.Db4o.Reflect.Generic.GenericClass
			 superclass) : base(reflector, delegateClass, "(GA) " + name, superclass)
		{
		}

		public override Db4objects.Db4o.Reflect.IReflectClass GetComponentType()
		{
			return GetDelegate();
		}

		public override bool IsArray()
		{
			return true;
		}

		public override bool IsInstance(object candidate)
		{
			if (!(candidate is Db4objects.Db4o.Reflect.Generic.GenericArray))
			{
				return false;
			}
			return IsAssignableFrom(((Db4objects.Db4o.Reflect.Generic.GenericObject)candidate
				)._class);
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
