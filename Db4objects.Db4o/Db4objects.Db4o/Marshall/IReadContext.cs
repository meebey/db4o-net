/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Marshall
{
	/// <summary>
	/// this interface is passed to internal class com.db4o.internal.TypeHandler4
	/// when instantiating objects.
	/// </summary>
	/// <remarks>
	/// this interface is passed to internal class com.db4o.internal.TypeHandler4
	/// when instantiating objects.
	/// </remarks>
	public interface IReadContext : IContext, IReadBuffer
	{
		/// <summary>
		/// Interprets the current position in the context as
		/// an ID and returns the object with this ID.
		/// </summary>
		/// <remarks>
		/// Interprets the current position in the context as
		/// an ID and returns the object with this ID.
		/// </remarks>
		/// <returns>the object</returns>
		object ReadObject();

		/// <summary>
		/// reads sub-objects, in cases where the TypeHandler4
		/// is known.
		/// </summary>
		/// <remarks>
		/// reads sub-objects, in cases where the TypeHandler4
		/// is known.
		/// </remarks>
		object ReadObject(ITypeHandler4 handler);
	}
}
