/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	/// <summary>Reflection Class representation.</summary>
	/// <remarks>
	/// Reflection Class representation
	/// <br/><br/>See documentation for System.Reflection API.
	/// </remarks>
	/// <seealso cref="IReflector">IReflector</seealso>
	public interface IReflectClass
	{
		IReflectClass GetComponentType();

		IReflectConstructor[] GetDeclaredConstructors();

		IReflectField[] GetDeclaredFields();

		IReflectField GetDeclaredField(string name);

		/// <summary>Returns the ReflectClass instance being delegated to.</summary>
		/// <remarks>
		/// Returns the ReflectClass instance being delegated to.
		/// If there's no delegation it should return this.
		/// </remarks>
		/// <returns>delegate or this</returns>
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
		/// <param name="testConstructor">true, if the special constructor shall be tested, false if it shall be set without testing
		/// 	</param>
		/// <returns>true if the special constructor is in place after the call</returns>
		bool SkipConstructor(bool flag, bool testConstructor);

		void UseConstructor(IReflectConstructor constructor, object[] @params);

		// FIXME: remove. Reintroduced since OM depends on it - refactor OM.
		object[] ToArray(object obj);
	}
}
