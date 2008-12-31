/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Reflect.Core
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
		IReflectClass[] GetParameterTypes();

		object NewInstance(object[] parameters);
	}
}
