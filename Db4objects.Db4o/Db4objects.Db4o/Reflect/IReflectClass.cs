namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.Class.</summary>
	/// <remarks>
	/// representation for java.lang.Class.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="Db4objects.Db4o.Reflect.IReflector">Db4objects.Db4o.Reflect.IReflector
	/// 	</seealso>
	public interface IReflectClass
	{
		Db4objects.Db4o.Reflect.IReflectClass GetComponentType();

		Db4objects.Db4o.Reflect.IReflectConstructor[] GetDeclaredConstructors();

		Db4objects.Db4o.Reflect.IReflectField[] GetDeclaredFields();

		Db4objects.Db4o.Reflect.IReflectField GetDeclaredField(string name);

		Db4objects.Db4o.Reflect.IReflectClass GetDelegate();

		Db4objects.Db4o.Reflect.IReflectMethod GetMethod(string methodName, Db4objects.Db4o.Reflect.IReflectClass[]
			 paramClasses);

		string GetName();

		Db4objects.Db4o.Reflect.IReflectClass GetSuperclass();

		bool IsAbstract();

		bool IsArray();

		bool IsAssignableFrom(Db4objects.Db4o.Reflect.IReflectClass type);

		bool IsCollection();

		bool IsInstance(object obj);

		bool IsInterface();

		bool IsPrimitive();

		bool IsSecondClass();

		object NewInstance();

		Db4objects.Db4o.Reflect.IReflector Reflector();

		/// <summary>
		/// instructs to install or uninstall a special constructor for the
		/// respective platform that avoids calling the constructor for the
		/// respective class
		/// </summary>
		/// <param name="flag">
		/// true to try to install a special constructor, false if
		/// such a constructor is to be removed if present
		/// </param>
		/// <returns>true if the special constructor is in place after the call</returns>
		bool SkipConstructor(bool flag);

		void UseConstructor(Db4objects.Db4o.Reflect.IReflectConstructor constructor, object[]
			 @params);

		object[] ToArray(object obj);
	}
}
