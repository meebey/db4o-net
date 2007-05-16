/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	/// <summary>representation for java.lang.reflect.Method.</summary>
	/// <remarks>
	/// representation for java.lang.reflect.Method.
	/// <br /><br />See the respective documentation in the JDK API.
	/// </remarks>
	/// <seealso cref="IReflector">IReflector</seealso>
	public interface IReflectMethod
	{
		object Invoke(object onObject, object[] parameters);

		IReflectClass GetReturnType();
	}
}
