namespace Db4objects.Db4o.Reflect
{
	/// <exclude></exclude>
	public class ReflectorUtils
	{
		public static Db4objects.Db4o.Reflect.IReflectClass ReflectClassFor(Db4objects.Db4o.Reflect.IReflector
			 reflector, object clazz)
		{
			clazz = Db4objects.Db4o.Internal.Platform4.GetClassForType(clazz);
			if (clazz is Db4objects.Db4o.Reflect.IReflectClass)
			{
				return (Db4objects.Db4o.Reflect.IReflectClass)clazz;
			}
			if (clazz is System.Type)
			{
				return reflector.ForClass((System.Type)clazz);
			}
			if (clazz is string)
			{
				return reflector.ForName((string)clazz);
			}
			return reflector.ForObject(clazz);
		}
	}
}
