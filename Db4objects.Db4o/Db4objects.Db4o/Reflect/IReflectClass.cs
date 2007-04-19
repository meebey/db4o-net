using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.Class.</summary>
	/// <remarks>
	/// representation for java.lang.Class.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="IReflector">IReflector</seealso>
	public interface IReflectClass
	{
		IReflectClass GetComponentType();

		IReflectConstructor[] GetDeclaredConstructors();

		IReflectField[] GetDeclaredFields();

		IReflectField GetDeclaredField(string name);

		IReflectClass GetDelegate();

		IReflectMethod GetMethod(string methodName, IReflectClass[] paramClasses);

		string GetName();

		IReflectClass GetSuperclass();

		bool IsAbstract();

		bool IsArray();

		bool IsAssignableFrom(IReflectClass type);

		bool IsCollection();

		bool IsInstance(object obj);

		bool IsInterface();

		bool IsPrimitive();

		bool IsSecondClass();

		object NewInstance();

		IReflector Reflector();

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

		void UseConstructor(IReflectConstructor constructor, object[] @params);

		object[] ToArray(object obj);
	}
}
