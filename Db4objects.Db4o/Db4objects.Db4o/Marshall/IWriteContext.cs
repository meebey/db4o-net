/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Marshall;
using Db4objects.Db4o.Typehandlers;

namespace Db4objects.Db4o.Marshall
{
	/// <summary>
	/// this interface is passed to internal class com.db4o.internal.TypeHandler4 during marshalling
	/// and provides methods to marshal objects.
	/// </summary>
	/// <remarks>
	/// this interface is passed to internal class com.db4o.internal.TypeHandler4 during marshalling
	/// and provides methods to marshal objects.
	/// </remarks>
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
		/// be a primitive type, do not use this method but use
		/// a matching
		/// <see cref="IWriteBuffer">IWriteBuffer</see>
		/// method instead.
		/// </remarks>
		/// <param name="obj">the object to write.</param>
		void WriteObject(object obj);

		/// <summary>
		/// writes sub-objects, in cases where the TypeHandler4
		/// is known.
		/// </summary>
		/// <remarks>
		/// writes sub-objects, in cases where the TypeHandler4
		/// is known.
		/// </remarks>
		/// <param name="obj">the object to write</param>
		void WriteObject(ITypeHandler4 handler, object obj);

		/// <summary>
		/// reserves a buffer with a specific length at the current
		/// position, to be written in a later step.
		/// </summary>
		/// <remarks>
		/// reserves a buffer with a specific length at the current
		/// position, to be written in a later step.
		/// </remarks>
		/// <param name="length">the length to be reserved.</param>
		/// <returns>the ReservedBuffer</returns>
		IReservedBuffer Reserve(int length);
	}
}
