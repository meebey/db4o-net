namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.reflect.Constructor.</summary>
	/// <remarks>
	/// representation for java.lang.reflect.Constructor.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Reflect.IReflector">Db4objects.Db4o.Reflect.IReflector
	/// 	</seealso>
	public interface IReflectConstructor
	{
		void SetAccessible();

		Db4objects.Db4o.Reflect.IReflectClass[] GetParameterTypes();

		object NewInstance(object[] parameters);
	}
}
