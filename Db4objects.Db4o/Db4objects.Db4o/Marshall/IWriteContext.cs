/* Copyright (C) 2004 - 2007  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Marshall
{
	/// <summary>
	/// this interface is passed to
	/// <see cref="ITypeHandler4">ITypeHandler4</see>
	/// during marshalling
	/// and provides methods to marshall objects.
	/// </summary>
	public interface IWriteContext : IContext, IWriteBuffer
	{
		/// <summary>
		/// makes sure the object is stored and writes the ID of
		/// the object to the context.
		/// </summary>
		/// <remarks>
		/// makes sure the object is stored and writes the ID of
		/// the object to the context.
		/// Use this method for first class objects only (objects that
		/// have an identity in the database). If the object can potentially
		/// be a primitive type, do not use this method bue use
		/// <see cref="#writeAny(Object)">#writeAny(Object)</see>
		/// instead.
		/// </remarks>
		/// <param name="obj">the object to write.</param>
		void WriteObject(object obj);

		/// <summary>
		/// writes sub-objects, in cases where the
		/// <see cref="ITypeHandler4">ITypeHandler4</see>
		/// is known.
		/// </summary>
		/// <param name="obj">the object to write</param>
		void WriteObject(ITypeHandler4 handler, object obj);
	}
}
