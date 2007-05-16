/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.reflect.Field.</summary>
	/// <remarks>
	/// representation for java.lang.reflect.Field.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="IReflector">IReflector</seealso>
	public interface IReflectField
	{
		object Get(object onObject);

		string GetName();

		IReflectClass GetFieldType();

		bool IsPublic();

		bool IsStatic();

		bool IsTransient();

		void Set(object onObject, object value);

		void SetAccessible();

		IReflectClass IndexType();

		object IndexEntry(object orig);
	}
}
