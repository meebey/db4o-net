/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Config;
using Db4objects.Db4o.Ext;
using Db4objects.Db4o.Internal;
using Db4objects.Db4o.Internal.Delete;
using Db4objects.Db4o.Internal.Fieldhandlers;
using Db4objects.Db4o.Marshall;

namespace Db4objects.Db4o.Typehandlers
{
	/// <summary>
	/// handles reading, writing, deleting, defragmenting and
	/// comparisons for types of objects.<br /><br />
	/// Custom Typehandlers can be implemented to alter the default
	/// behaviour of storing all non-transient fields of an object.<br /><br />
	/// </summary>
	/// <seealso>
	/// 
	/// <see cref="IConfiguration.RegisterTypeHandler">IConfiguration.RegisterTypeHandler
	/// 	</see>
	/// 
	/// </seealso>
	public interface ITypeHandler4 : IFieldHandler, IComparable4
	{
		// 	TODO: Not all TypeHandlers can implement Comparable4.
		// Consider to change the hierarchy, not to extend Comparable4
		// and to have callers check, if Comparable4 is implemented by 
		// a TypeHandler.
		/// <summary>gets called when an object gets deleted.</summary>
		/// <remarks>gets called when an object gets deleted.</remarks>
		/// <param name="context"></param>
		/// <exception cref="Db4oIOException">Db4oIOException</exception>
		void Delete(IDeleteContext context);

		/// <summary>gets called when an object gets defragmented.</summary>
		/// <remarks>gets called when an object gets defragmented.</remarks>
		/// <param name="context"></param>
		void Defragment(IDefragmentContext context);

		/// <summary>gets called when an object is read from the database.</summary>
		/// <remarks>gets called when an object is read from the database.</remarks>
		/// <param name="context"></param>
		/// <returns>the instantiated object</returns>
		object Read(IReadContext context);

		/// <summary>gets called when an object is to be written to the database.</summary>
		/// <remarks>gets called when an object is to be written to the database.</remarks>
		/// <param name="context"></param>
		/// <param name="obj">the object</param>
		void Write(IWriteContext context, object obj);
	}
}
