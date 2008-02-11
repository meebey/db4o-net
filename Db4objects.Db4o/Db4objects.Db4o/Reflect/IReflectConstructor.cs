/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect
{
	/// <summary>Reflection Constructor representation.</summary>
	/// <remarks>
	/// Reflection Constructor representation
	/// <br/><br/>See documentation for System.Reflection API.
	/// </remarks>
	/// <seealso cref="IReflector">IReflector</seealso>
	public interface IReflectConstructor
	{
		void SetAccessible();

		IReflectClass[] GetParameterTypes();

		object NewInstance(object[] parameters);
	}
}
