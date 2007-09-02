/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Marshall
{
	/// <summary>
	/// this interface is passed to
	/// <see cref="TypeHandler4">TypeHandler4</see>
	/// when instantiating objects.
	/// </summary>
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
	}
}
