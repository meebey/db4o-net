namespace Db4objects.Db4o.Reflect.Net
{

	/// <remarks>Reflection implementation for Constructor to map to JDK reflection.</remarks>
	public class NetConstructor : Db4objects.Db4o.Reflect.ReflectConstructor
	{
		private readonly Db4objects.Db4o.Reflect.Reflector reflector;

		private readonly System.Reflection.ConstructorInfo constructor;

		public NetConstructor(Db4objects.Db4o.Reflect.Reflector reflector, System.Reflection.ConstructorInfo
			 constructor)
		{
			this.reflector = reflector;
			this.constructor = constructor;
		}

		public virtual Db4objects.Db4o.Reflect.ReflectClass[] GetParameterTypes()
		{
			return Db4objects.Db4o.Reflect.Net.NetReflector.ToMeta(reflector, Sharpen.Runtime.GetParameterTypes(constructor));
		}

		public virtual void SetAccessible()
		{
			Db4objects.Db4o.Platform4.SetAccessible(constructor);
		}

		public virtual object NewInstance(object[] parameters)
		{
			try
			{
				return constructor.Invoke(parameters);
			}
			catch (System.Exception e)
			{
				return null;
			}
		}
	}
}
