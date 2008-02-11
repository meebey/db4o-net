/* Copyright (C) 2004 - 2008  db4objects Inc.  http://www.db4o.com */

using Db4objects.Db4o.Reflect;

namespace Db4objects.Db4o.Typehandlers
{
	/// <summary>
	/// Predicate to be able to select if a specific TypeHandler is
	/// applicable for a specific Type and version.
	/// </summary>
	/// <remarks>
	/// Predicate to be able to select if a specific TypeHandler is
	/// applicable for a specific Type and version.
	/// </remarks>
	public interface ITypeHandlerPredicate
	{
		/// <summary>
		/// return true if a TypeHandler is to be used for a specific
		/// Type and version
		/// </summary>
		/// <param name="classReflector">
		/// the Type passed by db4o that is to
		/// be tested by this predicate.
		/// </param>
		/// <param name="version">the version</param>
		/// <returns>
		/// true if the TypeHandler is to be used for a specific
		/// Type and version.
		/// </returns>
		bool Match(IReflectClass classReflector, int version);
	}
}
