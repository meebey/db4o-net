/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Config
{
	/// <summary>
	/// Custom class handler to provide modified instantiation,
	/// marshalling and querying behaviour for special classes.
	/// </summary>
	/// <remarks>
	/// Custom class handler to provide modified instantiation,
	/// marshalling and querying behaviour for special classes.
	/// </remarks>
	public interface ICustomClassHandler
	{
		/// <summary>implement to create a new instance of an object.</summary>
		/// <remarks>implement to create a new instance of an object.</remarks>
		/// <returns>the new Object</returns>
		object NewInstance();

		/// <summary>
		/// implement and return true, if this CustomClassHandler creates
		/// object instances on calls to
		/// <see cref="ICustomClassHandler.NewInstance">ICustomClassHandler.NewInstance</see>
		/// .
		/// </summary>
		/// <returns>true, if this CustomClassHandler creates new instances.</returns>
		bool CanNewInstance();

		IReflectClass ClassSubstitute();

		bool IgnoreAncestor();
	}
}
