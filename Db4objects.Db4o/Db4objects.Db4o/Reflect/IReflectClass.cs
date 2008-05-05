/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

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

		// FIXME: remove. Reintroduced since OM depends on it - refactor OM.
		object[] ToArray(object obj);

		object NullValue();

		void CreateConstructor();
	}
}
